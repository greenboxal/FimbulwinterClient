using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using FimbulwinterClient.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace FimbulwinterClient.Content
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

        private short m_version;
        public short Version
        {
            get { return m_version; }
            set { m_version = value; }
        }

        private Sprite m_sprite;
        public Sprite Sprite
        {
            get { return m_sprite; }
        }

        private List<string> m_events;
        public List<string> Events
        {
            get { return m_events; }
        }

        private List<Act> m_actions;
        private List<Act> Actions
        {
            get { return m_actions; }
        }

        private List<float> m_delays;
        public List<float> Delays
        {
            get { return m_delays; }
            set { m_delays = value; }
        }

        private float m_delay;
        public float Delay
        {
            get { return m_delay; }
            set { m_delay = value; }
        }

        private int m_action;
        public int Action
        {
            get { return m_action; }
            set { m_action = value; m_frame = 0; m_delay = 0; }
        }

        private int m_frame;
        public int Frame
        {
            get { return m_frame; }
            set { m_frame = value; }
        }

        private bool m_loop;
        public bool Loop
        {
            get { return m_loop; }
            set { m_loop = value; }
        }

        private bool m_playing;
        public bool Playing
        {
            get { return m_playing; }
            set { m_playing = value; }
        }

        public SpriteAction(Sprite spr)
        {
            m_sprite = spr;

            m_events = new List<string>();
            m_actions = new List<Act>();
            m_delays = new List<float>();
        }

        public float GetDelay(int i)
        {
            if (i < m_delays.Count)
                return m_delays[i];

            return 4.0f;
        }

        public bool Load(Stream s)
        {
            BinaryReader br = new BinaryReader(s);

            byte[] magic = br.ReadBytes(2);

            if (magic[0] != (byte)'A' || magic[1] != (byte)'C')
            {
                return false;
            }

            m_version = br.ReadInt16();

            if (m_version != 0x200 && m_version != 0x201 &&
                m_version != 0x202 && m_version != 0x203 &&
                m_version != 0x204 && m_version != 0x205)
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
                                    sc.Mask = Color.White;
                                    sc.Zoom = new Vector2(1.0f);
                                    sc.Angle = 0;
                                    sc.SpriteType = 0;
                                    sc.Size = new Point(0, 0);

                                    if (m_version >= 0x200)
                                    {
                                        byte r = br.ReadByte();
                                        byte g = br.ReadByte();
                                        byte b = br.ReadByte();
                                        byte a = br.ReadByte();

                                        sc.Mask = Color.FromNonPremultiplied(r, g, b, a);

                                        if (m_version >= 0x204)
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

                                        if (m_version >= 0x205)
                                        {
                                            sc.Size.X = br.ReadInt32();
                                            sc.Size.Y = br.ReadInt32();
                                        }
                                    }

                                    mo.Clips.Add(sc);
                                }
                            }

                            mo.EventID = -1;
                            if (m_version >= 0x200)
                            {
                                mo.EventID = br.ReadInt32();

                                if (m_version == 0x200)
                                    mo.EventID = -1;
                            }

                            if (m_version >= 0x203)
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

                    m_actions.Add(act);
                }
            }

            if (m_version >= 0x201)
            {
                uint eventCount = br.ReadUInt32();

                if (eventCount > 0)
                {
                    for (int i = 0; i < eventCount; i++)
                    {
                        m_events.Add(br.ReadCString(40));
                    }
                }
            }

            if (m_version >= 0x202)
            {
                if (m_events.Count > 0)
                {
                    for (int i = 0; i < m_events.Count; i++)
                    {
                        m_delays.Add(br.ReadSingle());
                    }
                }
            }

            return true;
        }

        public void Update(GameTime gt)
        {
            m_delay += (int)gt.ElapsedGameTime.TotalMilliseconds;

            float d = GetDelay(m_action) * 25;
            while (m_delay > d)
            {
                m_delay -= d;
                m_frame++;
            }

            Act act = m_actions[m_action];
            if (m_frame >= act.Motions.Count)
            {
                if (m_loop)
                {
                    m_frame = m_frame % act.Motions.Count;
                }
                else
                {
                    m_playing = false;
                    m_frame = act.Motions.Count - 1;
                }
            }
        }

        public void Draw(RODirection direction, float z)
        {

        }

        public void Draw(SpriteBatch sb, Point pos, SpriteAction parent, bool ext)
        {
            Act act = m_actions[m_action];
            Motion mo = act.Motions[m_frame];

            if (parent != null)
            {
                Motion pmo = parent.Actions[m_action].Motions[m_frame];

                if (pmo.AttachPoints.Count > 0)
                {
                    pos.X += pmo.AttachPoints[0].Position.X;
                    pos.Y += pmo.AttachPoints[0].Position.Y;
                }
            }

            for (int i = 0; i < mo.Clips.Count; i++)
                m_sprite.Draw(mo, i, sb, pos.X, pos.Y, ext);
        }
    }
}
