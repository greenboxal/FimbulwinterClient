using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FimbulwinterClient.Content;

namespace FimbulwinterClient.IO.ContentLoaders
{
    public class MapLoader : IContentLoader
    {
        public object LoadContent(ROContentManager rcm, Stream s, string fn)
        {
            Map map = new Map(rcm.Game.GraphicsDevice);

            if (!map.Load(s, rcm.LoadContent<Stream>(fn.Replace(".gat", ".gnd")), rcm.LoadContent<Stream>(fn.Replace(".gat", ".rsw"))))
                return null;

            return map;
        }
    }
}
