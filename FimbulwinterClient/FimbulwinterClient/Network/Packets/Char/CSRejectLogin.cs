using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets.Char
{
    public class CSRejectLogin : InPacket
    {
        public byte Result { get; set; }

        public override bool Read(byte[] data)
        {
            Result = data[0];

            return true;
        }
    }
}
