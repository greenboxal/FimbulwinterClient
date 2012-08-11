using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Graphics;
using FimbulwinterClient.Core.Content.MapInternals;
using OpenTK;

namespace FimbulwinterClient.Core.Content
{
    public class Map
    {
        public Ground Ground { get; private set; }
        public Altitude Altitude { get; private set; }
        public World World { get; private set; }

        public Map()
        {
        }

        public bool Load(Stream gat, Stream gnd, Stream rsw)
        {
            Altitude = new Altitude();
            if (!Altitude.Load(gat))
                return false;

            Ground = new Ground();
            if (!Ground.Load(gnd))
                return false;

            World = new World(this);
            if (!World.Load(rsw))
                return false;

            return true;
        }
    }
}
