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
        private Sprite _sprite;
        public Sprite XnaSprite
        {
            get { return _sprite; }
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

        public SpriteAction(Sprite spr)
            : base(spr)
        {
            _sprite = spr;
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

        public void Draw(RODirection direction, float z)
        {

        }

        public void Draw(SpriteBatch sb, Microsoft.Xna.Framework.Point pos, SpriteAction parent, bool ext)
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
                _sprite.Draw(mo, i, sb, pos.X, pos.Y, ext);
        }
    }
}
