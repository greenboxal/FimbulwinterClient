using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace FimbulwinterClient.IO.ContentLoaders
{
    public class Texture2DLoader : IContentLoader
    {
        public object LoadContent(ROContentManager rcm, Stream s, string fn)
        {
            if (fn.EndsWith(".bmp"))
            {
                MemoryStream ms = new MemoryStream();

                Bitmap bmp = (Bitmap)Bitmap.FromStream(s);
                bmp.MakeTransparent(Color.Fuchsia);
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                s.Close();

                return Texture2D.FromStream(rcm.Game.GraphicsDevice, ms);
            }
            else
            {
                Texture2D r = Texture2D.FromStream(rcm.Game.GraphicsDevice, s);

                s.Close();

                return r;
            }
        }
    }
}
