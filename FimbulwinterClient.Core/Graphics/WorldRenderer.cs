using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Graphics;
using FimbulwinterClient.Core.Content;
using FimbulwinterClient.Core.Content.MapInternals;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FimbulwinterClient.Core.Graphics
{
    public partial class WorldRenderer
    {
        public Map Map { get; private set; }

        public WorldRenderer(Map map)
        {
            Map = map;
        }

        public void LoadResources()
        {
            LoadGround();
            LoadWater();
        }

        private double _waterElapsed;
        public void Update(double elapsedTime)
        {
            _waterElapsed += elapsedTime;
            while (_waterElapsed >= 0.05F)
            {
                WaterCurrentTexture++;

                if (WaterCurrentTexture >= WaterTextures.Length)
                    WaterCurrentTexture = 0;

                _waterElapsed -= 0.05F;
            }
        }

        public void Render(double elapsedTime)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            // We don't need to see what is on our back
            GL.CullFace(CullFaceMode.Back);

            // Render ground
            GL.PushMatrix();
            GL.Rotate(180, Vector3.UnitX);
            {
                GL.DepthMask(true);

                GroundBuffer.Bind();
                GroundShaderProgram.Begin();
                GroundShaderProgram.SetLightmap(GroundLightmap);
                foreach (Tuple<Texture2D, IndexBuffer> t in GroundMeshes)
                {
                    GroundShaderProgram.SetTexture(t.Item1);
                    GroundBuffer.Render(BeginMode.Triangles, t.Item2, t.Item2.Count);
                }
                GroundShaderProgram.End();

                //GL.DepthMask(false);
            }
            GL.PopMatrix();

            // Render water
            GL.PushMatrix();
            GL.Rotate(180, Vector3.UnitX);
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                WaterBuffer.Bind();
                WaterShaderProgram.Begin();
                WaterShaderProgram.SetTexture(WaterTextures[WaterCurrentTexture]);
                WaterShaderProgram.SetAlpha(0.5F);
                WaterBuffer.Render(BeginMode.TriangleStrip, WaterIndexes, WaterIndexes.Count);
                WaterShaderProgram.End();

                GL.Disable(EnableCap.Blend);
            }
            GL.PopMatrix();

            // Render models
            GL.PushMatrix();
            GL.Rotate(180, Vector3.UnitX);
            {
                foreach (World.ModelObject obj in Map.World.Models)
                    obj.Draw(elapsedTime);
            }
            GL.PopMatrix();

            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
        }
    }
}
