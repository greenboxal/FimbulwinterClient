using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FimbulwinterClient.Core.Assets;
using IrrlichtLime.IO;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class MapLoader : IContentLoader
    {
        public LoadType Type
        {
            get { return LoadType.Stream; }
        }

        public object Load(ReadFile readFile, string assetName)
        {
            throw new NotSupportedException();
        }

        public object Load(Stream stream, string assetName)
        {
            Map map = new Map();
            Stream ground = SharedInformation.ContentManager.Load<Stream>(assetName.Replace(".gat", ".gnd"));
            Stream world = SharedInformation.ContentManager.Load<Stream>(assetName.Replace(".gat", ".rsw"));

            if (!map.Load(stream, ground, world))
                return null;

            stream.Close();

            return map;
        }
    }
}
