using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets.Char
{

    [Method(methodId: (ushort)Enums.PacketHeader.HEADER_HC_SECOND_PASSWD_LOGIN,
        size: 12,
        name: "HC_SECOND_PASSWD_LOGIN",
        direction: MethodAttribute.packetdirection.pd_in)]
    public class HC_Second_Passwd_Login : InPacket
    {
        public bool Read(byte[] data)
        {
            return true;

        }
    }
}
