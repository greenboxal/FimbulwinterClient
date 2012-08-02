using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using FimbulwinterClient.Core;
using FimbulwinterClient.Core.Content;

namespace FimbulwinterClient.Core.Config
{
    [Serializable]
    public class Configuration
    {
        private float _bgmVolume;
        public float BgmVolume
        {
            get { return _bgmVolume; }
            set
            {
                if (_bgmVolume != value && BgmVolumeChanged != null)
                    BgmVolumeChanged(value);

                _bgmVolume = value; 
            }
        }

        private float _effectVolume;
        public float EffectVolume
        {
            get { return _effectVolume; }
            set 
            {
                if (_effectVolume != value && EffectVolumeChanged != null)
                    EffectVolumeChanged(value);

                _effectVolume = value; 
            }
        }

        private int _screenWidth;
        public int ScreenWidth
        {
            get { return _screenWidth; }
            set { _screenWidth = value; }
        }

        private int _screenHeight;
        public int ScreenHeight
        {
            get { return _screenHeight; }
            set { _screenHeight = value; }
        }

        private string _lastLogin;
        public string LastLogin
        {
            get { return _lastLogin; }
            set { _lastLogin = value; }
        }

        private bool _saveLast;
        public bool SaveLast
        {
            get { return _saveLast; }
            set { _saveLast = value; }
        }

        private ServersInfo _serversInfo;
        [XmlIgnore]
        public ServersInfo ServersInfo
        {
            get { return _serversInfo; }
            set { _serversInfo = value; }
        }

        public event Action<float> BgmVolumeChanged;
        public event Action<float> EffectVolumeChanged;

        public Configuration()
        {
            _bgmVolume = 1.0f;
            _effectVolume = 1.0f;

            _screenWidth = 1280;
            _screenHeight = 768;

            _saveLast = false;
            _lastLogin = "";
        }

        public static Configuration FromStream(Stream s)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Configuration));

            return (Configuration)xs.Deserialize(s);
        }

        public void ReadConfig()
        {
            GrfFileSystem.AddGrf(@"rdata.grf");
            GrfFileSystem.AddGrf(@"data.grf");

            using (Stream s = SharedInformation.ContentManager.Load<Stream>(@"data\fb\config\serverinfo.xml"))
            {
                _serversInfo = ServersInfo.FromStream(s);
                s.Close();
            }
        }

        public void Save()
        {
            XmlSerializer xs = new XmlSerializer(typeof(Configuration));

            xs.Serialize(new FileStream(@"data\fb\config\config.xml", FileMode.Create), this);
        }
    }
}
