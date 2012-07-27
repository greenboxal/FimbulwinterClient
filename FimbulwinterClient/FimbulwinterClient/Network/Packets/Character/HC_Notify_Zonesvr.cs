using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace FimbulwinterClient.Network.Packets.Character
{
    [PackerHandler(PacketHeader.HEADER_HC_NOTIFY_ZONESVR,
        "HC_NOTIFY_ZONESVR",
        28,
        PackerHandlerAttribute.PacketDirection.In)]
    public class HC_Notify_Zonesvr : InPacket
    {
        public int GID { get; set; }
        public string Mapname { get; set; }
        public IPAddress IP { get; set; }
        public int Port { get; set; }

        public bool Read(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader br = new BinaryReader(new MemoryStream(data)))
                {
                    GID = br.ReadInt32();
                    Mapname = br.ReadCString(16);
                    IP = new IPAddress(br.ReadBytes(4));
                    Port = br.ReadInt16();
                }
            }
            return true;
        }
    }
}
