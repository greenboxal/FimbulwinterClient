using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.GUI.System
{
    public class Button : Control
    {
        private bool pressed;
        private bool hover;

        public Button()
        {
            TabStop = false;
        }

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            int absX = (int)GetAbsX();
            int absY = (int)GetAbsY();

            int imgX = 0;
            int imgY = 0;

            if (!pressed)
            {
                if (hover)
                {
                    imgX = 29;
                    imgY = 40;
                }
                else
                {
                    imgX = 29;
                    imgY = 20;
                }
            }
            else
            {
                imgX = 29;
                imgY = 0;
            }

            int middleW = (int)Size.X - 10;

            Color clr = Color.White;
            if (Parent.Dragging)
                clr = Color.White * 0.5f;

            sb.Draw(FormSkin, new Rectangle(absX, absY, 5, 20), new Rectangle(imgX, imgY, 5, 20), clr);
            sb.Draw(FormSkin, new Rectangle(absX + 5, absY, middleW, 20), new Rectangle(imgX + 6, imgY, 5, 20), clr);
            sb.Draw(FormSkin, new Rectangle(absX + 5 + middleW, absY, 5, 20), new Rectangle(imgX + 59, imgY, 5, 20), clr);

            Vector2 textSize = Gulim8.MeasureString(this.Text);
            sb.DrawString(Gulim8, this.Text, new Vector2((float)absX + (this.Size.X / 2) - (textSize.X / 2), absY + 5), ForeColor);

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
