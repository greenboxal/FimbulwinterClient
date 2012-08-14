using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using FimbulwinterClient.Core.Graphics;
using FimbulwinterClient.Extensions;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FimbulwinterClient.Core.Content.MapInternals
{
    public class World
    {
        public class Water
        {
            public float Level { get; private set; }
            public int Type { get; private set; }
            public float WaveHeight { get; private set; }
            public float WaveSpeed { get; private set; }
            public float WavePitch { get; private set; }
            public int AnimSpeed { get; private set; }

            public void Load(BinaryReader br, int major, int minor)
            {
                if (major >= 2 || (major == 1 && minor >= 3))
                {
                    Level = br.ReadSingle();
                }
                else
                {
                    Level = 0.0F;
                }

                if (major >= 2 || (major == 1 && minor >= 8))
                {
                    Type = br.ReadInt32();
                    WaveHeight = br.ReadSingle();
                    WaveSpeed = br.ReadSingle();
                    WavePitch = br.ReadSingle();
                }
                else
                {
                    Type = 0;
                    WaveHeight = 1.0F;
                    WaveSpeed = 2.0F;
                    WavePitch = 50.0F;
                }

                if (major >= 2 || (major == 1 && minor >= 9))
                {
                    AnimSpeed = br.ReadInt32();
                }
                else
                {
                    AnimSpeed = 3;
                }
            }
        }

        public class Light
        {
            public int Longitude { get; private set; }
            public int Latitude { get; private set; }
            public Vector3 Diffuse { get; private set; }
            public Vector3 Ambient { get; private set; }
            public Vector3 Position { get; private set; }

            public void Load(BinaryReader br, int major, int minor)
            {
                if (major >= 2 || (major == 1 && minor >= 5))
                {
                    Longitude = br.ReadInt32();
                    Latitude = br.ReadInt32();

                    float r = br.ReadSingle();
                    float g = br.ReadSingle();
                    float b = br.ReadSingle();

                    Diffuse = new Vector3(r, g, b);

                    r = br.ReadSingle();
                    g = br.ReadSingle();
                    b = br.ReadSingle();

                    Ambient = new Vector3(r, g, b);
                }
                else
                {
                    Longitude = 45;
                    Latitude = 45;

                    Diffuse = new Vector3(1.0F, 1.0F, 1.0F);
                    Ambient = new Vector3(0.3F, 0.3F, 0.3F);
                }

                if (major >= 2 || (major == 1 && minor >= 7))
                {
                    br.ReadSingle();
                }

                float x = (float)Math.Cos((Longitude + 90) * MathHelper.Pi / 180.0) * (float)Math.Cos((90 - Latitude) * MathHelper.Pi / 180.0);
                float z = (float)Math.Sin((Longitude + 90) * MathHelper.Pi / 180.0) * (float)Math.Sin((90 - Latitude) * MathHelper.Pi / 180.0);
                float y = (float)Math.Cos((90 - Latitude) * MathHelper.Pi / 180.0);

                Position = new Vector3(x, y, z);
            }
        }

        public class Ground
        {
            public int[] Square { get; private set; }

            public int Top
            {
                get { return Square[0]; }
            }

            public int Bottom
            {
                get { return Square[1]; }
            }

            public int Left
            {
                get { return Square[2]; }
            }

            public int Right
            {
                get { return Square[3]; }
            }

            public void Load(BinaryReader br, int major, int minor)
            {
                Square = new int[4];

                if (major >= 2 || (major == 1 && minor >= 6))
                {
                    Square[0] = br.ReadInt32();
                    Square[1] = br.ReadInt32();
                    Square[2] = br.ReadInt32();
                    Square[3] = br.ReadInt32();
                }
                else
                {
                    Square[0] = -500;
                    Square[1] = 500;
                    Square[2] = -500;
                    Square[3] = 500;
                }
            }
        }

        public class ModelObject
        {
            private Vector3 _position;
            private Vector3 _rotation;
            private Vector3 _scale;

            public string Name { get; private set; }
            public int AnimationType { get; private set; }
            public float AnimationSpeed { get; private set; }
            public int BlockType { get; private set; }
            public string ModelName { get; private set; }
            public string NodeName { get; private set; }

            public Vector3 Position
            {
                get { return _position; }
                set { _position = value; }
            }

            public Vector3 Rotation
            {
                get { return _rotation; }
                private set { _rotation = value; }
            }

            public Vector3 Scale
            {
                get { return _scale; }
                private set { _scale = value; }
            }

            private RsmModel _model;
            public RsmModel Model
            {
                get { return _model; }
            }

            private RsmMesh _mesh;
            public RsmMesh Mesh
            {
                get { return _mesh; }
            }
            
            public void Load(BinaryReader br, int major, int minor, MapInternals.Ground gnd)
            {
                if ((major == 1 && minor >= 3) || major > 1)
                {
                    Name = br.ReadCString(40);
                    AnimationType = br.ReadInt32();
                    AnimationSpeed = br.ReadSingle();

                    if (AnimationSpeed < 0.0F || AnimationSpeed >= 100.0F)
                        AnimationSpeed = 1.0F;

                    BlockType = br.ReadInt32();
                }

                ModelName = br.ReadCString(80);
                NodeName = br.ReadCString(80);

                _position.X = br.ReadSingle();
                _position.Y = br.ReadSingle();
                _position.Z = br.ReadSingle();

                _rotation.X = br.ReadSingle();
                _rotation.Y = br.ReadSingle();
                _rotation.Z = br.ReadSingle();

                _scale.X = br.ReadSingle();
                _scale.Y = br.ReadSingle();
                _scale.Z = br.ReadSingle();
            }

            public void SetModel(RsmModel model)
            {
                _model = model;

                if (_model.Loaded)
                    _mesh = _model.FindMesh(NodeName) ?? _model.RootMesh;
            }

            public void Draw(MapInternals.Ground ground, CommonShaderProgram shader, double elapsed)
            {
                if (_model == null || !_model.Loaded)
                    return;

                if (_mesh == null)
                    _mesh = _model.FindMesh(NodeName) ?? _model.RootMesh;

                float offsetX = ground.Zoom * ground.Width / 2;
                float offsetZ = ground.Zoom * ground.Height / 2;

                GL.PushMatrix();

                GL.Translate(-offsetX + _position.X * 5, _position.Y, -offsetZ + _position.Z * 5);
                GL.Rotate(-Rotation.X, Vector3.UnitX);
                GL.Rotate(-Rotation.Z, Vector3.UnitZ);
                GL.Rotate(Rotation.Y, Vector3.UnitY);
                GL.Scale(_scale);

                _mesh.Draw(shader, elapsed);

                GL.PopMatrix();
            }
        }

        public string IniFile { get; private set; }
        public string GroundFile { get; private set; }
        public string AltitudeFile { get; private set; }
        public string ScrFile { get; private set; }
        public Water WaterInfo { get; private set; }
        public Light LightInfo { get; private set; }
        public Ground GroundInfo { get; private set; }
        public List<ModelObject> Models { get; private set; }
        public Map Owner { get; private set; }

        protected byte MinorVersion;
        protected byte MajorVersion;

        public World(Map owner)
        {
            Owner = owner;
        }

        public bool Load(Stream rsw)
        {
            BinaryReader br = new BinaryReader(rsw);
            string header = ((char)br.ReadByte()).ToString(CultureInfo.InvariantCulture) + ((char)br.ReadByte()) + ((char)br.ReadByte()) + ((char)br.ReadByte());

            if (header != "GRSW")
                return false;

            MajorVersion = br.ReadByte();
            MinorVersion = br.ReadByte();

            if (!(MajorVersion == 1 && MinorVersion >= 2 && MinorVersion <= 9) &&
                !(MajorVersion == 2 && MinorVersion <= 2))
                return false;

            IniFile = br.ReadCString(40);
            GroundFile = br.ReadCString(40);
            AltitudeFile = br.ReadCString(40);
            ScrFile = br.ReadCString(40);

            WaterInfo = new Water();
            WaterInfo.Load(br, MajorVersion, MinorVersion);

            LightInfo = new Light();
            LightInfo.Load(br, MajorVersion, MinorVersion);

            GroundInfo = new Ground();
            GroundInfo.Load(br, MajorVersion, MinorVersion);

            Models = new List<ModelObject>();

            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int type = br.ReadInt32();
                int skipSize = 0;

                if (type == 1)
                {
                    ModelObject mo = new ModelObject();

                    mo.Load(br, MajorVersion, MinorVersion, Owner.Ground);

                    Vector3 tmp = mo.Position;
                    tmp.X = (mo.Position.X / 5) + Owner.Ground.Width;
                    tmp.Z = (mo.Position.Z / 5) + Owner.Ground.Height;
                    mo.Position = tmp;

                    Models.Add(mo);
                }
                else if (type == 2)
                {
                    skipSize = 80 + sizeof(float) * 3 + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(float);
                }
                else if (type == 3)
                {
                    skipSize = 80 + 80 + sizeof(float) * 3 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(float);

                    if (MajorVersion >= 2)
                        skipSize += sizeof(float);
                }
                else if (type == 4)
                {
                    skipSize = 80 + sizeof(float) * 3 + sizeof(int) + sizeof(float) + sizeof(float) * 4;
                }

                if (skipSize > 0)
                    br.ReadBytes(skipSize);
            }

            return true;
        }
    }
}
