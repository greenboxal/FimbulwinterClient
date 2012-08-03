using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Gui;
using FimbulwinterClient.Gui.Ingame;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FimbulwinterClient.Core.Assets;
using Microsoft.Xna.Framework.Input;
using FimbulwinterClient.Core;

namespace FimbulwinterClient.Screens
{
    public class IngameScreen : IGameScreen
    {
        Camera _camera;

        SpriteFont _font;
        Map _map;

        public IngameScreen(Map map)
        {
            _map = map;
            _font = SharedInformation.ContentManager.Load<SpriteFont>(@"fb\Gulim8b.xnb");

            _camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, -1), Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Down), 1.0F, 5000.0F);
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            ROClient.Singleton.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            sb.Begin();
            sb.DrawString(_font, string.Format("X={0}, Y={1}, Z={2} -> X={3}, Y={4}, Z={5}", _camera.Position.X, _camera.Position.Y, _camera.Position.Y, _camera.Target.X, _camera.Target.Y, _camera.Target.Z), new Vector2(10, 10), Color.White);
            sb.End();

            ROClient.Singleton.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ROClient.Singleton.GraphicsDevice.BlendState = BlendState.Opaque;
            ROClient.Singleton.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            _map.Draw(gameTime, _camera.View, _camera.Projection, _camera.World);
        }

        Vector2 tempMousePosition;
        MouseState prevMouseState;
        public void Update(SpriteBatch sb, GameTime gameTime)
        {
            if (!ROClient.Singleton.IsActive)
                return;

            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 10.0f;

            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);

            if (keyboardState.IsKeyDown(Keys.W))
                _camera.MoveForward(1.0f * timeDifference);
            else if (keyboardState.IsKeyDown(Keys.S))
                _camera.MoveForward(-1.0f * timeDifference);

            if (keyboardState.IsKeyDown(Keys.A))
                _camera.Strafe(-1.0f * timeDifference);
            else if (keyboardState.IsKeyDown(Keys.D))
                _camera.Strafe(1.0f * timeDifference);

            if (keyboardState.IsKeyDown(Keys.LeftShift))
                _camera.Levitate(1.0f * timeDifference);
            else if (keyboardState.IsKeyDown(Keys.LeftControl))
                _camera.Levitate(-1.0f * timeDifference);

            Viewport viewport = SharedInformation.GraphicsDevice.Viewport;

            if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Pressed)
            {
                Vector2 center = GetViewportCenter(viewport);

                _camera.AddYaw((center.X - mousePos.X) / 100.0F * timeDifference);
                _camera.AddPitch((center.Y - mousePos.Y) / 100.0F * timeDifference);

                Mouse.SetPosition((int)center.X, (int)center.Y);
            }
            else if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released)
            {
                Vector2 center = GetViewportCenter(viewport);
                tempMousePosition = new Vector2(mouseState.X, mouseState.Y);
                Mouse.SetPosition((int)center.X, (int)center.Y);
            }
            else if (mouseState.RightButton == ButtonState.Released && prevMouseState.RightButton == ButtonState.Pressed)
            {
                Mouse.SetPosition((int)tempMousePosition.X, (int)tempMousePosition.Y);
            }

            if (mousePos.X >= viewport.X && mousePos.Y >= viewport.Y && mousePos.X <= viewport.X + viewport.Width && mousePos.Y <= viewport.Y + viewport.Height)
            {
                _camera.UpdateMouseRay(mousePos, viewport);
            }

            _camera.Update();
            _map.Update(gameTime);

            prevMouseState = mouseState;
        }

        public static Vector2 GetViewportCenter(Viewport viewport)
        {
            return new Vector2(viewport.X + viewport.Width / 2, viewport.Y + viewport.Height / 2);
        }

        public static Rectangle GetViewportRectangle(Viewport viewport)
        {
            return new Rectangle(0, 0, viewport.Width, viewport.Height);
        }

        public void Dispose()
        {

        }
    }
}
