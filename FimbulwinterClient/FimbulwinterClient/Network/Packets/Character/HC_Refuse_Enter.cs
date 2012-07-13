using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets.Char
{
    [Method(methodId: (ushort)Enums.PacketHeader.HEADER_HC_REFUSE_ENTER,
        size: 3,
        name: "HC_REFUSE_ENTER",
        direction: MethodAttribute.packetdirection.pd_in)]
    public class HC_Refuse_Enter : InPacket
    {
        public byte Result { get; set; }

        public bool Read(byte[] data)
        {
            Result = data[0];

            return true;
        }
    }
}
