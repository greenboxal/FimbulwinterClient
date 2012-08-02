using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FimbulwinterClient.Gui.System
{
    public class Border : Control
    {
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Microsoft.Xna.Framework.GameTime gt)
        {
            int absX = (int)GetAbsX();
            int absY = (int)GetAbsY();

            Color clr = Color.FromNonPremultiplied(197, 206, 230, 255);

            // top line
            Vector2 p1 = new Vector2(absX + 2, absY);
            Vector2 p2 = new Vector2(absX + (int)Size.X - 1 - 3, absY);
            Utils.DrawLine(sb, clr, p1, p2);

            // bottom line
            p1 = new Vector2(absX + 2, absY + (int)Size.Y - 1);
            p2 = new Vector2(absX + (int)Size.X - 1 - 3, absY + (int)Size.Y - 1);
            Utils.DrawLine(sb, clr, p1, p2);

            // left top to bottom
            p1 = new Vector2(absX, absY + 3);
            p2 = new Vector2(absX, absY + (int)Size.Y - 3);
            Utils.DrawLine(sb, clr, p1, p2);

            // top left corner
            p1 = new Vector2(absX, absY + 1);
            p2 = new Vector2(absX + 2, absY + 1);
            Utils.DrawLine(sb, clr, p1, p2);

            p1 = new Vector2(absX, absY + 2);
            p2 = new Vector2(absX + 1, absY + 2);
            Utils.DrawLine(sb, clr, p1, p2);

            // top right corner
            p1 = new Vector2(absX + (int)Size.X - 1 - 3, absY + 1);
            p2 = new Vector2(absX + (int)Size.X - 1 - 1, absY + 1);
            Utils.DrawLine(sb, clr, p1, p2);

            p1 = new Vector2(absX + (int)Size.X - 1 - 2, absY + 2);
            p2 = new Vector2(absX + (int)Size.X - 1 - 1, absY + 2);
            Utils.DrawLine(sb, clr, p1, p2);

            // bottom left corner
            p1 = new Vector2(absX, absY + (int)Size.Y - 2);
            p2 = new Vector2(absX + 2, absY + (int)Size.Y - 2);
            Utils.DrawLine(sb, clr, p1, p2);

            p1 = new Vector2(absX, absY + (int)Size.Y - 3);
            p2 = new Vector2(absX + 1, absY + (int)Size.Y - 3);
            Utils.DrawLine(sb, clr, p1, p2);
            
            // right line
            p1 = new Vector2(absX + (int)Size.X - 1, absY + 3);
            p2 = new Vector2(absX + (int)Size.X - 1, absY + (int)Size.Y - 3);
            Utils.DrawLine(sb, clr, p1, p2);

            // bottom right corner
            p1 = new Vector2(absX + (int)Size.X - 1 - 3, absY + (int)Size.Y - 2);
            p2 = new Vector2(absX + (int)Size.X - 1 - 1, absY + (int)Size.Y - 2);
            Utils.DrawLine(sb, clr, p1, p2);

            p1 = new Vector2(absX + (int)Size.X - 1 - 2, absY + (int)Size.Y - 3);
            p2 = new Vector2(absX + (int)Size.X - 1 - 1, absY + (int)Size.Y - 3);
            Utils.DrawLine(sb, clr, p1, p2);
            //clr = Color.FromNonPremultiplied(255, 0, 0, 255);
            /*
            */
        }
    }
}
