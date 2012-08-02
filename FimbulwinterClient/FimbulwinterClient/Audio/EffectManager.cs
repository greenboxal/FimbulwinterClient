using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using FimbulwinterClient.Core.Config;
using FimbulwinterClient.Core;

namespace FimbulwinterClient.Audio
{
    public class EffectManager : GameComponent
    {
        public EffectManager()
            : base(ROClient.Singleton)
        {

        }

        public void PlayEffect(SoundEffect se)
        {
            SoundEffectInstance sei = se.CreateInstance();
            sei.Volume = SharedInformation.Config.EffectVolume;
            sei.Play();
        }
    }
}
