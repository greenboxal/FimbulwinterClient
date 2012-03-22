using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace FimbulwinterClient.Content
{
    public class RsmMeshPart
    {
        private Effect effect;
        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        private IndexBuffer indexBuffer;
        public IndexBuffer IndexBuffer
        {
            get { return indexBuffer; }
        }

        public void SetData(GraphicsDevice gd, short[] vertices)
        {
            indexBuffer = new IndexBuffer(gd, typeof(short), vertices.Length, BufferUsage.None);
            indexBuffer.SetData(vertices);
        }
    }
}
