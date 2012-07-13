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
        private List<ServerInfo> _servers;

        [XmlArray("Servers")]
        public List<ServerInfo> Servers
        {
            get { return _servers; }
            set { _servers = value; }
        }

        private string _serviceType;
        public string ServiceType
        {
            get { return _serviceType; }
            set { _serviceType = value; }
        }

        private string _serverType;
        public string ServerType
        {
            get { return _serverType; }
            set { _serverType = value; }
        }

        public ServersInfo()
        {
            _servers = new List<ServerInfo>();
            _serviceType = "";
            _serverType = "";
        }

        public static ServersInfo FromStream(Stream s)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ServersInfo));

            return (ServersInfo)xs.Deserialize(s);
        }
    }
}
