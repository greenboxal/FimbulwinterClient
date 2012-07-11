using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FimbulwinterClient.Network.Packets.Char
{
    public struct CSCharData
    {
        public int CharID;
        public int BaseExp;
        public int Zeny;
        public int JobExp;
        public int JobLevel;
        public int Opt1;
        public int Opt2;
        public int Option;
        public int Karma;
        public int Manner;
        public short StatusPoint;
        public int HP;
        public int MaxHP;
        public short SP;
        public short MaxSP;
        public short Speed;
        public short Class;
        public short Hair;
        public short Weapon;
        public short BaseLevel;
        public short SkillPoint;
        public short HeadLow;
        public short Shield;
        public short HeadTop;
        public short HeadMid;
        public short HairColor;
        public short ClothesColor;
        public string Name;
        public byte Str;
        public byte Agi;
        public byte Vit;
        public byte Int;
        public byte Dex;
        public byte Luk;
        public short Slot;
        public short Rename;
        public string MapName;
        public int DeleteDate;
        public int Robe;
    }

    public class CSAcceptLogin : InPacket
    {
        public int MaxSlots { get; set; }
        public int AvailableSlots { get; set; }
        public int PremiumSlots { get; set; }
        public CSCharData[] Chars { get; set; }

        public override bool Read(byte[] data)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(data));
            int numChars = (data.Length - 23) / 144;

            MaxSlots = br.ReadByte();
            AvailableSlots = br.ReadByte();
            PremiumSlots = br.ReadByte();

            br.ReadBytes(20);

            Chars = new CSCharData[numChars];
            for (int i = 0; i < numChars; i++)
            {
                CSCharData cd = new CSCharData();
                
                cd.CharID = br.ReadInt32();
                cd.BaseExp = br.ReadInt32();
                cd.Zeny = br.ReadInt32();
                cd.JobExp = br.ReadInt32();
                cd.JobLevel = br.ReadInt32();
                cd.Opt1 = br.ReadInt32();
                cd.Opt2 = br.ReadInt32();
                cd.Option = br.ReadInt32();
                cd.Karma = br.ReadInt32();
                cd.Manner = br.ReadInt32();
                cd.StatusPoint = br.ReadInt16();
                cd.HP = br.ReadInt32();
                cd.MaxHP = br.ReadInt32();
                cd.SP = br.ReadInt16();
                cd.MaxSP = br.ReadInt16();
                cd.Speed = br.ReadInt16();
                cd.Class = br.ReadInt16();
                cd.Hair = br.ReadInt16();
                cd.Weapon = br.ReadInt16();
                cd.BaseLevel = br.ReadInt16();
                cd.SkillPoint = br.ReadInt16();
                cd.HeadLow = br.ReadInt16();
                cd.Shield = br.ReadInt16();
                cd.HeadTop = br.ReadInt16();
                cd.HeadMid = br.ReadInt16();
                cd.HairColor= br.ReadInt16();
                cd.ClothesColor = br.ReadInt16();
                cd.Name = br.ReadCString(23);
                cd.Str = br.ReadByte();
                cd.Agi = br.ReadByte();
                cd.Vit = br.ReadByte();
                cd.Int = br.ReadByte();
                cd.Dex = br.ReadByte();
                cd.Luk = br.ReadByte();
                cd.Slot = br.ReadInt16();
                cd.Rename = br.ReadInt16();
                cd.MapName = br.ReadCString(16);
                cd.DeleteDate = br.ReadInt32();
                cd.Robe = br.ReadInt32();

                Chars[i] = cd;
            }

            return true;
        }
    }
}
