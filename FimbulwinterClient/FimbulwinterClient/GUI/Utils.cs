using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.GUI
{
    public class Utils
    {
        public static void DrawBackground(SpriteBatch sb, Color c, int x, int y, int w, int h)
        {
            Texture2D tex = new Texture2D(sb.GraphicsDevice, w, h);
            uint[] colors = new uint[w * h];

            for (int i = 0; i < colors.Length; i++)
                colors[i] = c.PackedValue;

            tex.SetData(colors);

            sb.Draw(tex, new Rectangle(x, y, w, h), Color.White);
        }
    }
}
