using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Network.Packets.Login;
using FimbulwinterClient.Network.Packets.Char;
using FimbulwinterClient.Config;

namespace FimbulwinterClient
{
    public class NetworkState
    {
        ServerInfo selectedServer;
        public ServerInfo SelectedServer
        {
            get { return selectedServer; }
            set { selectedServer = value; }
        }

        LSAcceptLogin acceptedLogin;
        public LSAcceptLogin LoginAccept
        {
            get { return acceptedLogin; }
            set { acceptedLogin = value; }
        }

        CharServerInfo selectedCharServer;
        public CharServerInfo SelectedCharServer
        {
            get { return selectedCharServer; }
            set { selectedCharServer = value; }
        }

        CSAcceptLogin charAccept;
        public CSAcceptLogin CharAccept
        {
            get { return charAccept; }
            set { charAccept = value; }
        }
    }
}
