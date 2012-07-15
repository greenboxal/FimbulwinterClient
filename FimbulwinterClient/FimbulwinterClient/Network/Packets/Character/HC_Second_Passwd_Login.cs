using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets.Char
{
    [PackerHandler(PacketHeader.HEADER_HC_SECOND_PASSWD_LOGIN,
        "HC_SECOND_PASSWD_LOGIN",
        12,
        PackerHandlerAttribute.PacketDirection.In)]
    public class HC_Second_Passwd_Login : InPacket
    {
        public bool Read(byte[] data)
        {
            return true;

        }
    }
}
