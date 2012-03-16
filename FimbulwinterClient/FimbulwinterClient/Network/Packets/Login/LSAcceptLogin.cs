using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace FimbulwinterClient.Network.Packets.Login
{
    public struct CharServerInfo
    {
        public IPAddress IP { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public int Users { get; set; }
        public int Type { get; set; }
        public int New { get; set; }

        public override string ToString()
        {
            return Name + " (" + Users + ")";
        }
    }

    public class LSAcceptLogin : InPacket
    {
        public int LoginID1 { get; set; }
        public int AccountID { get; set; }
        public int LoginID2 { get; set; }
        public byte Sex { get; set; }
        public CharServerInfo[] Servers { get; set; }

        public override bool Read(byte[] data)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(data));

            int serverCount = (data.Length - 43) / 32;

            LoginID1 = br.ReadInt32();
            AccountID = br.ReadInt32();
            LoginID2 = br.ReadInt32();

            br.ReadBytes(30);
            
            Sex = br.ReadByte();

            Servers = new CharServerInfo[serverCount];
            for (int i = 0; i < serverCount; i++)
            {
                CharServerInfo csi = new CharServerInfo();

                csi.IP = new IPAddress(br.ReadBytes(4));
                csi.Port = br.ReadInt16();
                csi.Name = br.ReadCString(20);
                csi.Users = br.ReadInt16();
                csi.Type = br.ReadInt16();
                csi.New = br.ReadInt16();

                Servers[i] = csi;
            }

            return true;
        }
    }
}
