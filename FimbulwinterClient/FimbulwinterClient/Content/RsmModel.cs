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
            {
                string fn = mdl.Textures[i].Name;

                Map.OnReportStatus("Loading texture {0}...", fn);

                textures[i] = contentManager.LoadContent<Texture2D>("data\\texture\\" + fn.Korean());
            }

            meshes = new RsmMesh[mdl.Nodes.Length];
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i] = new RsmMesh(graphicsDevice, mdl, mdl.Nodes[i], textures);
            }

            for (int i = 0; i < meshes.Length; i++)
            {
                List<RsmMesh> childs = new List<RsmMesh>();

                for (int n = 0; n < meshes.Length; n++)
                {
                    if (mdl.Nodes[n].ParentName == meshes[i].Name)
                    {
                        meshes[n].Parent = meshes[i];
                        childs.Add(meshes[n]);
                    }
                }

                meshes[i].Children = childs.ToArray();
            }

            for (int i = 0; i < meshes.Length; i++)
            {
                if (meshes[i].Parent == null)
                    meshes[i].Parent = meshes[i];
            }

            root = GetMesh(mdl.MainNode);
            root.Parent = null;

            return true;
        }

        private RsmMesh GetMesh(string p)
        {
            foreach (RsmMesh m in meshes)
                if (m.Name == p)
                    return m;

            return null;
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

            return m;// *mesh.Bone.Transform;
        }
    }
}
