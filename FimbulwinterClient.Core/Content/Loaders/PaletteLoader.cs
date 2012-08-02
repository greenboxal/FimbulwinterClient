using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulwinterClient.Core.Assets;

namespace FimbulwinterClient.Core.Content.Loaders
{
    class PaletteLoader : IContentLoader
    {
        public object Load(Stream stream, string assetName)
        {
            Palette pal = new Palette();

            if (!pal.Load(stream))
                return null;

            stream.Close();

            return pal;
        }
    }
}
