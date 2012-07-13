using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets.Char
{
    public class CH_Enter : OutPacket
    {
        private int aid, lig1, lig2;
        private byte sex;

        public CH_Enter(int aid, int lig1, int lig2, byte sex)
            : base(Convert.ToUInt16(Enums.PacketHeader.HEADER_CH_ENTER), 17)
        {
            this.aid = aid;
            this.lig1 = lig1;
            this.lig2 = lig2;
            this.sex = sex;
        }

        public override bool Write(System.IO.BinaryWriter bw)
        {
            base.Write(bw);

            bw.Write(aid);
            bw.Write(lig1);
            bw.Write(lig2);
            bw.Write((short)0);
            bw.Write(sex);
            bw.Flush();

            return true;
        }
    }
}
