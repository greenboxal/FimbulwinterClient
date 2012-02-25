using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace FimbulwinterClient
{
    public struct ServerInfo
    {
        private string m_display;
        public string Display
        {
            get { return m_display; }
            set { m_display = value; }
        }

        private string m_desc;
        public string Desc
        {
            get { return m_desc; }
            set { m_desc = value; }
        }

        private string m_address;
        public string Address
        {
            get { return m_address; }
            set { m_address = value; }
        }

        private int m_port;
        public int Port
        {
            get { return m_port; }
            set { m_port = value; }
        }

        private int m_version;
        public int Version
        {
            get { return m_version; }
            set { m_version = value; }
        }

        private string m_registrationUrl;
        public string RegistrationUrl
        {
            get { return m_registrationUrl; }
            set { m_registrationUrl = value; }
        }

        public override string ToString()
        {
            return m_display;
        }
    }

    [Serializable]
    public class ROConfig
    {
        public const int RO_MAXCHARS = 9;

        private ROClient m_client;
        [XmlIgnore]
        public ROClient Client
        {
            get { return m_client; }
            set { m_client = value; }
        }

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

        private string m_serviceType;
        [XmlIgnore]
        public string ServiceType
        {
            get { return m_serviceType; }
        }

        private string m_serverType;
        [XmlIgnore]
        public string ServerType
        {
            get { return m_serverType; }
        }

        private List<ServerInfo> m_servers;
        [XmlIgnore]
        public List<ServerInfo> Servers
        {
            get { return m_servers; }
            set { m_servers = value; }
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

        public event Action<float> BgmVolumeChanged;
        public event Action<float> EffectVolumeChanged;

        public ROConfig()
        {
            m_bgmVolume = 1.0f;
            m_effectVolume = 1.0f;

            m_servers = new List<ServerInfo>();

            m_screenWidth = 1280;
            m_screenHeight = 768;

            m_saveLast = false;
            m_lastLogin = "";
        }

        public ROConfig(ROClient cl)
            : this()
        {
            m_client = cl;
        }

        public static ROConfig FromStream(Stream s)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ROConfig));

            return (ROConfig)xs.Deserialize(s);
        }

        public void ReadConfig()
        {
            m_client.ContentManager.FileSystem.LoadGrf("data.grf");

            ReadClientInfo();
        }

        private void ReadClientInfo()
        {
            XDocument xml = XDocument.Load(m_client.ContentManager.LoadContent<Stream>("data/fb/clientinfo.xml"));
            
            XElement clientinfo = xml.Element("clientinfo");

            m_serviceType = clientinfo.Element("servicetype").Value;
            m_serverType = clientinfo.Element("servertype").Value;

            var connections = from connection in clientinfo.Elements("connection")
                              select new ServerInfo
                              {
                                  Display = connection.Element("display").Value,
                                  Desc = connection.Element("desc").Value,
                                  Address = connection.Element("address").Value,
                                  Port = int.Parse(connection.Element("port").Value),
                                  Version = int.Parse(connection.Element("version").Value),
                                  RegistrationUrl = connection.Element("version").Value
                              };

            foreach (var connection in connections)
	            m_servers.Add(connection);
        }

        public void Save()
        {
            XmlSerializer xs = new XmlSerializer(typeof(ROConfig));

            xs.Serialize(new FileStream("data/fb/config.xml", FileMode.Create), this);
        }
    }
}
