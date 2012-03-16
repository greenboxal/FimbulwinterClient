using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace FimbulwinterClient.Config
{
    [Serializable]
    public class ServersInfo
    {
        private List<ServerInfo> m_servers;

        [XmlArray("Servers")]
        public List<ServerInfo> Servers
        {
            get { return m_servers; }
            set { m_servers = value; }
        }

        private string m_serviceType;
        public string ServiceType
        {
            get { return m_serviceType; }
            set { m_serviceType = value; }
        }

        private string m_serverType;
        public string ServerType
        {
            get { return m_serverType; }
            set { m_serverType = value; }
        }

        public ServersInfo()
        {
            m_servers = new List<ServerInfo>();
            m_serviceType = "";
            m_serverType = "";
        }

        public static ServersInfo FromStream(Stream s)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ServersInfo));

            return (ServersInfo)xs.Deserialize(s);
        }
    }
}
