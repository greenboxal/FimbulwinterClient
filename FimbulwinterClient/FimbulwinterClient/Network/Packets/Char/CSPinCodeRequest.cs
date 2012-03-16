using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets.Char
{
    public class CSPinCodeRequest : InPacket
    {
        public override bool Read(byte[] data)
        {
            return true;

        }
    }
}
