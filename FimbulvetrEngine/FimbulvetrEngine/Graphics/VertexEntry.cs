using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace FimbulvetrEngine.Graphics
{
    public struct VertexEntry
    {
        public int Position { get; private set; }
        public VertexEntryType Type { get; private set; }
        public VertexEntryKind Kind { get; private set; }
        public TextureUnit Unit { get; private set; }

        public VertexEntry(int position, VertexEntryType type, VertexEntryKind kind)
            : this(position, type, kind, TextureUnit.Texture0)
        {

        }

        public VertexEntry(int position, VertexEntryType type, VertexEntryKind kind, TextureUnit unit) 
            : this()
        {
            Position = position;
            Type = type;
            Kind = kind;
            Unit = unit;
        }

        public void Activate(int offset, int stride)
        {
            switch (Kind)
            {
                case VertexEntryKind.Position:
                    GL.VertexPointer(GetValueCount(), VertexPointerType.Float, stride, offset);
                    break;
                case VertexEntryKind.Normal:
                    GL.NormalPointer(NormalPointerType.Float, stride, offset);
                    break;
                case VertexEntryKind.Texture:
                    GL.ActiveTexture(Unit);
                    GL.TexCoordPointer(GetValueCount(), TexCoordPointerType.Float, stride, offset);
                    break;
            }
        }

        public int GetSizeInBytes()
        {
            switch (Type)
            {
                case VertexEntryType.Vector2:
                    return 8;
                case VertexEntryType.Vector3:
                    return 12;
                case VertexEntryType.Vector4:
                    return 16;
            }

            return 0;
        }

        public int GetValueCount()
        {
            switch (Type)
            {
                case VertexEntryType.Vector2:
                    return 2;
                case VertexEntryType.Vector3:
                    return 3;
                case VertexEntryType.Vector4:
                    return 4;
            }

            return 0;
        }
    }

    public enum VertexEntryType
    {
        Vector2,
        Vector3,
        Vector4
    }

    public enum VertexEntryKind
    {
        Position,
        Normal,
        Texture
    }
}
