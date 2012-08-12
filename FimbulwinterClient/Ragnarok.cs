using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FimbulvetrEngine;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Framework;
using FimbulvetrEngine.IO;
using FimbulvetrEngine.Plugin;
using FimbulwinterClient.Core;
using FimbulwinterClient.GameStates;
using Gwen;
using Gwen.Control;
using Gwen.Skin;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace FimbulwinterClient
{
    public class Ragnarok : Game
    {
        public new static Ragnarok Instance { get; private set; }

        public Stopwatch Stopwatch { get; private set; }
        public long LastTime { get; private set; }
        public Gwen.Input.OpenTK Input { get; private set; }
        public Gwen.Renderer.OpenTK Renderer { get; private set; }
        public Gwen.Skin.Base Skin { get; private set; }
        public Canvas Canvas { get; private set; }
        public GameState GameState { get; private set; }

        private int _fpsCounter;
        public int FPS { get; private set; }
        public Label FPSLabel { get; private set; }

        public Ragnarok()
        {
            if (Instance != null)
                throw new Exception("This class can have only one instance, use Ragnarok.Instance.");

            Initialization.DoInit();

            Stopwatch = new Stopwatch();

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
            GL.ClearColor(Color.CornflowerBlue);

            Renderer = new Gwen.Renderer.OpenTK();
            Skin = new TexturedBase(Renderer, ContentManager.Instance.Load<Stream>(@"data\fb\skin.png"));
            Canvas = new Canvas(Skin);

            Input = new Gwen.Input.OpenTK(this);
            Input.Initialize(Canvas);

            Canvas.SetSize(Width, Height);
            Canvas.ShouldDrawBackground = true;
            Canvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);

            Keyboard.KeyDown += KeyboardKeyDown;
            Keyboard.KeyUp += KeyboardKeyUp;

            Mouse.ButtonDown += MouseButtonDown;
            Mouse.ButtonUp += MouseButtonUp;
            Mouse.Move += MouseMove;
            Mouse.WheelChanged += MouseWheel;

            // FPS Onscreen Display
            FPSLabel = new Label(Canvas);
            FPSLabel.SetPosition(10, 10);
            FPSLabel.SetBounds(10, 10, 100, 100);

            ChangeWorld("prontera");

            Stopwatch.Restart();
            LastTime = 0;
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            float aspectRatio = Width / (float)Height;
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0F, 5000.0F);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);

            Canvas.SetSize(Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            LastTime = Stopwatch.ElapsedMilliseconds;

            if (GameState != null)
                GameState.Update(e);

            if (LastTime > 1000)
            {
                Stopwatch.Restart();

                FPS = _fpsCounter;
                _fpsCounter = 0;

                FPSLabel.Text = "FPS: " + FPS;

                if (Renderer.TextCacheSize > 1000)
                    Renderer.FlushTextCache();

                ThreadBoundGC.Collect();
            }
            else
            {
                _fpsCounter++;
            }

            ContentManager.Instance.PoolBackgroundLoading(3);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            if (GameState != null)
                GameState.Render(e);

            Canvas.RenderCanvas();

            SwapBuffers();
        }

        private void KeyboardKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            Input.ProcessKeyDown(e);
        }

        private void KeyboardKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            Input.ProcessKeyUp(e);
        }

        private void MouseButtonDown(object sender, MouseButtonEventArgs args)
        {
            Input.ProcessMouseMessage(args);
        }

        private void MouseButtonUp(object sender, MouseButtonEventArgs args)
        {
            Input.ProcessMouseMessage(args);
        }

        private void MouseMove(object sender, MouseMoveEventArgs args)
        {
            Input.ProcessMouseMessage(args);
        }

        private void MouseWheel(object sender, MouseWheelEventArgs args)
        {
            Input.ProcessMouseMessage(args);
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

        public override void Dispose()
        {
            GC.Collect();
            ThreadBoundGC.Collect();
            Canvas.Dispose();
            Skin.Dispose();
            Renderer.Dispose();
            base.Dispose();
        }
    }
}