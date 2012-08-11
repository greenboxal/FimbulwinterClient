using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FimbulvetrEngine;
using FimbulvetrEngine.Framework;
using FimbulvetrEngine.IO;
using FimbulvetrEngine.Plugin;
using FimbulwinterClient.Core;
using FimbulwinterClient.GameStates;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace FimbulwinterClient
{
    public class Ragnarok : Game
    {
        public new static Ragnarok Instance { get; private set; }

        public GameState GameState { get; private set; }

        public Ragnarok()
        {
            if (Instance != null)
                throw new Exception("This class can have only one instance, use Ragnarok.Instance.");

            Initialization.DoInit();

            Instance = this;
        }

        protected override void ReadConfiguration()
        {
            Vetr.Instance.ReadConfiguration("ragnarok.xml", "Ragnarok");
            PluginManager.Instance.LoadPlugins();
            FileSystemManager.Instance.LoadAll();

            ReadGameConfig();
        }

        protected override void Initialize()
        {
            OnResize(null);

            ChangeWorld("prontera");
        }

        protected override void OnResize(EventArgs e)
        {
            //GL.Viewport(0, 0, Width, Height);

            float aspectRatio = Width / (float)Height;
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0F, 5000.0F);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (GameState != null)
                GameState.Update(e);

            ThreadBoundGC.Collect();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (GameState != null)
                GameState.Render(e);

            SwapBuffers();
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