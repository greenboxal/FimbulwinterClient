using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Graphics;
using FimbulvetrEngine.IO;

namespace FimbulvetrEngine.Content.Loaders
{
    public class Texture2DLoader : IContentLoader
    {
        public object LoadContent(ContentManager contentManager, string contentName, bool background)
        {
            Texture2D texture = new Texture2D();

            if (background)
            {
                ContentManager.Instance.EnqueueBackgroundLoading(o =>
                {
                    Stream stream = FileSystemManager.Instance.OpenStream(contentName);

                    if (stream == null)
                        return;

                    TextureManager.Instance.LoadFromStream(stream, texture, background);
                });
            }
            else
            {
                Stream stream = FileSystemManager.Instance.OpenStream(contentName);

                if (stream == null)
                    return null;

                TextureManager.Instance.LoadFromStream(stream, texture, background);
            }

            contentManager.CacheContent(contentName, texture);

            return texture;
        }
    }
}
