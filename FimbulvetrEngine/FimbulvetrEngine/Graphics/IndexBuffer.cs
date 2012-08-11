using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FimbulvetrEngine.Graphics
{
    public class IndexBuffer : GraphicResource
    {
        public int Id { get; private set; }
        public DrawElementsType Type { get; set; }
        public int Count { get; private set; }

        public IndexBuffer(DrawElementsType type)
        {
            int id;
            GL.GenBuffers(1, out id);

            Type = type;
            Id = id;
        }

        protected override void GCUnmanagedFinalize()
        {
            if (Id != 0)
            {
                int id = Id;
                GL.DeleteBuffers(1, ref id);
            }
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Id);
        }

        public void SetData<T>(T[] data, BufferUsageHint hint) where T : struct
        {
            Count = data.Length;

            Bind();
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(data.Length * GetSizeInBytes()), data, hint);
        }

        public int GetSizeInBytes()
        {
            switch (Type)
            {
                case DrawElementsType.UnsignedByte:
                    return 1;
                case DrawElementsType.UnsignedShort:
                    return 2;
                case DrawElementsType.UnsignedInt:
                    return 4;
            }

            return 0;
        }
    }
}
