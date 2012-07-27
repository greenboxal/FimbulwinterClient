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
        VertexBuffer vertexBuffer;
        Dictionary<Texture2D, VertexBuffer> vertexBuffers;

        Effect effect;
        Matrix viewMatrix;
        Matrix projectionMatrix;

        Vector3 cameraPosition = new Vector3(6, 6, 1200);
        float leftrightRot = MathHelper.PiOver2;
        float updownRot = -MathHelper.Pi / 10.0f;
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

            Mouse.SetPosition(ROClient.Singleton.GraphicsDevice.Viewport.Width / 2, ROClient.Singleton.GraphicsDevice.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();

            effect = ROClient.Singleton.Content.Load<Effect>("Effect2");
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            ROClient.Singleton.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            var sf = ROClient.Singleton.GuiManager.Client.Content.Load<SpriteFont>("fb\\Gulim8b");
            sb.Begin();
            sb.DrawString(sf, string.Format("X={0}, Y={1}, Z={2} -> X={3}, Y={4}, Z={5}", cameraPosition.X, cameraPosition.Y, cameraPosition.Y, cameraFinalTarget.X, cameraFinalTarget.Y, cameraFinalTarget.Z), new Vector2(10, 10), Color.White);
            sb.End();

            effect.CurrentTechnique = effect.Techniques["Textured"];

            Matrix worldMatrix = Matrix.Identity;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, ROClient.Singleton.GraphicsDevice.Viewport.AspectRatio, 1.0f, 20000.0f);
            
            effect.Parameters["xWorld"].SetValue(worldMatrix);
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);

            effect.Parameters["xEnableLighting"].SetValue(true);
            effect.Parameters["xAmbient"].SetValue(0.4f);
            effect.Parameters["xLightDirection"].SetValue(new Vector3(-0.5f, -1, -0.5f));

            _map.Draw(effect);
        }

        public void Update(SpriteBatch sb, GameTime gameTime)
        {
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
