using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.Gui
{
    public class Utils
    {
        static private Texture2D _empty_texture;

        public static void Init(GraphicsDevice gd)
        {
            _empty_texture = new Texture2D(gd, 1, 1, false, SurfaceFormat.Color);
            _empty_texture.SetData(new[] { Color.White });
        }

        public static void DrawBackground(SpriteBatch sb, Color c, int x, int y, int w, int h)
        {
            sb.Draw(_empty_texture, new Rectangle(x, y, w, h), c);
        }

        static public void DrawLine(SpriteBatch batch, Color color,
                                    Vector2 point1, Vector2 point2)
        {

            DrawLine(batch, color, point1, point2, 0);
        }

        static public void DrawLine(SpriteBatch batch, Color color, Vector2 point1,
                                    Vector2 point2, float Layer)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = (point2 - point1).Length();

            batch.Draw(_empty_texture, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, 1),
                       SpriteEffects.None, Layer);
        }
    }
}
