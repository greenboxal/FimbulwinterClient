using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.GUI.System
{
    public class Label : Control
    {
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Microsoft.Xna.Framework.GameTime gt)
        {
            sb.DrawString(Arial10, this.Text, new Microsoft.Xna.Framework.Vector2(GetAbsX(), GetAbsY()), ForeColor);
        }
    }
}
