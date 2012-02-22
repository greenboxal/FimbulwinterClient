using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using FMOD;

namespace FimbulwinterClient.Audio
{
    public class BGMManager : GameComponent
    {
        private ROConfig _cfg;

        private readonly FMOD.System _system;
        public FMOD.System System
        {
            get { return _system; }
        }

        private FMOD.Sound _sound;
        private FMOD.Channel _channel;

        public BGMManager(ROClient g, ROConfig cfg)
            : base(g)
        {
            RESULT result = Factory.System_Create(ref _system);
            if (result != RESULT.OK)
                throw new Exception("Create SoundSystem Failed");

            uint version = 0;
            result = System.getVersion(ref version);
            if (result != RESULT.OK || version < VERSION.number)
                throw new Exception("Create SoundSystem Failed");

            result = System.init(32, INITFLAGS.NORMAL, (IntPtr)null);
            if (result != RESULT.OK)
                throw new Exception("Create SoundSystem Failed");

            _cfg = cfg;
            cfg.BgmVolumeChanged += new Action<float>(cfg_BgmVolumeChanged);
        }

        void cfg_BgmVolumeChanged(float obj)
        {
            if (_channel != null)
                _channel.setVolume(obj);
        }

        public void PlayBGM(string name)
        {
            RESULT result = _system.createSound(string.Format("BGM/{0}.mp3", name), MODE.HARDWARE, ref _sound);
            if (result != RESULT.OK)
                throw new Exception("Create Sound Failed");

            result = _system.playSound(CHANNELINDEX.FREE, _sound, false, ref _channel);
            if (result != RESULT.OK)
                throw new Exception("Play Sound Failed");
            _channel.setVolume(_cfg.BgmVolume);
        }
    }
}
