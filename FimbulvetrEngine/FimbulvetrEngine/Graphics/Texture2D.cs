using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace FimbulvetrEngine.Graphics
{
    public class Texture2D : GraphicResource
    {
        public int Texture { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Texture2D(int width, int height)
            : this(GL.GenTexture(), width, height)
        {
            Bind();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
        }

        public Texture2D(int texture, int width, int height)
        {
            Texture = texture;
            Width = width;
            Height = height;
        }

        protected override void GCUnmanagedFinalize()
        {
            if (Texture != 0)
                GL.DeleteTexture(Texture);
        }

        public void SetData(PixelFormat format, PixelInternalFormat iFormat, PixelType type, IntPtr data)
        {
            Bind();
            GL.TexImage2D(TextureTarget.Texture2D, 0, iFormat, Width, Height, 0, format, type, data);
        }

        public void SetData<T>(PixelFormat format, PixelInternalFormat iFormat, PixelType type, T[] data) where T : struct
        {
            Bind();
            GL.TexImage2D(TextureTarget.Texture2D, 0, iFormat, Width, Height, 0, format, type, data);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, Texture);
        }
    }
}
