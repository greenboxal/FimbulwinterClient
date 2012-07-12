using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient
{
    public static class ROConst
    {
        public static readonly string Humans = "ÀÎ°£Á·"; // 인간족
        public static readonly string Accessories = "¾Ç¼¼»ç¸®"; // 악세사리
        public static readonly string Monster = "¸ó½ºÅÍ"; // 몬스터

        public static readonly string Body = "¸öÅë"; // 몸통
        public static readonly string Head = "¸Ó¸®Åë"; // 머리통
        public static readonly string[] Sex = new string[] { "¿©", "³²" }; // 여, 남
        
        public static readonly string novice = "\xC3\xCA\xBA\xB8\xC0\xDA";
        public static readonly string swordsman = "\xB0\xCB\xBB\xE7";
        public static readonly string magician = "\xB8\xB6\xB9\xFD\xBB\xE7";
        public static readonly string archer = "\xB1\xC3\xBC\xF6";
        public static readonly string acolyte = "\xBC\xBA\xC1\xF7\x9F\xE0";
        public static readonly string merchant = "\xBB\xF3\xC0\xCE";
        public static readonly string thief = "\xB5\xB5\xB5\xCF";
        public static readonly string knight = "\xB1\xE2\xBB\xE7";
        public static readonly string priest = "\xC7\xC1\xB8\xAE\xBD\xBA\xC6\xAE";
        public static readonly string wizard = "\xC0\xA7\xC0\xFA\xB5\xE5";
        public static readonly string blacksmith = "\xC1\xA6\xC3\xB6";
        public static readonly string hunter = "\xC7\xE5\xC5\xCD";
        public static readonly string assassin = "\xBE\xEE\xBC\xBC\xBD\xC5";
        public static readonly string knight_mounted = "\xC6\xE4\xC4\xDA\xC6\xE4\xC4\xDA\x5F\xB1\xE2\xBB\xE7";
        public static readonly string crusader = "\xC5\xA9\xB7\xE7\xBC\xBC\xC0\xCC\xB4\xF5";
        public static readonly string monk = "\xB8\xF9\xC5\xA9";
        public static readonly string sage = "\xBC\xBC\xC0\xCC\xC1\xF6";
        public static readonly string rogue = "\xB7\xCE\xB1\xD7";
        public static readonly string alchemist = "\xBF\xAC\xB1\xDD\xBC\xFA\xBB\xE7";
        public static readonly string bard = "\xB9\xD9\xB5\xE5";
        public static readonly string dancer = "\xB9\xAB\xC8\xF1";
        public static readonly string crusader_mounted = "\xBD\xC5\xC6\xE4\xC4\xDA\xC5\xA9\xB7\xE7\xBC\xBC\xC0\xCC\xB4\xF5";
        public static readonly string gm = "\x9D\xC4\xBF\xB5\xC0\xDA";
        public static readonly string mercenary = "\xBF\xEB\xBA\xB4";

        public static readonly Dictionary<int, string> ClassNames;

        static ROConst()
        {
            ClassNames = new Dictionary<int, string>();
            ClassNames.Add(0, novice);
            ClassNames.Add(1, swordsman);
            ClassNames.Add(2, magician);
            ClassNames.Add(3, archer);
            ClassNames.Add(4, acolyte);
            ClassNames.Add(5, merchant);
            ClassNames.Add(6, thief);
            ClassNames.Add(7, knight);
            ClassNames.Add(8, priest);
            ClassNames.Add(9, wizard);
            ClassNames.Add(10, blacksmith);
            ClassNames.Add(11, hunter);
            ClassNames.Add(12, assassin);
            ClassNames.Add(13, knight_mounted);
            ClassNames.Add(14, crusader);
            ClassNames.Add(15, monk);
            ClassNames.Add(16, sage);
            ClassNames.Add(17, rogue);
            ClassNames.Add(18, alchemist);
            ClassNames.Add(19, bard);
            ClassNames.Add(20, dancer);
            ClassNames.Add(21, gm);
            ClassNames.Add(22, mercenary);
        }
    }
}
