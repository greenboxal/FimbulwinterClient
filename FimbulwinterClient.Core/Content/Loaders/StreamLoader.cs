using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class StreamLoader : IContentLoader
    {
        public object Load(Stream stream, string assetName)
        {
            return stream;
        }
    }
}
