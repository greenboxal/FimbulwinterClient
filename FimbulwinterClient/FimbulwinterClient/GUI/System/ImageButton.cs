using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.GUI.System
{
    public class ImageButton : Control
    {
        private bool pressed;
        private bool hover;

        Texture2D _n, _h, _p;

        public ImageButton(Texture2D n, Texture2D h, Texture2D p)
        {
            TabStop = false;
            _n = n;
            _h = h;
            _p = p;
        }

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            int absX = (int)GetAbsX();
            int absY = (int)GetAbsY();

            if (!pressed)
            {
                if (hover)
                {
                    sb.Draw(_h, new Rectangle(absX, absY, (int)this.Size.X, (int)this.Size.Y), Color.White);
                }
                else
                {
                    sb.Draw(_n, new Rectangle(absX, absY, (int)this.Size.X, (int)this.Size.Y), Color.White);
                }
            }
            else
            {
                sb.Draw(_p, new Rectangle(absX, absY, (int)this.Size.X, (int)this.Size.Y), Color.White);
            }

            base.Draw(sb, gt);
        }

        public override void OnMouseHover()
        {
            hover = true;

            base.OnMouseHover();
        }

        public override void OnMouseLeave()
        {
            hover = false;

            base.OnMouseLeave();
        }

        public override void OnMouseDown(Nuclex.Input.MouseButtons buttons, float x, float y)
        {
            pressed = true;

            base.OnMouseDown(buttons, x, y);
        }

        public override void OnMouseUp(Nuclex.Input.MouseButtons buttons, float x, float y)
        {
            pressed = false;

            base.OnMouseUp(buttons, x, y);
        }
    }
}
