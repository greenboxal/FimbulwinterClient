using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FimbulwinterClient.Gui.System
{
    public class Label : Control
    {
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Microsoft.Xna.Framework.GameTime gt)
        {
            sb.DrawString(Font, this.Text, new Microsoft.Xna.Framework.Vector2(GetAbsX(), GetAbsY()), ForeColor);
        }
    }
}
