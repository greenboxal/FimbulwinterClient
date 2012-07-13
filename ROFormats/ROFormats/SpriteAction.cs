using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Extensions;

namespace ROFormats
{
    public class SpriteAction
    {
        public struct Point
        {
            public int X, Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        public struct Vector2
        {
            public float X, Y;

            public Vector2(float x, float y)
            {
                X = x;
                Y = y;
            }

            public Vector2(float xy)
            {
                X = xy;
                Y = xy;
            }
        }

        public struct Rectangle
        {
            public int X, Y, Width, Height;

            public Rectangle(int x, int y, int w, int h)
            {
                X = x;
                Y = y;
                Width = w;
                Height = h;
            }
        }

        public struct AttachPoint
        {
            public Point Position;
            public int Attribute;
        }

        public struct SpriteClip
        {
            public Point Position;
            public int SpriteNumber;
            public bool Mirror;
            public Palette.Color Mask;
            public Vector2 Zoom;
            public int Angle;
            public int SpriteType;
            public Point Size;
        }

        public struct Motion
        {
            public Rectangle Range1;
            public Rectangle Range2;
            public int EventID;

            public List<SpriteClip> Clips;
            public List<AttachPoint> AttachPoints;
        }

        public struct Act
        {
            public List<Motion> Motions;
        }

        private short _version;
        public short Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private Sprite _sprite;
        public Sprite Sprite
        {
            get { return _sprite; }
        }

        private List<string> _events;
        public List<string> Events
        {
            get { return _events; }
        }

        private List<Act> _actions;
        public List<Act> Actions
        {
            get { return _actions; }
        }

        private List<float> _delays;
        public List<float> Delays
        {
            get { return _delays; }
            set { _delays = value; }
        }

        public SpriteAction(Sprite spr)
        {
            _sprite = spr;

            _events = new List<string>();
            _actions = new List<Act>();
            _delays = new List<float>();
        }

        public bool Load(Stream s)
        {
            BinaryReader br = new BinaryReader(s);

            byte[] magic = br.ReadBytes(2);

            if (magic[0] != (byte)'A' || magic[1] != (byte)'C')
            {
                return false;
            }

            _version = br.ReadInt16();

            if (_version != 0x200 && _version != 0x201 &&
                _version != 0x202 && _version != 0x203 &&
                _version != 0x204 && _version != 0x205)
            {
                return false;
            }

            ushort actCount = br.ReadUInt16();
            br.ReadBytes(10);

            if (actCount > 0)
            {
                for (int i = 0; i < actCount; i++)
                {
                    Act act = new Act();

                    uint motionCount = br.ReadUInt32();
                    if (motionCount > 0)
                    {
                        act.Motions = new List<Motion>();

                        for (int n = 0; n < motionCount; n++)
                        {
                            Motion mo = new Motion();

                            mo.Range1.X = br.ReadInt32();
                            mo.Range1.Y = br.ReadInt32();
                            mo.Range1.Width = br.ReadInt32();
                            mo.Range1.Height = br.ReadInt32();

                            mo.Range2.X = br.ReadInt32();
                            mo.Range2.Y = br.ReadInt32();
                            mo.Range2.Width = br.ReadInt32();
                            mo.Range2.Height = br.ReadInt32();

                            uint clipCount = br.ReadUInt32();
                            if (clipCount > 0)
                            {
                                mo.Clips = new List<SpriteClip>();

                                for (int j = 0; j < clipCount; j++)
                                {
                                    SpriteClip sc = new SpriteClip();

                                    sc.Position.X = br.ReadInt32();
                                    sc.Position.Y = br.ReadInt32();

                                    sc.SpriteNumber = br.ReadInt32();
                                    sc.Mirror = br.ReadInt32() == 1;
                                    sc.Mask = new Palette.Color(255, 255, 255, 255);
                                    sc.Zoom = new Vector2(1.0f);
                                    sc.Angle = 0;
                                    sc.SpriteType = 0;
                                    sc.Size = new Point(0, 0);

                                    if (_version >= 0x200)
                                    {
                                        byte r = br.ReadByte();
                                        byte g = br.ReadByte();
                                        byte b = br.ReadByte();
                                        byte a = br.ReadByte();

                                        sc.Mask = new Palette.Color(r, g, b, a);

                                        if (_version >= 0x204)
                                        {
                                            sc.Zoom.X = br.ReadSingle();
                                            sc.Zoom.Y = br.ReadSingle();
                                        }
                                        else
                                        {
                                            sc.Zoom = new Vector2(br.ReadSingle());
                                        }

                                        sc.Angle = br.ReadInt32();
                                        sc.SpriteType = br.ReadInt32();

                                        if (_version >= 0x205)
                                        {
                                            sc.Size.X = br.ReadInt32();
                                            sc.Size.Y = br.ReadInt32();
                                        }
                                    }

                                    mo.Clips.Add(sc);
                                }
                            }

                            mo.EventID = -1;
                            if (_version >= 0x200)
                            {
                                mo.EventID = br.ReadInt32();

                                if (_version == 0x200)
                                    mo.EventID = -1;
                            }

                            if (_version >= 0x203)
                            {
                                uint attachCount = br.ReadUInt32();

                                if (attachCount > 0)
                                {
                                    mo.AttachPoints = new List<AttachPoint>();

                                    for (int j = 0; j < attachCount; j++)
                                    {
                                        AttachPoint ap = new AttachPoint();

                                        br.ReadInt32();

                                        ap.Position.X = br.ReadInt32();
                                        ap.Position.Y = br.ReadInt32();
                                        ap.Attribute = br.ReadInt32();

                                        mo.AttachPoints.Add(ap);
                                    }
                                }
                            }

                            act.Motions.Add(mo);
                        }
                    }

                    _actions.Add(act);
                }
            }

            if (_version >= 0x201)
            {
                uint eventCount = br.ReadUInt32();

                if (eventCount > 0)
                {
                    for (int i = 0; i < eventCount; i++)
                    {
                        _events.Add(br.ReadCString(40));
                    }
                }
            }

            if (_version >= 0x202)
            {
                if (_events.Count > 0)
                {
                    for (int i = 0; i < _events.Count; i++)
                    {
                        _delays.Add(br.ReadSingle());
                    }
                }
            }

            return true;
        }
    }
}
