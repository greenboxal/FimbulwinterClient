using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets.Character
{
    public class CH_Select_Char : OutPacket
    {
        private int _num;

        public CH_Select_Char(int num)
            : base(Convert.ToUInt16(PacketHeader.HEADER_CH_SELECT_CHAR), 3)
        {
            _num = num;
        }

        public override bool Write(System.IO.BinaryWriter bw)
        {
            base.Write(bw);

            bw.Write((byte)_num);
            bw.Flush();

            return true;
        }
    }
}
