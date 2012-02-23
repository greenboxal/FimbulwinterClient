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

                Bitmap.FromStream(s).Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                return Texture2D.FromStream(rcm.Game.GraphicsDevice, ms);
            }
            else
            {
                return Texture2D.FromStream(rcm.Game.GraphicsDevice, s);
            }
        }
    }
}
