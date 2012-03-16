using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace FimbulwinterClient.Config
{
    [Serializable]
    public class Configuration
    {
        public const int MaxCharacters = 9;

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

        private int m_screenWidth;
        public int ScreenWidth
        {
            get { return m_screenWidth; }
            set { m_screenWidth = value; }
        }

        private int m_screenHeight;
        public int ScreenHeight
        {
            get { return m_screenHeight; }
            set { m_screenHeight = value; }
        }

        private string m_lastLogin;
        public string LastLogin
        {
            get { return m_lastLogin; }
            set { m_lastLogin = value; }
        }

        private bool m_saveLast;
        public bool SaveLast
        {
            get { return m_saveLast; }
            set { m_saveLast = value; }
        }

        private ServersInfo m_serversInfo;
        [XmlIgnore]
        public ServersInfo ServersInfo
        {
            get { return m_serversInfo; }
            set { m_serversInfo = value; }
        }

        private ROClient m_client;
        [XmlIgnore]
        public ROClient Client
        {
            get { return m_client; }
            set { m_client = value; }
        }

        public event Action<float> BgmVolumeChanged;
        public event Action<float> EffectVolumeChanged;

        public Configuration()
        {
            m_bgmVolume = 1.0f;
            m_effectVolume = 1.0f;

            m_screenWidth = 1280;
            m_screenHeight = 768;

            m_saveLast = false;
            m_lastLogin = "";
        }

        public Configuration(ROClient cl)
            : this()
        {
            m_client = cl;
        }

        public static Configuration FromStream(Stream s)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Configuration));

            return (Configuration)xs.Deserialize(s);
        }

        public void ReadConfig()
        {
            m_client.ContentManager.FileSystem.LoadGrf(@"D:\Games\Ragnarök\data.grf");

            using (Stream s = ROClient.Singleton.ContentManager.LoadContent<Stream>("data/fb/config/serverinfo.xml"))
            {
                m_serversInfo = ServersInfo.FromStream(s);
                s.Close();
            }
        }

        public void Save()
        {
            XmlSerializer xs = new XmlSerializer(typeof(Configuration));

            xs.Serialize(new FileStream("data/fb/config/config.xml", FileMode.Create), this);
        }
    }
}
