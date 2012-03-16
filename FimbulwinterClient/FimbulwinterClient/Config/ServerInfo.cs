using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Config
{
    [Serializable]
    public class ServerInfo
    {
        private string m_displayName;
        public string DisplayName
        {
            get { return m_displayName; }
            set { m_displayName = value; }
        }

        private string m_description;
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
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
            return m_displayName;
        }
    }
}
