using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient
{
    public static class ROConst
    {
        public static readonly string Humans      = "인간족";
        public static readonly string Robes       = "로브";
        public static readonly string Accessories = "악세사리";
        public static readonly string Monster     = "몬스터";

        public static readonly string Body = "몸통";
        public static readonly string Head = "머리통";
        public static readonly string[] Sex = new string[] { "여", "남" };
        
        public static readonly string class_novice     = "초보자";
        public static readonly string class_swordsman  = "초보자";
        public static readonly string class_magician   = "초보자";
        public static readonly string class_archer     = "초보자";
        public static readonly string class_acolyte    = "초보자";
        public static readonly string class_merchant   = "초보자";
        public static readonly string class_thief      = "초보자";
        public static readonly string class_knight     = "초보자";
        public static readonly string class_priest     = "초보자";
        public static readonly string class_wizard     = "초보자";
        public static readonly string class_blacksmith = "초보자";
        public static readonly string class_hunter     = "초보자";
        public static readonly string class_assassin   = "초보자";
        public static readonly string class_chicken    = "초보자";
        public static readonly string class_crusader   = "초보자";
        public static readonly string class_monk       = "초보자";
        public static readonly string class_sage       = "초보자";
        public static readonly string class_rogue      = "초보자";
        public static readonly string class_alchemist  = "초보자";
        public static readonly string class_bard       = "초보자";
        public static readonly string class_dancer     = "초보자";
        public static readonly string class_genetic    = "제네릭";

        public static readonly Dictionary<int, string> ClassSprites;
        public static readonly Dictionary<int, string> ClassNames;

        static ROConst()
        {
            ClassSprites = new Dictionary<int, string>();
            ClassSprites.Add((int)Enums.Jobtbl.JT_NOVICE, class_novice);
            ClassSprites.Add((int)Enums.Jobtbl.JT_SWORDMAN, class_swordsman);
            ClassSprites.Add((int)Enums.Jobtbl.JT_MAGICIAN, class_magician);
            ClassSprites.Add((int)Enums.Jobtbl.JT_ARCHER, class_archer);
            ClassSprites.Add((int)Enums.Jobtbl.JT_ACOLYTE, class_acolyte);
            ClassSprites.Add((int)Enums.Jobtbl.JT_MERCHANT, class_merchant);
            ClassSprites.Add((int)Enums.Jobtbl.JT_THIEF, class_thief);
            ClassSprites.Add((int)Enums.Jobtbl.JT_KNIGHT, class_knight);
            ClassSprites.Add((int)Enums.Jobtbl.JT_PRIEST, class_priest);
            ClassSprites.Add((int)Enums.Jobtbl.JT_WIZARD, class_wizard);
            ClassSprites.Add((int)Enums.Jobtbl.JT_BLACKSMITH, class_blacksmith);
            ClassSprites.Add((int)Enums.Jobtbl.JT_HUNTER, class_hunter);
            ClassSprites.Add((int)Enums.Jobtbl.JT_ASSASSIN, class_assassin);
            ClassSprites.Add((int)Enums.Jobtbl.JT_CHICKEN, class_chicken);
            ClassSprites.Add((int)Enums.Jobtbl.JT_CRUSADER, class_crusader);
            ClassSprites.Add((int)Enums.Jobtbl.JT_MONK, class_monk);
            ClassSprites.Add((int)Enums.Jobtbl.JT_SAGE, class_sage);
            ClassSprites.Add((int)Enums.Jobtbl.JT_ROGUE, class_rogue);
            ClassSprites.Add((int)Enums.Jobtbl.JT_ALCHEMIST, class_alchemist);
            ClassSprites.Add((int)Enums.Jobtbl.JT_BARD, class_bard);
            ClassSprites.Add((int)Enums.Jobtbl.JT_DANCER, class_dancer);
            ClassSprites.Add((int)Enums.Jobtbl.JT_GENETIC, class_genetic);

            ClassNames = new Dictionary<int, string>();
            ClassNames.Add((int)Enums.Jobtbl.JT_NOVICE, "Novice");
            ClassNames.Add((int)Enums.Jobtbl.JT_SWORDMAN, "Swordman");
            ClassNames.Add((int)Enums.Jobtbl.JT_MAGICIAN, "Magician");
            ClassNames.Add((int)Enums.Jobtbl.JT_ARCHER, "Archer");
            ClassNames.Add((int)Enums.Jobtbl.JT_ACOLYTE, "Acolyte");
            ClassNames.Add((int)Enums.Jobtbl.JT_MERCHANT, "Merchant");
            ClassNames.Add((int)Enums.Jobtbl.JT_THIEF, "Thief");
            ClassNames.Add((int)Enums.Jobtbl.JT_KNIGHT, "Knight");
            ClassNames.Add((int)Enums.Jobtbl.JT_PRIEST, "Priest");
            ClassNames.Add((int)Enums.Jobtbl.JT_WIZARD, "Wizard");
            ClassNames.Add((int)Enums.Jobtbl.JT_BLACKSMITH, class_blacksmith);
            ClassNames.Add((int)Enums.Jobtbl.JT_HUNTER, "Hunter");
            ClassNames.Add((int)Enums.Jobtbl.JT_ASSASSIN, "Assassin");
            ClassNames.Add((int)Enums.Jobtbl.JT_CHICKEN, "Knight");
            ClassNames.Add((int)Enums.Jobtbl.JT_CRUSADER, "Crusader");
            ClassNames.Add((int)Enums.Jobtbl.JT_MONK, "Monk");
            ClassNames.Add((int)Enums.Jobtbl.JT_SAGE, "Sage");
            ClassNames.Add((int)Enums.Jobtbl.JT_ROGUE, "Rogue");
            ClassNames.Add((int)Enums.Jobtbl.JT_ALCHEMIST, "Alchemist");
            ClassNames.Add((int)Enums.Jobtbl.JT_BARD, "Bard");
            ClassNames.Add((int)Enums.Jobtbl.JT_DANCER, "Dancer");
            ClassNames.Add((int)Enums.Jobtbl.JT_GENETIC, "Genetic");
        }
    }
}
