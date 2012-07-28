using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FimbulwinterClient.Content;
using Microsoft.Xna.Framework.Input;
using FimbulwinterClient.GUI;
using FimbulwinterClient.GUI.Ingame;
using System.IO;

namespace FimbulwinterClient.Screens
{
    class TestMap : IGameScreen
    {
        BasicEffect effect;
        Matrix viewMatrix;
        Matrix projectionMatrix;

        Vector3 cameraPosition = new Vector3(6, 6, 1200);
        float leftrightRot;
        float updownRot;
        const float rotationSpeed = 0.3f;
        const float moveSpeed = 150.0f;
        MouseState originalMouseState;

        private string _mapname;
        Map _map;

        public TestMap(string mapname)
        {
            _mapname = mapname;
            _map = ROClient.Singleton.ContentManager.LoadContent<Map>(Path.Combine("data\\", _mapname));
            //ROClient.Singleton.GuiManager.Controls.Add(new QuickSlotWindow());
            //ROClient.Singleton.GuiManager.Controls.Add(new CollectionInfoWindow());

            leftrightRot = MathHelper.ToRadians(90);
            updownRot = -MathHelper.Pi / _map.Ground.Zoom;

            Mouse.SetPosition(ROClient.Singleton.GraphicsDevice.Viewport.Width / 2, ROClient.Singleton.GraphicsDevice.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();

            effect = new BasicEffect(ROClient.Singleton.GraphicsDevice);
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            ROClient.Singleton.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            var sf = ROClient.Singleton.GuiManager.Client.Content.Load<SpriteFont>("fb\\Gulim8b");
            sb.Begin();
            //sb.Draw(_map.ShadowLightmap, new Rectangle(0, 0, _map.ShadowLightmap.Width, _map.ShadowLightmap.Height), Color.White);
            //sb.Draw(_map.ColorLightmap, new Rectangle(0, 0, _map.ColorLightmap.Width, _map.ColorLightmap.Height), Color.White);
            sb.DrawString(sf, string.Format("X={0}, Y={1}, Z={2} -> X={3}, Y={4}, Z={5}", cameraPosition.X, cameraPosition.Y, cameraPosition.Y, cameraFinalTarget.X, cameraFinalTarget.Y, cameraFinalTarget.Z), new Vector2(10, 10), Color.White);
            sb.End();

            ROClient.Singleton.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ROClient.Singleton.GraphicsDevice.BlendState = BlendState.Opaque;
            ROClient.Singleton.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            Matrix worldMatrix = Matrix.Identity;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), ROClient.Singleton.GraphicsDevice.Viewport.AspectRatio, 1.0f, 5000.0F);

            _map.Draw(gameTime, viewMatrix, projectionMatrix, worldMatrix);
        }

        public void Update(SpriteBatch sb, GameTime gameTime)
        {
            _map.Update(gameTime);
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            ProcessInput(timeDifference);
        }

        private void ProcessInput(float amount)
        {
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                leftrightRot -= rotationSpeed * xDifference * amount;
                updownRot -= rotationSpeed * yDifference * amount;
                Mouse.SetPosition(ROClient.Singleton.GraphicsDevice.Viewport.Width / 2, ROClient.Singleton.GraphicsDevice.Viewport.Height / 2);
                UpdateViewMatrix();
            }

            Vector3 moveVector = new Vector3(0, 0, 0);
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
                moveVector += new Vector3(0, 0, -1);
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
                moveVector += new Vector3(0, 0, 1);
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                moveVector += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
                moveVector += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.Q))
                moveVector += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.Z))
                moveVector += new Vector3(0, -1, 0);
            AddToCameraPosition(moveVector * amount);
        }

        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            cameraPosition += moveSpeed * rotatedVector;
            UpdateViewMatrix();
        }

        Vector3 cameraFinalTarget;
        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);

            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            cameraFinalTarget = cameraPosition + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraFinalTarget, cameraRotatedUpVector);
        }

        public void Dispose()
        {

        }
    }
}
