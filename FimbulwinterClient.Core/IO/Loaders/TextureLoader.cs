using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IrrlichtLime.Core;
using IrrlichtLime.Video;
using IrrlichtLime.IO;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class TextureLoader : IContentLoader
    {
        public LoadType Type
        {
            get { return LoadType.ReadFile; }
        }

        public object Load(ReadFile readFile, string assetName)
        {
            return SharedInformation.Graphics.AddTexture("", SharedInformation.Graphics.CreateImage(readFile));
        }

        public object Load(Stream stream, string assetName)
        {
            throw new NotSupportedException();
        }
    }
}
