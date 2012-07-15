using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text;
using FimbulwinterClient.Content;

namespace FimbulwinterClient.IO.ContentLoaders
{
    class PaletteLoader : IContentLoader
    {
        public object LoadContent(ROContentManager rcm, System.IO.Stream s, string fn)
        {
            Palette pal = new Palette();

            if (!pal.Load(s))
                return null;

            return pal;
        }
    }
}
