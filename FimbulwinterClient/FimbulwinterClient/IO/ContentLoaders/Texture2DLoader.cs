using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace FimbulwinterClient.IO.ContentLoaders
{
    public class Texture2DLoader : IContentLoader
    {
        public object LoadContent(ROContentManager rcm, Stream s)
        {
            return Texture2D.FromStream(rcm.GraphicsDevice, s);
        }
    }
}
