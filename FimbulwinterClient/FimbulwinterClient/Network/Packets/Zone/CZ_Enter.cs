using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets.Zone
{
    public class CZ_Enter : OutPacket
    {
        private int aid, gid, auth;
        private byte sex;

        public CZ_Enter(int aid, int gid, int auth, byte sex)
            : base(Convert.ToUInt16(PacketHeader.HEADER_CZ_ENTER), 13)
        {
            this.aid = aid;
            this.gid = gid;
            this.auth = auth;
            this.sex = sex;
        }

        public override bool Write(System.IO.BinaryWriter bw)
        {
            base.Write(bw);

            bw.Write(aid);
            bw.Write(gid);
            bw.Write(auth);
            bw.Write(0);
            bw.Write(sex);
            bw.Flush();

            return true;
        }
    }
}
