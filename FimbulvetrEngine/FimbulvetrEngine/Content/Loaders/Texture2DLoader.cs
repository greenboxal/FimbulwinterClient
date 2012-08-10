using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Graphics;

namespace FimbulvetrEngine.Content.Loaders
{
    public class Texture2DLoader : IContentLoader
    {
        public object LoadContent(ContentManager contentManager, string contentName, System.IO.Stream stream)
        {
            return TextureManager.Instance.LoadFromStream(stream);
        }
    }
}
