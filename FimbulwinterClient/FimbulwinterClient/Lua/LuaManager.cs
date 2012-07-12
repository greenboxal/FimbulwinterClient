using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LuaInterface;
using System.IO;
using FimbulwinterClient.Content;

namespace FimbulwinterClient.Lua
{
    public class LuaManager : GameComponent
    {

        private const string lua_folder = "data\\luafiles514\\lua files";

        private LuaInterface.Lua _luaparser;
        
        private Dictionary<int, Tuple<string, string>> accessories;
        public Dictionary<int, Tuple<string, string>> Accessories
        {
            get { return accessories; }
        }

        private Dictionary<int, Tuple<string, string>> robes;
        public Dictionary<int, Tuple<string, string>> Robes
        {
            get { return robes; }
        }

        private Dictionary<int, Tuple<string, string>> npcIdentity;
        public Dictionary<int, Tuple<string, string>> NpcIdentity
        {
            get { return npcIdentity; }
        }

        public LuaManager()
            : base(ROClient.Singleton)
        {
            _luaparser = new LuaInterface.Lua();

            LoadAccessory();
            LoadRobe();
            LoadNpcIdentity();
        }

        private void RunScript(string filename)
        {
            var file = ROClient.Singleton.ContentManager.LoadContent<Lub>(Path.Combine(lua_folder, filename));
            string temp = Path.GetTempFileName();
            using (FileStream fs = new FileStream(temp, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(file.Content);
                }
            }
            _luaparser.DoFile(temp);
            System.IO.File.Delete(temp);
            //if (!File.Exists(fullname))
                //throw new FileNotFoundException("The file could not be found.", fullname);
            //_luaparser.DoFile(fullname);
        }

        private void LoadAccessory()
        {
            RunScript("datainfo\\accessoryid.lub");
            RunScript("datainfo\\accname.lub");

            accessories = new Dictionary<int, Tuple<string, string>>();
            var tbl = _luaparser.GetTable("ACCESSORY_IDs");
            foreach (DictionaryEntry de in tbl)
                if (!accessories.ContainsKey(Convert.ToInt16(de.Value)))
                    accessories.Add(Convert.ToInt16(de.Value), new Tuple<string, string>(de.Key.ToString(), ""));

            tbl = _luaparser.GetTable("AccNameTable");
            foreach (DictionaryEntry de in tbl)
                if (accessories.ContainsKey(Convert.ToInt16(de.Key)))
                    accessories[Convert.ToInt16(de.Key)] = new Tuple<string, string>(accessories[Convert.ToInt16(de.Key)].Item1, de.Value.ToString().Korean());
        }

        private void LoadRobe()
        {
            RunScript("datainfo\\spriterobeid.lub");
            RunScript("datainfo\\spriterobename.lub");

            robes = new Dictionary<int, Tuple<string, string>>();
            var tbl = _luaparser.GetTable("SPRITE_ROBE_IDs");
            foreach (DictionaryEntry de in tbl)
                if (!robes.ContainsKey(Convert.ToInt16(de.Value)))
                    robes.Add(Convert.ToInt16(de.Value), new Tuple<string, string>(de.Key.ToString(), ""));

            tbl = _luaparser.GetTable("RobeNameTable");
            foreach (DictionaryEntry de in tbl)
                if (robes.ContainsKey(Convert.ToInt16(de.Key)))
                    robes[Convert.ToInt16(de.Key)] = new Tuple<string, string>(robes[Convert.ToInt16(de.Key)].Item1, de.Value.ToString().Korean());
        }

        private void LoadNpcIdentity()
        {
            RunScript("datainfo\\npcidentity.lub");
            RunScript("datainfo\\jobname.lub");

            npcIdentity = new Dictionary<int, Tuple<string, string>>();
            var tbl = _luaparser.GetTable("jobtbl");
            foreach (DictionaryEntry de in tbl)
                if (!npcIdentity.ContainsKey(Convert.ToInt16(de.Value)))
                    npcIdentity.Add(Convert.ToInt16(de.Value), new Tuple<string, string>(de.Key.ToString(), ""));

            tbl = _luaparser.GetTable("JobNameTable");
            foreach (DictionaryEntry de in tbl)
                if (npcIdentity.ContainsKey(Convert.ToInt16(de.Key)))
                    npcIdentity[Convert.ToInt16(de.Key)] = new Tuple<string, string>(npcIdentity[Convert.ToInt16(de.Key)].Item1, de.Value.ToString().Korean());
        }

    }
}
