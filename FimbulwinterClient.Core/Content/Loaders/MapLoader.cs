using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FimbulwinterClient.Core.Assets;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class MapLoader : IContentLoader
    {
        public object Load(Stream stream, string assetName)
        {
            Map map = new Map(SharedInformation.GraphicsDevice);
            Stream ground = SharedInformation.ContentManager.Load<Stream>(assetName.Replace(".gat", ".gnd"));
            Stream world = SharedInformation.ContentManager.Load<Stream>(assetName.Replace(".gat", ".rsw"));

            if (!map.Load(stream, ground, world))
                return null;

            stream.Close();

            return map;
        }
    }
}
