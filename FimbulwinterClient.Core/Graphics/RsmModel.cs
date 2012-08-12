using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using FimbulwinterClient.Extensions;
using OpenTK;

namespace FimbulwinterClient.Core.Graphics
{
    public class RsmModel
    {
        public enum ShadeType
        {
            No,
            Flat,
            Smooth,
            Black
        }

        public int AnimationLength { get; private set; }
        public ShadeType Shade { get; private set; }
        public int Alpha { get; private set; }
        public Texture2D[] Textures { get; private set; }
        public string MainNodeName { get; private set; }
        public RsmMesh[] Meshes { get; private set; }
        public RsmMesh RootMesh { get; private set; }

        protected byte MinorVersion;
        protected byte MajorVersion;

        public bool Load(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);

            string header = ((char)br.ReadByte()).ToString() + ((char)br.ReadByte()) + ((char)br.ReadByte()) + ((char)br.ReadByte());

            if (header != "GRSM")
                return false;

            MajorVersion = br.ReadByte();
            MinorVersion = br.ReadByte();

            AnimationLength = br.ReadInt32();
            Shade = (ShadeType)br.ReadInt32();

            if (MajorVersion > 1 || (MajorVersion == 1 && MinorVersion >= 4))
                Alpha = br.ReadByte();
            else
                Alpha = 255;

            br.ReadBytes(16);

            Textures = new Texture2D[br.ReadInt32()];
            for (int i = 0; i < Textures.Length; i++)
            {
                Textures[i] = ContentManager.Instance.Load<Texture2D>(@"data\texture\" + br.ReadCString(40));
            }

            MainNodeName = br.ReadCString(40);

            Meshes = new RsmMesh[br.ReadInt32()];
            for (int i = 0; i < Meshes.Length; i++)
            {
                RsmMesh mesh = new RsmMesh();

                mesh.Load(this, br, MajorVersion, MinorVersion);

                Meshes[i] = mesh;
            }
            
            RootMesh = FindMesh(MainNodeName);
            RootMesh.CreateChildren(Meshes);

            bbmin = new Vector3(999999, 999999, 999999);
            bbmax = new Vector3(-999999, -999999, -999999);

            RootMesh.SetBoundingBox(ref bbmin, ref bbmax);
            bbrange = (bbmin + bbmax) / 2.0F;

            Matrix4 mat = Matrix4.Scale(1, -1, 1);
            realbbmin = new Vector3(999999, 999999, 999999);
            realbbmax = new Vector3(-999999, -999999, -999999);
            RootMesh.SetBoundingBox2(mat, ref realbbmin, ref realbbmax);
            realbbrange = (realbbmax + realbbmin) / 2.0F;
            //maxrange = Math.Max(Math.Max(Math.Max(realbbmax.X, -realbbmin.X), Math.Max(realbbmax.Y, -realbbmin.Y)), Math.Max(realbbmax.Z, -realbbmin.Z))));

            return true;
        }

        public RsmMesh FindMesh(string name)
        {
            return Meshes.FirstOrDefault(t => String.Compare(t.Name, name, true) == 0);
        }

        public Vector3 realbbmin, realbbmax, realbbrange;
        Vector3 bbmin, bbmax, bbrange;

    }
}
