using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FimbulvetrEngine.Graphics
{
    public struct VertexPositionNormalTexture
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, TextureUnit.Texture0)
        );

        public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 texture)
        {
            Position = position;
            Normal = normal;
            Texture = texture;
        }
    }
}
