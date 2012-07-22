using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets
{
    public class Ping : OutPacket
    {
        private int time;

        public Ping(int ticks)
            : base(Convert.ToUInt16(PacketHeader.HEADER_PING), 6)
        {
            time = ticks;
        }

        public override bool Write(System.IO.BinaryWriter bw)
        {
            base.Write(bw);

            bw.Write(time);

            return true;
        }
    }
}
