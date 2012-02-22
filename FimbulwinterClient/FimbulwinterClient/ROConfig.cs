using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient
{
    public class ROConfig
    {
        private float m_bgmVolume;
        public float BgmVolume
        {
            get { return m_bgmVolume; }
            set
            {
                if (m_bgmVolume != value && BgmVolumeChanged != null)
                    BgmVolumeChanged(value);

                m_bgmVolume = value; 
            }
        }

        private float m_effectVolume;
        public float EffectVolume
        {
            get { return m_effectVolume; }
            set 
            {
                if (m_effectVolume != value && EffectVolumeChanged != null)
                    EffectVolumeChanged(value);

                m_effectVolume = value; 
            }
        }

        public event Action<float> BgmVolumeChanged;
        public event Action<float> EffectVolumeChanged;

        public ROConfig()
        {
            m_bgmVolume = 1.0f;
            m_effectVolume = 1.0f;
        }
    }
}
