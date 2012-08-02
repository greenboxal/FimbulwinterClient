using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nuclex.Input;
using FimbulwinterClient.Core;

namespace FimbulwinterClient.Gui.System
{
    public class Window : Control
    {
        private float dragDeltaX;
        private float dragDeltaY;

        public Texture2D FullImage { get; set; }

        public Window()
        {
            this.Size = new Vector2(280, 120);
            this.Text = "Window";
        }

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            int absX = (int)GetAbsX();
            int absY = (int)GetAbsY();


            Color clr = Color.White;
            if (Dragging)
                clr = Color.White * 0.5f;

            if (FullImage == null)
            {
                Rectangle tLeft = new Rectangle(absX, absY, 13, 17);
                Rectangle tMid = new Rectangle(absX + 13, absY, (int)Size.X - 13 - 3, 17);
                Rectangle tRight = new Rectangle(absX + 13 + tMid.Width, absY, 3, 17);

                Rectangle mLeft = new Rectangle(absX, absY + 17, 1, (int)Size.Y - 17 - 28);
                Rectangle mMid = new Rectangle(absX + 1, absY + 17, (int)Size.X - 2, (int)Size.Y - 17 - 28);
                Rectangle mRight = new Rectangle(absX + 1 + mMid.Width, absY + 17, 1, (int)Size.Y - 17 - 28);

                Rectangle bLeft = new Rectangle(absX, absY + 17 + mMid.Height, 3, 28);
                Rectangle bMid = new Rectangle(absX + 3, absY + 17 + mMid.Height, (int)Size.X - 6, 28);
                Rectangle bRight = new Rectangle(absX + bMid.Width + 3, absY + 17 + mMid.Height, 3, 28);

                sb.Draw(FormSkin, tLeft, new Rectangle(0, 0, 14, 17), clr);
                sb.Draw(FormSkin, tMid, new Rectangle(14, 0, 11, 17), clr);
                sb.Draw(FormSkin, tRight, new Rectangle(25, 0, 3, 17), clr);

                sb.Draw(FormSkin, mLeft, new Rectangle(0, 17, 1, 8), clr);
                sb.Draw(FormSkin, mMid, new Rectangle(6, 20, 2, 2), clr);
                sb.Draw(FormSkin, mRight, new Rectangle(27, 17, 1, 8), clr);

                sb.Draw(FormSkin, bLeft, new Rectangle(0, 25, 3, 28), clr);
                sb.Draw(FormSkin, bMid, new Rectangle(3, 25, 22, 28), clr);
                sb.Draw(FormSkin, bRight, new Rectangle(25, 25, 3, 28), clr);
                sb.DrawString(Gulim8, this.Text, new Vector2(absX + 18, absY + 4), Color.White);
                sb.DrawString(Gulim8, this.Text, new Vector2(absX + 17, absY + 3), Color.Black);
            }
            else
            {
                sb.Draw(FullImage, new Vector2(absX, absY), clr);
            }

            base.Draw(sb, gt);
        }

        public override void OnMouseUp(MouseButtons buttons, float x, float y)
        {
            Dragging = false;
        }

        public override void OnMouseMove(float x, float y)
        {
            if (Dragging)
            {
                float px = GetAbsX() + x - dragDeltaX;
                float py = GetAbsY() + y - dragDeltaY;
                if (px < 0) px = 0;
                if (py < 0) py = 0;
                if (px + this.Size.X > SharedInformation.Config.ScreenWidth) px = SharedInformation.Config.ScreenWidth - this.Size.X;
                if (py + this.Size.Y > SharedInformation.Config.ScreenHeight) py = SharedInformation.Config.ScreenHeight - this.Size.Y;
                this.Position = new Vector2(px, py);
            }
        }

        public override void OnMouseDown(MouseButtons buttons, float x, float y)
        {
            Dragging = true;
            dragDeltaX = x;
            dragDeltaY = y;
        }

        public void Close()
        {
            if (Parent == null)
                GuiManager.Singleton.EnqueueRemove(this);
        }
    }
}
