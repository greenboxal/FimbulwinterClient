using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FimbulwinterClient.Core.Assets
{
    public class SpriteAction
    {
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
            public Color Mask;
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

        private float _delay;
        public float Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        private int _action;
        public int Action
        {
            get { return _action; }
            set { _action = value; _frame = 0; _delay = 0; }
        }

        private int _frame;
        public int Frame
        {
            get { return _frame; }
            set { _frame = value; }
        }

        private bool _loop;
        public bool Loop
        {
            get { return _loop; }
            set { _loop = value; }
        }

        private bool _playing;
        public bool Playing
        {
            get { return _playing; }
            set { _playing = value; }
        }

        public SpriteAction(Sprite sprite)
        {
            _sprite = sprite;

            _events = new List<string>();
            _actions = new List<Act>();
            _delays = new List<float>();
        }

        public bool Load(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);

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
                                    sc.Mask = new Color(255, 255, 255, 255);
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

                                        sc.Mask = new Color(r, g, b, a);

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

        public float GetDelay(int i)
        {
            if (i < Delays.Count)
                return Delays[i];

            return 4.0f;
        }

        public void Update(GameTime gt)
        {
            _delay += (int)gt.ElapsedGameTime.TotalMilliseconds;

            float d = GetDelay(_action) * 25;
            while (_delay > d)
            {
                _delay -= d;
                _frame++;
            }

            Act act = Actions[_action];
            if (_frame >= act.Motions.Count)
            {
                if (_loop)
                {
                    _frame = _frame % act.Motions.Count;
                }
                else
                {
                    _playing = false;
                    _frame = act.Motions.Count - 1;
                }
            }
        }

        public void SetPalette(Palette pal)
        {
            _sprite.SetPalette(pal);
        }

        public void Draw(RODirection direction, float z)
        {

        }

        public void Draw(SpriteBatch sb, Microsoft.Xna.Framework.Point pos, SpriteAction parent, bool ext, SpriteEffects se = SpriteEffects.None)
        {
            Act act = Actions[_action];
            Motion mo = act.Motions[_frame];

            if (parent != null)
            {
                Motion pmo = parent.Actions[_action].Motions[_frame];

                if (pmo.AttachPoints.Count > 0)
                {
                    pos.X += pmo.AttachPoints[0].Position.X;
                    pos.Y += pmo.AttachPoints[0].Position.Y;
                }
            }

            for (int i = 0; i < mo.Clips.Count; i++)
                _sprite.Draw(mo, i, sb, pos.X, pos.Y, ext, se);
        }
    }
}
