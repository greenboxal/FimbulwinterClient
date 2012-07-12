using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient
{
    public static class ROConst
    {
        public static readonly string Humans = "인간족";
        public static readonly string Robes = "로브";
        public static readonly string Accessories = "악세사리";
        public static readonly string Monster = "몬스터";

        public static readonly string Body = "몸통"; // 몸통
        public static readonly string Head = "머리통"; // 머리통
        public static readonly string[] Sex = new string[] { "여", "남" }; // 여, 남
        
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

        public static readonly string genetic = "제네릭";

        public static readonly Dictionary<int, string> ClassSprites;
        public static readonly Dictionary<int, string> ClassNames;

        static ROConst()
        {
            ClassSprites = new Dictionary<int, string>();
            ClassSprites.Add(0, novice);
            ClassSprites.Add(1, swordsman);
            ClassSprites.Add(2, magician);
            ClassSprites.Add(3, archer);
            ClassSprites.Add(4, acolyte);
            ClassSprites.Add(5, merchant);
            ClassSprites.Add(6, thief);
            ClassSprites.Add(7, knight);
            ClassSprites.Add(8, priest);
            ClassSprites.Add(9, wizard);
            ClassSprites.Add(10, blacksmith);
            ClassSprites.Add(11, hunter);
            ClassSprites.Add(12, assassin);
            ClassSprites.Add(13, knight_mounted);
            ClassSprites.Add(14, crusader);
            ClassSprites.Add(15, monk);
            ClassSprites.Add(16, sage);
            ClassSprites.Add(17, rogue);
            ClassSprites.Add(18, alchemist);
            ClassSprites.Add(19, bard);
            ClassSprites.Add(20, dancer);
            ClassSprites.Add(21, gm);
            ClassSprites.Add(22, mercenary);
            ClassSprites.Add(4070, genetic);

            ClassNames = new Dictionary<int, string>();
            ClassNames.Add(4070, "Genetic");
        }
    }
}
