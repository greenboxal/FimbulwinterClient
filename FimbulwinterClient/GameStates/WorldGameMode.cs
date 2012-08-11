using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using FimbulwinterClient.Core;
using FimbulwinterClient.Core.Content;
using FimbulwinterClient.Core.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using QuickFont;

namespace FimbulwinterClient.GameStates
{
    public class WorldGameState : GameState
    {
        public string WorldName { get; private set; }
        public Map World { get; private set; }
        public WorldRenderer WorldRenderer { get; private set; }
        public Camera Camera { get; private set; }

        public WorldGameState(string worldName)
        {
            WorldName = worldName;
        }

        public override void Setup()
        {
            Camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, -1), 1.0F, 5000.0F);
            World = ContentManager.Instance.Load<Map>(@"data\" + WorldName + ".gat");
            
            WorldRenderer = new WorldRenderer(World);
            WorldRenderer.LoadResources();

            Ragnarok.Instance.Mouse.ButtonDown += Mouse_ButtonDown;
            Ragnarok.Instance.Mouse.ButtonUp += Mouse_ButtonUp;
        }

        private bool _pressed;
        void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right)
            {
                _pressed = true;
            }
        }

        void Mouse_ButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right)
            {
                _pressed = false;
            }
        }

        public override void Update(FrameEventArgs e)
        {
            float timeDifference = (float)e.Time * 100;

            KeyboardDevice keyboardState = Ragnarok.Instance.Keyboard;

            Vector2 mousePos = new Vector2(Ragnarok.Instance.X + Ragnarok.Instance.Mouse.X, Ragnarok.Instance.Y + Ragnarok.Instance.Mouse.Y);

            if (keyboardState[Key.W])
                Camera.MoveForward(1.0f * timeDifference);
            else if (keyboardState[Key.S])
                Camera.MoveForward(-1.0f * timeDifference);

            if (keyboardState[Key.A])
                Camera.Strafe(-1.0f * timeDifference);
            else if (keyboardState[Key.D])
                Camera.Strafe(1.0f * timeDifference);

            if (keyboardState[Key.ShiftLeft])
                Camera.Levitate(1.0f * timeDifference);
            else if (keyboardState[Key.ControlLeft])
                Camera.Levitate(-1.0f * timeDifference);

            if (keyboardState[Key.F])
            {
                World = null;
                WorldRenderer = null;
                GC.Collect();
            }

            if (_pressed)
            {
                // I don't care
#pragma warning disable 612,618
                Camera.AddYaw(-Ragnarok.Instance.Mouse.XDelta / 100.0F * timeDifference);
                Camera.AddPitch(-Ragnarok.Instance.Mouse.YDelta / 100.0F * timeDifference);
#pragma warning restore 612,618
            }

            if (WorldRenderer != null)
                WorldRenderer.Update(e.Time);

            Ragnarok.Instance.Title = string.Format("X={0}, Y={1}, Z={2} -> X={3}, Y={4}, Z={5}", Camera.Position.X, Camera.Position.Y, Camera.Position.Y, Camera.Target.X, Camera.Target.Y, Camera.Target.Z);
        }

        public override void Render(FrameEventArgs e)
        {
            Camera.Update();

            if (WorldRenderer != null)
                WorldRenderer.Render(e.Time);
        }

        public override void Shutdown()
        {

        }
    }
}