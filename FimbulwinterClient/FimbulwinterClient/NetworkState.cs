using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Network.Packets.Account;
using FimbulwinterClient.Network.Packets.Character;
using FimbulwinterClient.Core.Config;

namespace FimbulwinterClient
{
    public class NetworkState
    {
        ServerInfo _selectedServer;
        public ServerInfo SelectedServer
        {
            get { return _selectedServer; }
            set { _selectedServer = value; }
        }

        AC_Accept_Login _acceptedLogin;
        public AC_Accept_Login LoginAccept
        {
            get { return _acceptedLogin; }
            set { _acceptedLogin = value; }
        }

        CharServerInfo _selectedCharServer;
        public CharServerInfo SelectedCharServer
        {
            get { return _selectedCharServer; }
            set { _selectedCharServer = value; }
        }

        HC_Accept_Enter _charAccept;
        public HC_Accept_Enter CharAccept
        {
            get { return _charAccept; }
            set { _charAccept = value; }
        }

        CSCharData _selectedChar;
        public CSCharData SelectedChar
        {
            get { return _selectedChar; }
            set { _selectedChar = value; }
        }
    }
}
