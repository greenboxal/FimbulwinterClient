using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace FimbulvetrEngine.Graphics
{
    public struct VertexElement
    {
        public VertexElementFormat Type { get; private set; }
        public VertexElementUsage Kind { get; private set; }
        public TextureUnit Unit { get; private set; }

        public VertexElement(VertexElementFormat type, VertexElementUsage kind)
            : this(type, kind, TextureUnit.Texture0)
        {

        }

        public VertexElement(VertexElementFormat type, VertexElementUsage kind, TextureUnit unit) 
            : this()
        {
            Type = type;
            Kind = kind;
            Unit = unit;
        }

        public void Activate(int offset, int stride)
        {
            switch (Kind)
            {
                case VertexElementUsage.Position:
                    GL.VertexPointer(GetValueCount(), VertexPointerType.Float, stride, offset);
                    break;
                case VertexElementUsage.Normal:
                    GL.NormalPointer(NormalPointerType.Float, stride, offset);
                    break;
                case VertexElementUsage.TextureCoordinate:
                    GL.ActiveTexture(Unit);
                    GL.TexCoordPointer(GetValueCount(), TexCoordPointerType.Float, stride, offset);
                    break;
            }
        }

        public int GetSizeInBytes()
        {
            switch (Type)
            {
                case VertexElementFormat.Vector2:
                    return 8;
                case VertexElementFormat.Vector3:
                    return 12;
                case VertexElementFormat.Vector4:
                    return 16;
            }

            return 0;
        }

        public int GetValueCount()
        {
            switch (Type)
            {
                case VertexElementFormat.Vector2:
                    return 2;
                case VertexElementFormat.Vector3:
                    return 3;
                case VertexElementFormat.Vector4:
                    return 4;
            }

            return 0;
        }
    }

    public enum VertexElementFormat
    {
        Vector2,
        Vector3,
        Vector4
    }

    public enum VertexElementUsage
    {
        Position,
        Normal,
        TextureCoordinate
    }
}
