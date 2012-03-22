using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Content;

namespace FimbulwinterClient.IO.ContentLoaders
{
    public class RsmModelLoader : IContentLoader
    {
        public object LoadContent(ROContentManager rcm, System.IO.Stream s, string fn)
        {
            RsmModel rsm = new RsmModel(rcm.Game.GraphicsDevice, rcm);

            if (!rsm.Load(s))
                return null;

            return rsm;
        }
    }
}
