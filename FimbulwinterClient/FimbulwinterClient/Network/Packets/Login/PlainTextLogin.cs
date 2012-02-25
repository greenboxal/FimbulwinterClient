using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FimbulwinterClient.Utils;

namespace FimbulwinterClient.Network.Packets.Login
{
    public class PlainTextLogin : OutPacket
    {
        private string login;
        private string pw;
        private int version;
        private int type;

        public PlainTextLogin(string login, string pw, int version, int servertype)
            : base(0x64, 55)
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
