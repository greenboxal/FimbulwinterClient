using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.GUI.System
{
    public class CheckBox : Control
    {
        public bool Checked { get; set; }

        public CheckBox()
        {
            this.Size = new Vector2(10, 10);
        }

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            int absX = (int)GetAbsX();
            int absY = (int)GetAbsY();

            base.Draw(sb, gt);

            if (Checked)
            {
                sb.Draw(FormSkin, new Rectangle(absX, absY, 10, 10), new Rectangle(95, 12, 10, 10), Color.White);
            }
            else
            {
                sb.Draw(FormSkin, new Rectangle(absX, absY, 10, 10), new Rectangle(95, 0, 10, 10), Color.White);
            }

            sb.DrawString(Font, Text, new Vector2(absX + 12, absY - 4), ForeColor);
        }

        public override void OnClick(Nuclex.Input.MouseButtons buttons, float x, float y)
        {
            Checked = !Checked;

            base.OnClick(buttons, x, y);
        }
    }
}
