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
            set { bones = value; }
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

            return true;
        }

        public void CopyAbsoluteBoneTransformsTo(Matrix[] transforms)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                transforms[i] = bones[i].Transform;
            }
        }
    }
}
