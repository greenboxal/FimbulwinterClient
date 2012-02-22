using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace FimbulwinterClient.Audio
{
    public class EffectManager : GameComponent
    {
        ROConfig _cfg;

        public EffectManager(ROClient g, ROConfig cfg)
            : base(g)
        {
            _cfg = cfg;
        }

        public void PlayEffect(SoundEffect se)
        {
            SoundEffectInstance sei = se.CreateInstance();
            sei.Volume = _cfg.EffectVolume;
            sei.Play();
        }
    }
}
