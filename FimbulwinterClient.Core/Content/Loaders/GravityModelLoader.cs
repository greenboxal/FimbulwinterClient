using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FimbulwinterClient.Core.Assets;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class GravityModelLoader : IContentLoader
    {
        public object Load(Stream stream, string assetName)
        {
            GravityModel model = new GravityModel();

            if (!model.Load(stream))
                return false;

            stream.Close();

            return model;
        }
    }
}
