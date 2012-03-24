using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FimbulwinterClient.Content;
using Microsoft.Xna.Framework.Input;

namespace FimbulwinterClient.Screens
{
    public class TestScreen : IGameScreen
    {
        RsmModel mdl;
        float rx, ry, rz;
        float cz;

        public TestScreen()
        {
            mdl = ROClient.Singleton.ContentManager.LoadContent<RsmModel>("data/model/ÇÁ·ÐÅ×¶ó/µµ±¸Á¡.rsm");
            cz = -250;
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            Matrix worldMatrix = Matrix.CreateRotationX(rx) * Matrix.CreateRotationY(ry) * Matrix.CreateRotationZ(rz);
            Matrix viewMatrix = Matrix.CreateLookAt(new Vector3(5, 5, cz), Vector3.Zero, Vector3.Up);

            Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),
                ROClient.Singleton.GraphicsDevice.Viewport.AspectRatio,
                1.0f, 10000.0f);
            mdl.World = worldMatrix;

            foreach (BasicEffect eff in mdl.Root.Effects)
            {
                eff.EnableDefaultLighting();
                eff.View = viewMatrix;
                eff.Projection = projectionMatrix;
            }

            mdl.Draw(new Vector3(1.0f,1.0f,1.0f));
        }

        float old = 0;
        public void Update(SpriteBatch sb, GameTime gameTime)
        {
            KeyboardState s = Keyboard.GetState();

            if (s.IsKeyDown(Keys.A))
                ry += 0.10f;
            else if (s.IsKeyDown(Keys.D))
                ry -= 0.10f;

            if (s.IsKeyDown(Keys.W))
                rz += 0.10f;
            else if (s.IsKeyDown(Keys.S))
                rz -= 0.10f;

            if (s.IsKeyDown(Keys.Up))
                cz += 0.10f;
            else if (s.IsKeyDown(Keys.Down))
                cz -= 0.10f;

            MouseState m = Mouse.GetState();
            rx += (m.ScrollWheelValue / 360) - old;
            old = m.ScrollWheelValue / 360;
        }

        public void Dispose()
        {
            
        }
    }
}
