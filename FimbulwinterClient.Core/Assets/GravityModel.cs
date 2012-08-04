using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.Core.Assets
{
    public class GravityModel
    {
        public enum ShadeType
        {
            No,
            Flat,
            Smooth,
            Black
        }

        private int _animationLength;
        public int AnimationLength
        {
            get { return _animationLength; }
        }

        private ShadeType _shade;
        public ShadeType Shade
        {
            get { return _shade; }
        }

        private int _alpha;
        public int Alpha
        {
            get { return _alpha; }
        }

        private Texture2D[] _textures;
        public Texture2D[] Textures
        {
            get { return _textures; }
        }

        private string _mainNodeName;
        public string MainNodeName
        {
            get { return _mainNodeName; }
        }

        private GravityModelMesh[] _meshes;
        public GravityModelMesh[] Meshes
        {
            get { return _meshes; }
        }

        private GravityModelMesh _rootMesh;
        public GravityModelMesh RootMesh
        {
            get { return _rootMesh; }
        }

        protected byte minorVersion;
        protected byte majorVersion;

        public GravityModel()
        {

        }

        public bool Load(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);

            string header = ((char)br.ReadByte()).ToString() + ((char)br.ReadByte()) + ((char)br.ReadByte()) + ((char)br.ReadByte());

            if (header != "GRSM")
                return false;

            majorVersion = br.ReadByte();
            minorVersion = br.ReadByte();

            _animationLength = br.ReadInt32();
            _shade = (ShadeType)br.ReadInt32();

            if (majorVersion > 1 || (majorVersion == 1 && minorVersion >= 4))
                _alpha = br.ReadByte();
            else
                _alpha = 255;

            br.ReadBytes(16);

            _textures = new Texture2D[br.ReadInt32()];
            for (int i = 0; i < _textures.Length; i++)
            {
                _textures[i] = SharedInformation.ContentManager.Load<Texture2D>(@"data\texture\" + br.ReadCString(40));
            }

            _mainNodeName = br.ReadCString(40);

            _meshes = new GravityModelMesh[br.ReadInt32()];
            for (int i = 0; i < _meshes.Length; i++)
            {
                GravityModelMesh mesh = new GravityModelMesh();

                mesh.Load(this, br, majorVersion, minorVersion);

                _meshes[i] = mesh;
            }

            _rootMesh = FindMesh(_mainNodeName);
            _rootMesh.CreateChildren(_meshes);

            bbmin = new Vector3(999999, 999999, 999999);
            bbmax = new Vector3(-999999, -999999, -999999);

            _rootMesh.SetBoundingBox(ref bbmin, ref bbmax);
            bbrange = (bbmin + bbmax) / 2.0F;

            Matrix mat = Matrix.CreateScale(1, -1, 1);
            realbbmin = new Vector3(999999, 999999, 999999);
            realbbmax = new Vector3(-999999, -999999, -999999);
            _rootMesh.SetBoundingBox2(mat, ref realbbmin, ref realbbmax);
            realbbrange = (realbbmax + realbbmin) / 2.0F;
            //maxrange = Math.Max(Math.Max(Math.Max(realbbmax.X, -realbbmin.X), Math.Max(realbbmax.Y, -realbbmin.Y)), Math.Max(realbbmax.Z, -realbbmin.Z))));

            return true;
        }

        private GravityModelMesh FindMesh(string name)
        {
            for (int i = 0; i < _meshes.Length; i++)
            {
                if (String.Compare(_meshes[i].Name, name, true) == 0)
                    return _meshes[i];
            }

            return null;
        }

        public void Draw(Matrix world, Effect effect, GameTime gameTime)
        {
            _rootMesh.Draw(world, effect, gameTime);
        }

        float maxrange;
        public Vector3 realbbmin, realbbmax, realbbrange;
        Vector3 bbmin, bbmax, bbrange;
    }
}
