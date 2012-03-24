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

        private RsmBone bone;
        public RsmBone Bone
        {
            get { return bone; }
            set { bone = value; }
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

        private Vector3 scale;
        public Vector3 Scale
        {
            get { return scale; }
        }

        private float rotationAngle;
        public float RotationAngle
        {
            get { return rotationAngle; }
        }

        private Vector3 rotationAxis;
        public Vector3 RotationAxis
        {
            get { return rotationAxis; }
        }

        private Vector3 translation;
        public Vector3 Translation
        {
            get { return translation; }
        }

        private Tuple<Quaternion, int>[] rotationFrames;
        public Tuple<Quaternion, int>[] RotationFrames
        {
            get { return rotationFrames; }
        }

        private Vector3[] translationFrames;
        public Vector3[] TranslationFrames
        {
            get { return translationFrames; }
        }

        private bool isOnly;
        public bool IsOnly
        {
            get { return isOnly; }
        }

        private bool isMain;
        public bool IsMain
        {
            get { return isMain; }
        }

        private float[] offsetMT;
        public float[] OffsetMT
        {
            get { return offsetMT; }
        }

        public RsmMesh(GraphicsDevice gd, RsmBone self, ROFormats.Model mdl, ROFormats.Model.Node node, Texture2D[] textures)
        {
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[node.Faces.Length * 3];
            List<short>[] indices = new List<short>[node.Textures.Length];

            vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionNormalTexture), node.Faces.Length * 3, BufferUsage.None);
            meshParts = new RsmMeshPart[node.Textures.Length];
            effects = new Effect[node.Textures.Length];
            boundingBox = node.GetBoundingBox();
            name = node.Name;
            bone = self;

            isOnly = node.IsOnly;
            isMain = node.IsMain;

            scale = new Vector3(node.Scale.X, node.Scale.Y, node.Scale.Z);
            rotationAngle = node.RotAngle;
            rotationAxis = new Vector3(node.RotAxis.X, node.RotAxis.Y, node.RotAxis.Z);
            translation = new Vector3(node.Position.X, node.Position.Y, node.Position.Z);
            offsetMT = node.OffsetMT;

            for (int i = 0; i < meshParts.Length; i++)
            {
                RsmMeshPart part = new RsmMeshPart();
                BasicEffect eff = new BasicEffect(gd);

                eff.Texture = textures[node.Textures[i]];
                eff.TextureEnabled = true;
                eff.Alpha = mdl.Alpha / 255;
                eff.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
                eff.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                eff.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
                eff.SpecularPower = 5.0f;
                eff.LightingEnabled = true;

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

            rotationFrames = new Tuple<Quaternion, int>[node.RotKeyFrames.Length];
            for (int i = 0; i < rotationFrames.Length; i++)
                rotationFrames[i] = new Tuple<Quaternion, int>(new Quaternion(node.RotKeyFrames[i].QX, node.RotKeyFrames[i].QY, node.RotKeyFrames[i].QZ, node.RotKeyFrames[i].QW), node.RotKeyFrames[i].Frame);
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
