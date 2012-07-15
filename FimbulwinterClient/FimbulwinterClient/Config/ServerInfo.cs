using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Config
{
    [Serializable]
    public class ServerInfo
    {
        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        private int _port;
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        private int _version;
        public int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private string _registrationUrl;
        public string RegistrationUrl
        {
            get { return _registrationUrl; }
            set { _registrationUrl = value; }
        }

        public override string ToString()
        {
            return _displayName;
        }
    }
}
