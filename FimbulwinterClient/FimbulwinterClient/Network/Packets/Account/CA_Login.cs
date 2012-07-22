using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Extensions;

namespace FimbulwinterClient.Network.Packets.Account
{
    public class CA_Login : OutPacket
    {
        private string login;
        private string pw;
        private int version;
        private int type;

        public CA_Login(string login, string pw, int version, int servertype)
            : base(Convert.ToUInt16(PacketHeader.HEADER_CA_LOGIN), 55)
        {
            this.login = login;
            this.pw = pw;
            this.version = version;
            this.type = servertype;
        }

        public override bool Write(BinaryWriter bw)
        {
            base.Write(bw);

            bw.Write((int)version);
            bw.WriteCString(login, 24);
            bw.WriteCString(pw, 24);
            bw.Write((byte)type);
            bw.Flush();

            return true;
        }
    }
}
