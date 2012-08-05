using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IrrlichtLime.IO;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class StreamLoader : IContentLoader
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
            return stream;
        }
    }
}
