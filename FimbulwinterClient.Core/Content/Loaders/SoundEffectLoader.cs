using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class SoundEffectLoader : IContentLoader
    {
        public object Load(Stream stream, string assetName)
        {
            SoundEffect se = SoundEffect.FromStream(stream);

            stream.Close();

            return se;
        }
    }
}
