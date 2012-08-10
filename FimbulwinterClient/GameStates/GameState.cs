using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Core;
using System.Threading;
using OpenTK;

namespace FimbulwinterClient.GameStates
{
    public abstract class GameState
    {
        public abstract void Setup();
        public abstract void Update(FrameEventArgs e);
        public abstract void Render(FrameEventArgs e);
        public abstract void Shutdown();
    }
}