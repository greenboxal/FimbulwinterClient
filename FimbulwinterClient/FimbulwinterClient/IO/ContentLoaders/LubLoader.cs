using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FimbulwinterClient.Content;

namespace FimbulwinterClient.IO.ContentLoaders
{
    public class LubLoader : IContentLoader
    {
        public object LoadContent(ROContentManager rcm, Stream s, string fn)
        {
            Lub lub = new Lub();

            if (!lub.Load(s))
                return null;

            return lub;
        }
    }
}
