using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets.Char
{
    public class CSLoginPacket : OutPacket
    {
        private int aid, lig1, lig2;
        private byte sex;

        public CSLoginPacket(int aid, int lig1, int lig2, byte sex)
            : base(0x65, 17)
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
