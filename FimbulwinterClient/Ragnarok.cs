using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FimbulvetrEngine.Framework;
using FimbulwinterClient.Core;
using FimbulwinterClient.GameStates;
using OpenTK;
using OpenTK.Input;

namespace FimbulwinterClient
{
    public class Ragnarok : Game
    {
        public static Ragnarok Instance { get; private set; }

        public GameState GameState { get; private set; }

        public Ragnarok()
        {
            if (Instance != null)
                throw new Exception("This class can have only one instance, use Ragnarok.Instance.");

            Instance = this;
        }

        protected override void Initialize()
        {
            Initialization.DoInit();

            ChangeWorld("prontera");
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (GameState != null)
                GameState.Update(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (GameState != null)
                GameState.Render(e);
        }

        public void ChangeWorld(string name)
        {
            ChangeGameState(new WorldGameState(name));
        }

        public void ChangeGameState(GameState state)
        {
            if (GameState != null)
            {
                GameState.Shutdown();
            }

            if (state != null)
            {
                state.Setup();
            }

            GameState = state;
        }
    }
}