﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets.Char
{
    public class Ping : OutPacket
    {
        private int time;

        public Ping(int ticks)
            : base(0x187, 6)
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
