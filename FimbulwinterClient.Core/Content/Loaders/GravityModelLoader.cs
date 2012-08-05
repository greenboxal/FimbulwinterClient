using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FimbulwinterClient.Core.Assets;
using IrrlichtLime.IO;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class GravityModelLoader : IContentLoader
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
            /*GravityModel model = new GravityModel();

            if (!model.Load(stream))
                return false;

            stream.Close();

            return model;*/
            return null;
        }
    }
}
