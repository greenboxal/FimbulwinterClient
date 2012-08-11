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
            Texture2D texture = TextureManager.Instance.LoadFromStream(stream);

            if (texture == null)
                return null;

            contentManager.CacheContent(contentName, texture);

            return texture;
        }
    }
}
