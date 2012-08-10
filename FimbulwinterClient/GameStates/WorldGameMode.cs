using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Core;
using OpenTK;

namespace FimbulwinterClient.GameStates
{
    public class WorldGameState : GameState
    {
        public string WorldName { get; private set; }


        public WorldGameState(string worldName)
        {
            WorldName = worldName;
        }

        public override void Setup()
        {
            
        }

        public override void Update(FrameEventArgs e)
        {
            
        }

        public override void Render(FrameEventArgs e)
        {
            
        }

        public override void Shutdown()
        {
            
        }
    }
}