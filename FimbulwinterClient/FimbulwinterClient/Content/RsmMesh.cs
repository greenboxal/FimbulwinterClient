using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using BoundingBox = ROFormats.Model.BoundingBox;

namespace FimbulwinterClient.Content
{
    public class RsmMesh
    {
        private BoundingBox boundingBox;
        public BoundingBox BoundingBox
        {
            get { return boundingBox; }
        }

        private string name;
        public string Name
        {
            get { return name; }
        }

        private RsmBone parentBone;
        public RsmBone ParentBone
        {
            get { return parentBone; }
        }

        private RsmMeshPart[] meshParts;
        public RsmMeshPart[] MeshParts
        {
            get { return meshParts; }
        }

        private Effect[] effects;
        public Effect[] Effects
        {
            get { return effects; }
        }

        private VertexBuffer vertexBuffer;
        public VertexBuffer VertexBuffer
        {
            get { return vertexBuffer; }
        }

        private GraphicsDevice graphicsDevice;
        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
        }

        public RsmMesh(GraphicsDevice gd, RsmBone bone, ROFormats.Model mdl, ROFormats.Model.Node node, Texture2D[] textures)
        {
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[node.Faces.Length * 3];
            List<short>[] indices = new List<short>[node.Textures.Length];

            boundingBox = node.GetBoundingBox();
            vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionNormalTexture), node.Faces.Length * 3, BufferUsage.None);
            meshParts = new RsmMeshPart[node.Textures.Length];
            effects = new Effect[node.Textures.Length];
            parentBone = bone;
            name = node.Name;
            
            for (int i = 0; i < meshParts.Length; i++)
            {
                RsmMeshPart part = new RsmMeshPart();
                BasicEffect eff = new BasicEffect(gd);

                eff.Texture = textures[node.Textures[i]];
                eff.TextureEnabled = true;

                part.Effect = eff;

                meshParts[i] = part;
                effects[i] = eff;
                indices[i] = new List<short>();
            }

            for (int i = 0; i < node.Faces.Length; i++)
            {
                Vector3[] triangle = new Vector3[3];

                for (int n = 0; n < 3; n++)
                {
                    triangle[n] = new Vector3(
                                    node.Vertices[node.Faces[i].VertexID[n]].X,
                                    node.Vertices[node.Faces[i].VertexID[n]].Y,
                                    node.Vertices[node.Faces[i].VertexID[n]].Z
                                );
                }

                Vector3 normal = Vector3.Normalize(Vector3.Cross(triangle[2] - triangle[0], triangle[1] - triangle[0]));

                for (int n = 0; n < 3; n++)
                {
                    vertices[i * 3 + n] = new VertexPositionNormalTexture(
                        triangle[n],
                        normal,
                        new Vector2(
                            node.TVertices[node.Faces[i].TVertexID[n]].U,
                            node.TVertices[node.Faces[i].TVertexID[n]].V
                            )
                    );

                    indices[node.Faces[i].TexID].Add((short)(i * 3 + n));
                }
            }

            for (int i = 0; i < meshParts.Length; i++)
            {
                meshParts[i].SetData(gd, indices[i].ToArray());
            }

            vertexBuffer.SetData(vertices);
            graphicsDevice = gd;
        }

        public void Draw()
        {
            graphicsDevice.SetVertexBuffer(vertexBuffer);

            for (int i = 0; i < meshParts.Length; i++)
            {
                foreach (EffectPass pass in meshParts[i].Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                graphicsDevice.Indices = meshParts[i].IndexBuffer;
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, meshParts[i].IndexBuffer.IndexCount / 3);
            }
        }
    }
}
