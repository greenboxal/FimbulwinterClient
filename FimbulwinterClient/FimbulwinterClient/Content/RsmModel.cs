using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using FimbulwinterClient.IO;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.Content
{
    public class RsmModel
    {
        private RsmMesh[] meshes;
        public RsmMesh[] Meshes
        {
            get { return meshes; }
        }

        private RsmBone[] bones;
        public RsmBone[] Bones
        {
            get { return bones; }
        }

        private RsmMesh root;
        public RsmMesh Root
        {
            get { return root; }
        }

        private GraphicsDevice graphicsDevice;
        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
        }

        private ROContentManager contentManager;
        public ROContentManager ContentManager
        {
            get { return contentManager; }
        }

        private Texture2D[] textures;
        public Texture2D[] Textures
        {
            get { return textures; }
            set { textures = value; }
        }

        private Matrix world;
        public Matrix World
        {
            get { return world; }
            set { world = value; }
        }

        private int frame;
        public int Frame
        {
            get { return frame; }
            set { frame = value; }
        }

        public RsmModel(GraphicsDevice gd, ROContentManager cm)
        {
            graphicsDevice = gd;
            contentManager = cm;
        }

        public bool Load(Stream s)
        {
            ROFormats.Model mdl = new ROFormats.Model();

            if (!mdl.Load(s))
                return false;

            textures = new Texture2D[mdl.Textures.Length];
            for (int i = 0; i < mdl.Textures.Length; i++)
                textures[i] = contentManager.LoadContent<Texture2D>("data/texture/" + mdl.Textures[i].Name);

            meshes = new RsmMesh[mdl.Nodes.Length];
            bones = new RsmBone[mdl.Nodes.Length];
            for (int i = 0; i < meshes.Length; i++)
            {
                bones[i] = new RsmBone(i, mdl.Nodes[i]);
                meshes[i] = new RsmMesh(graphicsDevice, bones[i], mdl, mdl.Nodes[i], textures);
            }

            for (int i = 0; i < bones.Length; i++)
            {
                List<RsmBone> childs = new List<RsmBone>();

                for (int n = 0; n < bones.Length; n++)
                {
                    if (mdl.Nodes[n].ParentName == bones[i].Name)
                    {
                        bones[n].Parent = bones[i];
                        childs.Add(bones[n]);
                    }
                }

                bones[i].Children = childs.ToArray();
            }

            for (int i = 0; i < bones.Length; i++)
            {
                if (bones[i].Parent == null)
                    bones[i].Parent = bones[i];
            }

            root = GetMesh(mdl.MainNode);

            return true;
        }

        private RsmMesh GetMesh(string p)
        {
            foreach (RsmMesh m in meshes)
                if (m.Name == p)
                    return m;

            return null;
        }

        public void CopyAbsoluteBoneTransformsTo(Matrix[] transforms)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                transforms[i] = GetMeshTransform(i);
            }
        }

        public Matrix GetMeshTransform(int id)
        {
            RsmMesh mesh = meshes[id];
            Matrix m = Matrix.Identity;

            m *= Matrix.CreateTranslation(mesh.Translation);

            if (mesh.IsMain)
            {
                if (mesh.IsOnly)
                {
                    m *= Matrix.CreateTranslation(0.0F, -mesh.BoundingBox.Max[1] + mesh.BoundingBox.Offset[1], 0.0F);
                }
                else
                {
                    m *= Matrix.CreateTranslation(-mesh.BoundingBox.Offset[0], -mesh.BoundingBox.Max[1], -mesh.BoundingBox.Offset[2]);
                }
            }
            else
            {
                m *= Matrix.CreateTranslation(mesh.Translation);
            }

            if (mesh.RotationFrames.Length == 0)
            {
                m *= Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(mesh.RotationAxis, mesh.RotationAngle));
            }
            else
            {
                int current = 0;
                int next = 0;
                float t = 0;
                Quaternion q;

                for (int i = 0; i < mesh.RotationFrames.Length; i++)
                {
                    if (frame < mesh.RotationFrames[i].Item2)
                    {
                        current = i - 1;
                        break;
                    }
                }

                next = current + 1;

                if (next == mesh.RotationFrames.Length)
                    next = 0;

                t = (frame - mesh.RotationFrames[current].Item2) / (float)(mesh.RotationFrames[next].Item2 - mesh.RotationFrames[current].Item2);

                q = Quaternion.Lerp(mesh.RotationFrames[current].Item1, mesh.RotationFrames[next].Item1, t);
                q.Normalize();

                m *= Matrix.CreateFromQuaternion(q);

                frame += mesh.RotationFrames[mesh.RotationFrames.Length - 1].Item2 / 100;
                if (frame >= mesh.RotationFrames[mesh.RotationFrames.Length - 1].Item2)
                    frame = 0;
            }

            if (mesh.IsMain && mesh.IsOnly)
            {
                m *= Matrix.CreateTranslation(-mesh.BoundingBox.Offset[0], -mesh.BoundingBox.Offset[1], -mesh.BoundingBox.Offset[2]);
            }
            
            if (!mesh.IsMain || !mesh.IsOnly)
            {
                m *= Matrix.CreateTranslation(mesh.OffsetMT[9], mesh.OffsetMT[10], mesh.OffsetMT[11]);
            }

            return m * mesh.Bone.Transform;
        }

        public void Draw(Vector3 pos)
        {
            Matrix m = Matrix.CreateTranslation(pos.X, -pos.Y, pos.Z);

            DrawMesh(root.Bone.Index, world * m);
        }

        private void DrawMesh(int id, Matrix parentMatrix)
        {
            RsmMesh mesh = meshes[id];
            Matrix trans = GetMeshTransform(id) * parentMatrix;

            foreach(BasicEffect eff in mesh.Effects)
            {
                eff.World = trans;
            }

            mesh.Draw();

            foreach (RsmBone b in mesh.Bone.Children)
                DrawMesh(b.Index, world);
        }

    }
}
