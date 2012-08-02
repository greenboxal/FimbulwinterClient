using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class StringLoader : IContentLoader
    {
        public object Load(Stream stream, string assetName)
        {
            string str = new StreamReader(stream).ReadToEnd();

            stream.Close();

            return str;
        }
    }
}
