using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using FimbulvetrEngine.IO;
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

            Dispatcher.Instance.DispatchTask(o =>
            {
                Stream gat = FileSystemManager.Instance.OpenStream(@"data\" + baseName + ".gat");
                Stream gnd = FileSystemManager.Instance.OpenStream(@"data\" + baseName + ".gnd");
                Stream rsw = FileSystemManager.Instance.OpenStream(@"data\" + baseName + ".rsw");

                if (gat == null || gnd == null || rsw == null)
                    return;

                if (!map.Load(gat, gnd, rsw, background))
                    return;

                gat.Close();
                gnd.Close();
                rsw.Close();

                result.LoadResources(background);
            }, background);

            return result;
        }
    }
}
