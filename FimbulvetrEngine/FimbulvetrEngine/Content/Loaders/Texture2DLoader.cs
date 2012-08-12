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

            // FIXME: Deadlock occururing
            background = true;

            if (background)
            {
                ContentManager.Instance.EnqueueBackgroundLoading(o => LoadContentSub(texture, contentName, true));
            }
            else
            {
                if (!LoadContentSub(texture, contentName, false))
                    return null;
            }

            contentManager.CacheContent(contentName, texture);

            return texture;
        }

        private bool LoadContentSub(Texture2D texture, string contentName, bool background)
        {
            Stream stream = FileSystemManager.Instance.OpenStream(contentName);

            if (stream == null)
                return false;

            TextureManager.Instance.LoadFromStream(stream, texture, background);
            return true;
        }
    }
}
