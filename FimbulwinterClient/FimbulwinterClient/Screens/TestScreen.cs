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
        Vector3 modelPosition = Vector3.Forward;
        float modelRotation = 0.0f;

        // Set the position of the camera in world space, for our view matrix.
        Vector3 cameraPosition = new Vector3(0.0f, 25.0f, 500.0f);

        float aspectRatio = 0;

        public TestScreen()
        {
            mdl = ROClient.Singleton.ContentManager.LoadContent<RsmModel>("data/model/gld2/ÇÁ·Ð_¿©°ü.rsm");

            aspectRatio = ROClient.Singleton.GraphicsDevice.Viewport.AspectRatio;
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            Matrix[] transforms = new Matrix[mdl.Bones.Length];

            mdl.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (RsmMesh mesh in mdl.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(modelPosition);
                    effect.View = Matrix.CreateLookAt(cameraPosition,
                        Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 10000.0f);
                    effect.World = Matrix.Identity;
                }

                mesh.Draw();
            }
        }

        public void Update(SpriteBatch sb, GameTime gameTime)
        {
            modelRotation += 0.1f;
        }

        public void Dispose()
        {
            
        }
    }
}
