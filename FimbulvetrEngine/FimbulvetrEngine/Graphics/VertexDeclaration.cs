using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace FimbulvetrEngine.Graphics
{
    public class VertexDeclaration
    {
        public VertexElement[] Declaration { get; private set; }

        public VertexDeclaration(params VertexElement[] entries)
        {
            Declaration = entries;
        }

        public void Activate()
        {
            int offset = 0;
            int stride = GetTotalSize();

            for (int i = 0; i < Declaration.Length; i++)
            {
                Declaration[i].Activate(offset, stride);
                offset += Declaration[i].GetSizeInBytes();
            }
        }

        public int GetTotalSize()
        {
            int size = 0;

            for (int i = 0; i < Declaration.Length; i++)
                size += Declaration[i].GetSizeInBytes();

            return size;
        }
    }
}
