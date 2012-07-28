using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FimbulwinterClient.Content.MapInternals
{
    public struct VertexPositionTextureNormalLightmap : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;
        public Vector2 Lightmap;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(32, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }

        public VertexPositionTextureNormalLightmap(Vector3 position, Vector3 normal, Vector2 texture, Vector2 lightmap)
        {
            Position = position;
            Normal = normal;
            Texture = texture;
            Lightmap = lightmap;
        }
    }
}
