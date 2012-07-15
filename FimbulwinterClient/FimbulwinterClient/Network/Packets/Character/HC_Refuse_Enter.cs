using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets.Char
{
    [PackerHandler(PacketHeader.HEADER_HC_REFUSE_ENTER,
        "HC_REFUSE_ENTER",
        3,
        PackerHandlerAttribute.PacketDirection.In)]
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
