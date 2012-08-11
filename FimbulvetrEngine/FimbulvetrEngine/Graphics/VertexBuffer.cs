using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FimbulvetrEngine.Graphics
{
    public class VertexBuffer : GraphicResource
    {
        public int Id { get; private set; }
        public VertexDeclaration Declaration { get; private set; }
        public int Count { get; private set; }

        public VertexBuffer(VertexDeclaration declaration)
        {
            int id;
            GL.GenBuffers(1, out id);

            Declaration = declaration;
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
            GL.BindBuffer(BufferTarget.ArrayBuffer, Id);

            Declaration.Activate();
        }

        public void SetData<T>(T[] data, BufferUsageHint hint) where T : struct
        {
            int size = BlittableValueType.StrideOf(data) * data.Length;

            if (size != data.Length * Declaration.GetTotalSize())
                throw new Exception("This data doesn't fit the VertexDeclaration of this buffer.");

            Count = data.Length;

            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(size), data, hint);
        }

        public void Render(BeginMode mode, IndexBuffer indexBuffer, int count)
        {
            indexBuffer.Bind();

            GL.DrawElements(mode, indexBuffer.Count, indexBuffer.Type, IntPtr.Zero);
        }
    }
}
