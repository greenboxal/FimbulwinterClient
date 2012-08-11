using System.Drawing;
using FimbulvetrEngine.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FimbulwinterClient.Core.Graphics
{
    public struct VertexPositionTextureNormalLightmap
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;
        public Vector2 Lightmap;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, TextureUnit.Texture0),
            new VertexElement(VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, TextureUnit.Texture1)
        );

        public VertexPositionTextureNormalLightmap(Vector3 position, Vector3 normal, Vector2 texture, Vector2 lightmap, Color color)
        {
            Position = position;
            Normal = normal;
            Texture = texture;
            Lightmap = lightmap;
        }
    }
}
