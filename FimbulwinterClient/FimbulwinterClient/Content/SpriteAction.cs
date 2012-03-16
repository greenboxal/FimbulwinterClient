using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FimbulwinterClient.Content
{
    public class SpriteAction : ROFormats.SpriteAction
    {
        private Sprite m_sprite;
        public Sprite XnaSprite
        {
            get { return m_sprite; }
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
            : base(spr)
        {
            m_sprite = spr;
        }

        public float GetDelay(int i)
        {
            if (i < Delays.Count)
                return Delays[i];

            return 4.0f;
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

            Act act = Actions[m_action];
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

        public void Draw(SpriteBatch sb, Microsoft.Xna.Framework.Point pos, SpriteAction parent, bool ext)
        {
            Act act = Actions[m_action];
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
