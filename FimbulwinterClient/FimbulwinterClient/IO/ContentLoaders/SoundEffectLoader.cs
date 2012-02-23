using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace FimbulwinterClient.IO.ContentLoaders
{
    public class SoundEffectLoader : IContentLoader
    {
        public object LoadContent(ROContentManager rcm, Stream s, string fn)
        {
            return SoundEffect.FromStream(s);
        }
    }
}
