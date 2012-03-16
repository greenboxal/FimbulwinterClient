using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using FMOD;
using FimbulwinterClient.Config;
using System.IO;

namespace FimbulwinterClient.Audio
{
    public class BGMManager : GameComponent
    {
        private readonly FMOD.System system;
        public FMOD.System System
        {
            get { return system; }
        }

        private FMOD.Sound sound;
        private FMOD.Channel channel;
        private string currentSound;

        public BGMManager()
            : base(ROClient.Singleton)
        {
            uint version = 0;
            RESULT result = Factory.System_Create(ref system);

            if (result != RESULT.OK)
                throw new Exception("Create SoundSystem Failed");

            result = System.getVersion(ref version);

            if (result != RESULT.OK || version < VERSION.number)
                throw new Exception("Create SoundSystem Failed");

            result = System.init(32, INITFLAGS.NORMAL, (IntPtr)null);

            if (result != RESULT.OK)
                throw new Exception("Create SoundSystem Failed");

            ROClient.Singleton.Config.BgmVolumeChanged += new Action<float>(cfg_BgmVolumeChanged);
        }

        void cfg_BgmVolumeChanged(float vol)
        {
            if (channel != null)
                channel.setVolume(vol);
        }

        public void PlayBGM(string name)
        {
            string fname = string.Format("BGM/{0}.mp3", name);

            if (currentSound != fname && File.Exists(fname))
            {
                RESULT result = system.createSound(fname, MODE.HARDWARE, ref sound);

                if (result != RESULT.OK)
                    throw new Exception("Create Sound Failed");

                result = system.playSound(CHANNELINDEX.FREE, sound, false, ref channel);

                if (result != RESULT.OK)
                    throw new Exception("Play Sound Failed");

               channel.setVolume(ROClient.Singleton.Config.BgmVolume);
               currentSound = fname;
            }
        }
    }
}
