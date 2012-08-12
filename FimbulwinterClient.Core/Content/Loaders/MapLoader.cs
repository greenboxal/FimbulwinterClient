using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using FimbulwinterClient.Core.Graphics;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class MapLoader : IContentLoader
    {
        public object LoadContent(ContentManager contentManager, string contentName, bool background)
        {
            Map map = new Map();
            WorldRenderer result = new WorldRenderer(map);
            string baseName = Path.GetFileNameWithoutExtension(contentName);

            if (background)
            {
                ContentManager.Instance.EnqueueBackgroundLoading(o => LoadContentSub(map, result, baseName, true));
            }
            else
            {
                if (!LoadContentSub(map, result, baseName, false))
                    return null;
            }

            return result;
        }

        public bool LoadContentSub(Map map, WorldRenderer renderer, string basename, bool background)
        {
            Stream gat = ContentManager.Instance.Load<Stream>(@"data\" + basename + ".gat");
            Stream gnd = ContentManager.Instance.Load<Stream>(@"data\" + basename + ".gnd");
            Stream rsw = ContentManager.Instance.Load<Stream>(@"data\" + basename + ".rsw");

            if (gat == null || gnd == null || rsw == null)
                return false;

            if (!map.Load(gat, gnd, rsw, background))
                return false;

            gat.Close();
            gnd.Close();
            rsw.Close();

            renderer.LoadResources(background);

            return true;
        }
    }
}
