using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using FimbulwinterClient.Core;
using FimbulwinterClient.Core.Content;
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
        public Camera Camera { get; private set; }

        public static Vector2 GetViewportCenter()
        {
            return new Vector2(Ragnarok.Instance.X + Ragnarok.Instance.Width / 2, Ragnarok.Instance.Y + Ragnarok.Instance.Height / 2);
        }

        public WorldGameState(string worldName)
        {
            WorldName = worldName;
        }

        public override void Setup()
        {
            Camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, -1), 1.0F, 5000.0F);
            World = ContentManager.Instance.Load<Map>(@"data\" + WorldName + ".gat");

            Ragnarok.Instance.Mouse.ButtonDown += Mouse_ButtonDown;
            Ragnarok.Instance.Mouse.ButtonUp += Mouse_ButtonUp;
        }

        Vector2 _tempMousePosition;
        private bool _pressed;
        void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right)
            {
                Vector2 center = GetViewportCenter();
                _tempMousePosition = new Vector2(e.X, e.Y);
                Mouse.SetPosition((int)center.X, (int)center.Y);
                _pressed = true;
            }
        }

        void Mouse_ButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right)
            {
                Mouse.SetPosition((int)_tempMousePosition.X, (int)_tempMousePosition.Y);
                _pressed = false;
            }
        }

        public override void Update(FrameEventArgs e)
        {
            float timeDifference = (float)e.Time;

            KeyboardDevice keyboardState = Ragnarok.Instance.Keyboard;

            Vector2 mousePos = new Vector2(Ragnarok.Instance.Mouse.X, Ragnarok.Instance.Mouse.Y);

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
                GC.Collect();
            }

            if (_pressed)
            {
                Vector2 center = GetViewportCenter();

                Camera.AddYaw((center.X - mousePos.X) / 100.0F * timeDifference);
                Camera.AddPitch((center.Y - mousePos.Y) / 100.0F * timeDifference);

                Mouse.SetPosition((int)center.X, (int)center.Y);
            }

            Camera.Update();

            Ragnarok.Instance.Title = string.Format("X={0}, Y={1}, Z={2} -> X={3}, Y={4}, Z={5}", Camera.Position.X, Camera.Position.Y, Camera.Position.Y, Camera.Target.X, Camera.Target.Y, Camera.Target.Z);
        }

        public override void Render(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.CornflowerBlue);

            Matrix4 view = Matrix4.LookAt(new Vector3(0, 0, 0), new Vector3(0, 0, -1), Vector3.UnitY);
            
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref view);

            //World.Draw(e.Time);
        }

        public override void Shutdown()
        {

        }
    }
}