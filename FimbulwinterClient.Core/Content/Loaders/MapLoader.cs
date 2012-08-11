using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;

namespace FimbulwinterClient.Core.Content.Loaders
{
    class MapLoader : IContentLoader
    {
        public object LoadContent(ContentManager contentManager, string contentName, Stream stream)
        {
            string baseName = Path.GetFileNameWithoutExtension(contentName);

            Stream gat = ContentManager.Instance.Load<Stream>(@"data\" + baseName + ".gat");
            Stream gnd = ContentManager.Instance.Load<Stream>(@"data\" + baseName + ".gnd");
            Stream rsw = ContentManager.Instance.Load<Stream>(@"data\" + baseName + ".rsw");

            Map result = new Map();

            if (!result.Load(gat, gnd, rsw))
                return null;

            gat.Close();
            gnd.Close();
            rsw.Close();
            stream.Close();

            return result;
        }
    }
}
