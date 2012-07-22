using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace FimbulwinterClient
{
    public static class Statics
    {
        public static readonly string Humans = "인간족";
        public static readonly string Folder_Robes = "로브";
        public static readonly string Folder_Accessories = "악세사리";
        public static readonly string Monster = "몬스터";

        public static readonly string Palette_Body = "몸";
        public static readonly string Palette_Head = "머리";

        public static readonly string Body = "몸통";
        public static readonly string Head = "머리통";
        public static readonly string[] Sex = new string[] { "여", "남" };

        public static readonly string class_novice = "초보자";
        public static readonly string class_swordsman = "검사";
        public static readonly string class_magician = "마법사";
        public static readonly string class_archer = "궁수";
        public static readonly string class_acolyte = "성직자";
        public static readonly string class_merchant = "상인";
        public static readonly string class_thief = "도둑";

        public static readonly string class_knight = "기사";
        public static readonly string class_priest = "프리스트";
        public static readonly string class_wizard = "위저드";
        public static readonly string class_blacksmith = "제철공";
        public static readonly string class_hunter = "헌터";
        public static readonly string class_assassin = "어세신";
        public static readonly string class_chicken = "";
        public static readonly string class_crusader = "크루세이더";
        public static readonly string class_monk = "몽크";
        public static readonly string class_sage = "세이지";
        public static readonly string class_rogue = "로그";
        public static readonly string class_alchemist = "연금술사";
        public static readonly string class_bard = "";
        public static readonly string class_dancer = "무희";

        public static readonly string class_knight_h = "로드나이트";
        public static readonly string class_priest_h = "성투사2";
        public static readonly string class_blacksmith_h = "화이트스미스";
        public static readonly string class_assassin_h = "어쌔신크로스";
        public static readonly string class_hunter_h = "스나이퍼";
        public static readonly string class_rogue_h = "스토커";
        public static readonly string class_monk_h = "챔피온_여";
        public static readonly string class_alchemist_h = "크리에이터";

        public static readonly string class_runeknight = "룬나이트";
        public static readonly string class_ranger = "레인져";
        public static readonly string class_archbishop = "아크비숍";
        public static readonly string class_genetic = "미케닉";
        public static readonly string class_guadian = "가드";
        public static readonly string class_guillotinx = "길로틴크로스";
        public static readonly string class_warlock = "워록";
        public static readonly string class_sura = "슈라";
        public static readonly string class_shadowchaser = "쉐도우체이서";
        public static readonly string class_genetic2 = "제네릭";
        public static readonly string class_wanderer = "원더러";
        public static readonly string class_scholar = "소서러";

        public static readonly string class_oboro = "oboro";


        //private static Texture2D _backgroundImage;
        //public static Texture2D BackgroundImage
        //{
        //    get { return _backgroundImage; }
        //    set { _backgroundImage = value; }
        //}

        public static readonly Dictionary<int, string> ClassNames;
        public static readonly Dictionary<string, string> MapNames;

        private static Dictionary<int, string> _classSprites;
        public static Dictionary<int, string> ClassSprites
        {
            get { return _classSprites; }
        }

        private static Dictionary<int, Tuple<string, string>> _accessories;
        public static Dictionary<int, Tuple<string, string>> Accessories
        {
            get
            {
                if (_accessories == null) _accessories = new Dictionary<int, Tuple<string, string>>();
                return _accessories;
            }
        }

        private static Dictionary<int, Tuple<string, string>> _robes;
        public static Dictionary<int, Tuple<string, string>> Robes
        {
            get
            {
                if (_robes == null) _robes = new Dictionary<int, Tuple<string, string>>();
                return _robes;
            }
        }

        private static Dictionary<int, Tuple<string, string>> _npcIdentity;
        public static Dictionary<int, Tuple<string, string>> NpcIdentity
        {
            get
            {
                if (_npcIdentity == null) _npcIdentity = new Dictionary<int, Tuple<string, string>>();
                return _npcIdentity;
            }
        }

        static Statics()
        {
            try
            {
                MapNames = new Dictionary<string, string>();
                MapNames.Add("payon.gat", "유저인터페이스");

                _classSprites = new Dictionary<int, string>();
                _classSprites.Add((int)Jobtbl.JT_NOVICE, class_novice);
                _classSprites.Add((int)Jobtbl.JT_SWORDMAN, class_swordsman);
                _classSprites.Add((int)Jobtbl.JT_MAGICIAN, class_magician);
                _classSprites.Add((int)Jobtbl.JT_ARCHER, class_archer);
                _classSprites.Add((int)Jobtbl.JT_ACOLYTE, class_acolyte);
                _classSprites.Add((int)Jobtbl.JT_MERCHANT, class_merchant);
                _classSprites.Add((int)Jobtbl.JT_THIEF, class_thief);
                _classSprites.Add((int)Jobtbl.JT_KNIGHT, class_knight);
                _classSprites.Add((int)Jobtbl.JT_PRIEST, class_priest);
                _classSprites.Add((int)Jobtbl.JT_WIZARD, class_wizard);
                _classSprites.Add((int)Jobtbl.JT_BLACKSMITH, class_blacksmith);
                _classSprites.Add((int)Jobtbl.JT_HUNTER, class_hunter);
                _classSprites.Add((int)Jobtbl.JT_ASSASSIN, class_assassin);
                _classSprites.Add((int)Jobtbl.JT_CHICKEN, class_chicken);
                _classSprites.Add((int)Jobtbl.JT_CRUSADER, class_crusader);
                _classSprites.Add((int)Jobtbl.JT_MONK, class_monk);
                _classSprites.Add((int)Jobtbl.JT_SAGE, class_sage);
                _classSprites.Add((int)Jobtbl.JT_ROGUE, class_rogue);
                _classSprites.Add((int)Jobtbl.JT_ALCHEMIST, class_alchemist);
                _classSprites.Add((int)Jobtbl.JT_BARD, class_bard);
                _classSprites.Add((int)Jobtbl.JT_DANCER, class_dancer);
                _classSprites.Add((int)Jobtbl.JT_SURA, class_sura);
                _classSprites.Add((int)Jobtbl.JT_GENETIC, class_genetic);
                _classSprites.Add((int)Jobtbl.JT_OBORO, class_oboro);
                _classSprites.Add((int)Jobtbl.JT_ALCHEMIST_H, class_alchemist_h);
                _classSprites.Add((int)Jobtbl.JT_BLACKSMITH_H, class_blacksmith_h);

                // temp
                int g_serviceType = 2;

                ClassNames = new Dictionary<int, string>();
                // first class
                ClassNames.Add((int)Jobtbl.JT_NOVICE, "Novice");
                ClassNames.Add((int)Jobtbl.JT_SWORDMAN, "Swordman");
                ClassNames.Add((int)Jobtbl.JT_MAGICIAN, "Magician");
                ClassNames.Add((int)Jobtbl.JT_ARCHER, "Archer");
                ClassNames.Add((int)Jobtbl.JT_ACOLYTE, "Acolyte");
                ClassNames.Add((int)Jobtbl.JT_MERCHANT, "Merchant");
                ClassNames.Add((int)Jobtbl.JT_THIEF, "Thief");

                // 2-1
                ClassNames.Add((int)Jobtbl.JT_KNIGHT, "Knight");
                ClassNames.Add((int)Jobtbl.JT_PRIEST, "Priest");
                ClassNames.Add((int)Jobtbl.JT_WIZARD, "Wizard");
                ClassNames.Add((int)Jobtbl.JT_BLACKSMITH, "Blacksmith");
                ClassNames.Add((int)Jobtbl.JT_HUNTER, "Hunter");
                ClassNames.Add((int)Jobtbl.JT_ASSASSIN, "Assassin");
                ClassNames.Add((int)Jobtbl.JT_CHICKEN, "Knight");

                // 2-2
                ClassNames.Add((int)Jobtbl.JT_CRUSADER, "Crusader");
                ClassNames.Add((int)Jobtbl.JT_MONK, "Monk");
                ClassNames.Add((int)Jobtbl.JT_SAGE, "Sage");
                ClassNames.Add((int)Jobtbl.JT_ROGUE, "Rogue");
                ClassNames.Add((int)Jobtbl.JT_ALCHEMIST, "Alchemist");
                ClassNames.Add((int)Jobtbl.JT_BARD, "Bard");
                ClassNames.Add((int)Jobtbl.JT_DANCER, "Dancer");
                ClassNames.Add((int)Jobtbl.JT_CHICKEN2, "Crusader");


                ClassNames.Add((int)Jobtbl.JT_SUPERNOVICE, "Super Novice");
                ClassNames.Add((int)Jobtbl.JT_GUNSLINGER, "Gunslinger");
                ClassNames.Add((int)Jobtbl.JT_NINJA, "Ninja");
                ClassNames.Add((int)Jobtbl.JT_NOVICE_H, "Novice High");

                // transcendent
                ClassNames.Add((int)Jobtbl.JT_SWORDMAN_H, "Swordman High");
                ClassNames.Add((int)Jobtbl.JT_MAGICIAN_H, "Magician High");
                ClassNames.Add((int)Jobtbl.JT_ARCHER_H, "Archer High");
                ClassNames.Add((int)Jobtbl.JT_ACOLYTE_H, "Acolyte High");
                ClassNames.Add((int)Jobtbl.JT_MERCHANT_H, "Merchant High");
                ClassNames.Add((int)Jobtbl.JT_THIEF_H, "Thief High");
                ClassNames.Add((int)Jobtbl.JT_KNIGHT_H, "Lord Knight");
                ClassNames.Add((int)Jobtbl.JT_PRIEST_H, "High Priest");
                ClassNames.Add((int)Jobtbl.JT_WIZARD_H, "High Wizard");
                ClassNames.Add((int)Jobtbl.JT_BLACKSMITH_H, "Whitesmith");
                ClassNames.Add((int)Jobtbl.JT_HUNTER_H, "Sniper");
                ClassNames.Add((int)Jobtbl.JT_ASSASSIN_H, "Assassin Cross");
                ClassNames.Add((int)Jobtbl.JT_CHICKEN_H, "Lord Knight");
                ClassNames.Add((int)Jobtbl.JT_CRUSADER_H, "Paladin");
                ClassNames.Add((int)Jobtbl.JT_MONK_H, "Champion");
                ClassNames.Add((int)Jobtbl.JT_SAGE_H, "Professor");
                ClassNames.Add((int)Jobtbl.JT_ROGUE_H, "Stalker");
                ClassNames.Add((int)Jobtbl.JT_ALCHEMIST_H, "Creator");
                ClassNames.Add((int)Jobtbl.JT_BARD_H, "Clown");
                ClassNames.Add((int)Jobtbl.JT_DANCER_H, "Gypsy");
                ClassNames.Add((int)Jobtbl.JT_CHICKEN2_H, "Paladin");

                // baby
                ClassNames.Add((int)Jobtbl.JT_NOVICE_B, "Baby Novice");
                ClassNames.Add((int)Jobtbl.JT_SWORDMAN_B, "Baby Swordman");
                ClassNames.Add((int)Jobtbl.JT_MAGICIAN_B, "Baby Magician");
                ClassNames.Add((int)Jobtbl.JT_ARCHER_B, "Baby Archer");
                ClassNames.Add((int)Jobtbl.JT_ACOLYTE_B, "Baby Acolyte");
                ClassNames.Add((int)Jobtbl.JT_MERCHANT_B, "Baby Merchant");
                ClassNames.Add((int)Jobtbl.JT_THIEF_B, "Baby Thief");
                ClassNames.Add((int)Jobtbl.JT_KNIGHT_B, "Baby Knight");
                ClassNames.Add((int)Jobtbl.JT_PRIEST_B, "Baby Priest");
                ClassNames.Add((int)Jobtbl.JT_WIZARD_B, "Baby Wizard");
                ClassNames.Add((int)Jobtbl.JT_BLACKSMITH_B, "Baby Blacksmith");
                ClassNames.Add((int)Jobtbl.JT_HUNTER_B, "Baby Hunter");
                ClassNames.Add((int)Jobtbl.JT_ASSASSIN_B, "Baby Assassin");
                ClassNames.Add((int)Jobtbl.JT_CHICKEN_B, "Baby Knight");
                ClassNames.Add((int)Jobtbl.JT_CRUSADER_B, "Baby Crusader");
                ClassNames.Add((int)Jobtbl.JT_MONK_B, "Baby Monk");
                ClassNames.Add((int)Jobtbl.JT_SAGE_B, "Baby Sage");
                ClassNames.Add((int)Jobtbl.JT_ROGUE_B, "Baby Rogue");
                ClassNames.Add((int)Jobtbl.JT_ALCHEMIST_B, "Baby Alchemist");
                ClassNames.Add((int)Jobtbl.JT_BARD_B, "Baby Bard");
                ClassNames.Add((int)Jobtbl.JT_DANCER_B, "Baby Dancer");
                ClassNames.Add((int)Jobtbl.JT_CHICKEN2_B, "Baby Crusader");
                ClassNames.Add((int)Jobtbl.JT_SUPERNOVICE_B, "Super Baby");

                ClassNames.Add((int)Jobtbl.JT_TAEKWON, "TaeKwon Boy");
                ClassNames.Add((int)Jobtbl.JT_STAR, "Star Gladiator");
                ClassNames.Add((int)Jobtbl.JT_STAR2, "Star Gladiator");
                ClassNames.Add((int)Jobtbl.JT_LINKER, "Soul Linker");
                ClassNames.Add((int)Jobtbl.JT_GANGSI, "Munak");
                ClassNames.Add((int)Jobtbl.JT_DEATHKNIGHT, "Death Knight");
                ClassNames.Add((int)Jobtbl.JT_COLLECTOR, "Dark Collector");

                // 3-1
                ClassNames.Add((int)Jobtbl.JT_RUNE_KNIGHT, "Rune Knight");
                ClassNames.Add((int)Jobtbl.JT_RUNE_KNIGHT_H, "Rune Knight");
                ClassNames.Add((int)Jobtbl.JT_ARCHBISHOP, "Arch Bishop");
                ClassNames.Add((int)Jobtbl.JT_ARCHBISHOP_H, "Arch Bishop");
                ClassNames.Add((int)Jobtbl.JT_WARLOCK, "Warlock");
                ClassNames.Add((int)Jobtbl.JT_WARLOCK_H, "Warlock");
                ClassNames.Add((int)Jobtbl.JT_MECHANIC, "Mechanic");
                ClassNames.Add((int)Jobtbl.JT_MECHANIC_H, "Mechanic");
                ClassNames.Add((int)Jobtbl.JT_RANGER, "Ranger");
                ClassNames.Add((int)Jobtbl.JT_RANGER_H, "Ranger");
                ClassNames.Add((int)Jobtbl.JT_GUILLOTINE_CROSS, "Glt. Cross");
                ClassNames.Add((int)Jobtbl.JT_GUILLOTINE_CROSS_H, "Glt. Cross");

                // 3-2
                ClassNames.Add((int)Jobtbl.JT_ROYAL_GUARD, "Royal Guard");
                ClassNames.Add((int)Jobtbl.JT_ROYAL_GUARD_H, "Royal Guard");
                ClassNames.Add((int)Jobtbl.JT_SURA, "Sura");
                ClassNames.Add((int)Jobtbl.JT_SURA_H, "Sura");
                ClassNames.Add((int)Jobtbl.JT_SORCERER, "Sorcerer");
                ClassNames.Add((int)Jobtbl.JT_SORCERER_H, "Sorcerer");
                ClassNames.Add((int)Jobtbl.JT_GENETIC, "Genetic");
                ClassNames.Add((int)Jobtbl.JT_GENETIC_H, "Genetic");
                ClassNames.Add((int)Jobtbl.JT_SHADOW_CHASER, "Shadow Chaser");
                ClassNames.Add((int)Jobtbl.JT_SHADOW_CHASER_H, "Shadow Chaser");
                ClassNames.Add((int)Jobtbl.JT_WANDERER, "Wanderer");
                ClassNames.Add((int)Jobtbl.JT_WANDERER_H, "Wanderer");
                ClassNames.Add((int)Jobtbl.JT_MINSTREL, "Minstrel");
                ClassNames.Add((int)Jobtbl.JT_MINSTREL_H, "Minstrel");

                //
                ClassNames.Add((int)Jobtbl.JT_KAGEROU, "Kagerou");
                ClassNames.Add((int)Jobtbl.JT_OBORO, "Oboro");

                // localization
                if (g_serviceType == (int)ServiceType.ServiceAmerica)
                {
                    ClassNames[(int)Jobtbl.JT_MAGICIAN] = "Mage";
                    ClassNames[(int)Jobtbl.JT_NOVICE_H] = "High Novice";
                    ClassNames[(int)Jobtbl.JT_SWORDMAN_H] = "High Swordman";
                    ClassNames[(int)Jobtbl.JT_MAGICIAN_H] = "High Mage";
                    ClassNames[(int)Jobtbl.JT_ARCHER_H] = "High Archer";
                    ClassNames[(int)Jobtbl.JT_ACOLYTE_H] = "High Acolyte";
                    ClassNames[(int)Jobtbl.JT_MERCHANT_H] = "High Merchant";
                    ClassNames[(int)Jobtbl.JT_THIEF_H] = "High Thief";
                    ClassNames[(int)Jobtbl.JT_BLACKSMITH_H] = "Mastersmith";
                    ClassNames[(int)Jobtbl.JT_SAGE_H] = "Scholar";
                    ClassNames[(int)Jobtbl.JT_ALCHEMIST_H] = "Biochemist";
                    ClassNames[(int)Jobtbl.JT_BARD_H] = "Minstrel";
                    ClassNames[(int)Jobtbl.JT_STAR] = "Taekwon Master";
                    ClassNames[(int)Jobtbl.JT_STAR2] = "Taekwon Master";
                }
                if (g_serviceType == (int)ServiceType.ServiceJapan)
                {
                    ClassNames[(int)Jobtbl.JT_ROGUE_H] = "Chaser";
                }

                if (g_serviceType == (int)ServiceType.ServiceIndonesia)
                {
                    ClassNames[(int)Jobtbl.JT_CRUSADER] = "Defender";
                    ClassNames[(int)Jobtbl.JT_CHICKEN2] = "Defender";
                    ClassNames[(int)Jobtbl.JT_CRUSADER_B] = "Baby Defender";
                    ClassNames[(int)Jobtbl.JT_CHICKEN2_B] = "Baby Defender";
                }
                if (g_serviceType == (int)ServiceType.ServicePhilippine)
                {
                    ClassNames[(int)Jobtbl.JT_BLACKSMITH_H] = "Mastersmith";
                    ClassNames[(int)Jobtbl.JT_SAGE_H] = "Scholar";
                    ClassNames[(int)Jobtbl.JT_ALCHEMIST_H] = "Biochemist";
                    ClassNames[(int)Jobtbl.JT_BARD_H] = "Minstrel";
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
        }
    }
}
