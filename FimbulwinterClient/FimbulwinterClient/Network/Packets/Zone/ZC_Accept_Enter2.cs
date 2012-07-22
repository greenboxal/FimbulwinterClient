using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FimbulwinterClient.Network.Packets.Zone
{
    [PackerHandler(PacketHeader.HEADER_ZC_ACCEPT_ENTER2,
           "ZC_ACCEPT_ENTER2",
           42,
           PackerHandlerAttribute.PacketDirection.In)]
    public class ZC_Accept_Enter2 : InPacket
    {

        public bool Read(byte[] data)
        {
            using (BinaryReader br = new BinaryReader(new MemoryStream(data)))
            {
            }
            return true;
        }
    }
}
