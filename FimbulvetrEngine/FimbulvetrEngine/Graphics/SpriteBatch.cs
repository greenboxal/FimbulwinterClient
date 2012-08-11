using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FimbulvetrEngine.Graphics
{
    public class SpriteBatch
    {
        private List<RenderOperation> _operations;

        private abstract class RenderOperation
        {
            public abstract void Render();
        }

        private class Texture2DRenderOperation : RenderOperation
        {
            private readonly Texture2D _texture;
            private readonly Vector2 _position;
            private readonly Vector2 _size;
            private readonly Color _color;

            public Texture2DRenderOperation(Texture2D texture, Vector2 position, Vector2 size, Color color)
            {
                _texture = texture;
                _position = position;
                _size = size;
                _color = color;
            }

            public override void Render()
            {
                if (_texture == null)
                {
                    GL.Color4(_color);
                }
                else
                {
                    _texture.Bind();

                    GL.BlendColor(_color);
                    GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
                }

                GL.Begin(BeginMode.Quads);
                {
                    GL.TexCoord2(0.0F, 0.0F); GL.Vertex2(_position.X, _position.Y);
                    GL.TexCoord2(1.0F, 0.0F); GL.Vertex2(_position.X + _size.X, _position.Y);
                    GL.TexCoord2(1.0F, 1.0F); GL.Vertex2(_position.X + _size.X, _position.Y + _size.Y);
                    GL.TexCoord2(0.0F, 1.0F); GL.Vertex2(_position.X, _position.Y + _size.Y);
                }
                GL.End();
            }
        }

        public void Begin()
        {
            _operations = new List<RenderOperation>();
        }

        public void End()
        {
            // Disables
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);

            // Enables
            GL.Enable(EnableCap.Texture2D);

            // Texture Env
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);

            // Cull for better performance
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Cw);

            // Setup 2D mode
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.Ortho(0, Vetr.Instance.Window.Width, Vetr.Instance.Window.Height, 0, 0, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            // Render each operation
            foreach(RenderOperation operation in _operations)
                operation.Render();

            // Restore matrices
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            
            // Restore state
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);

            _operations.Clear();
            _operations = null;
        }

        public void Draw(Texture2D texture, Vector2 position)
        {
            Draw(texture, position, new Vector2(texture.Width, texture.Height));
        }

        public void Draw(Texture2D texture, Vector2 position, Vector2 size)
        {
 	        Draw(texture, position, size, Color.White);
        }

        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            _operations.Add(new Texture2DRenderOperation(texture, position, new Vector2(texture.Width, texture.Height), color));
        }

        public void Draw(Texture2D texture, Vector2 position, Vector2 size, Color color)
        {
            _operations.Add(new Texture2DRenderOperation(texture, position, size, color));
        }
    }
}
