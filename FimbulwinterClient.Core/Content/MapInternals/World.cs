using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using FimbulwinterClient.Extensions;
using OpenTK;

namespace FimbulwinterClient.Core.Content.MapInternals
{
    public class World
    {
        public struct Water
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

        public struct Light
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

        public struct Ground
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

        public struct ModelObject
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

            /*private GravityModel _model;
            public GravityModel Model
            {
                get { return _model; }
            }*/

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

                //_model = SharedInformation.ContentManager.Load<GravityModel>(@"data\model\" + ModelName);
            }

            /*public void Draw(Effect effect, GameTime gameTime)
            {
                if (_model == null)
                    return;

                Matrix world = Matrix.CreateTranslation(Position.X, Position.Y, Position.Z);
                world *= Matrix.CreateRotationZ(-Rotation.Z);
                world *= Matrix.CreateRotationX(-Rotation.X);
                world *= Matrix.CreateRotationY(Rotation.Y);
                world *= Matrix.CreateScale(Scale.X, Scale.Y, Scale.Z);
                world *= Matrix.CreateTranslation(-_model.realbbrange.X, _model.realbbrange.Y, -_model.realbbrange.Z);

                _model.Draw(world, effect, gameTime);
            }*/
        }

        public string IniFile { get; private set; }
        public string GroundFile { get; private set; }
        public string AltitudeFile { get; private set; }
        public string ScrFile { get; private set; }
        public Water WaterInfo { get; private set; }
        public Light LightInfo { get; private set; }
        public Ground GroundInfo { get; private set; }
        private List<ModelObject> Models { get; set; }
        public int WaterIndex { get; private set; }
        public Map Owner { get; private set; }

        protected byte MinorVersion;
        protected byte MajorVersion;

        private static Texture2D[][] _waterTextures;
        public static Texture2D[][] WaterTextures
        {
            get { return _waterTextures; }
        }

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

            WaterInfo.Load(br, MajorVersion, MinorVersion);
            LightInfo.Load(br, MajorVersion, MinorVersion);
            GroundInfo.Load(br, MajorVersion, MinorVersion);

            Models = new List<ModelObject>();

            //Logger.TabLevel++;

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

            //Logger.TabLevel--;

            //Logger.WriteLine("Loading water type {0} textures...", WaterInfo.Type);
            SetupWater(WaterInfo.Type);

            //Logger.WriteLine("Creating water vertex buffer...");
            //UpdateWaterVertices(Owner);

            //Logger.WriteLine("World v{0}.{1} status: {2} objects - {3} models", MajorVersion, MinorVersion, count, Models.Count);
            //Logger.WriteLine("    Water height: {0}", WaterInfo.Level);
            //Logger.WriteLine("    Water type: {0}", WaterInfo.Type);

            return true;
        }

        #region Water

        private void SetupWater(int i)
        {
            if (_waterTextures == null)
            {
                _waterTextures = new Texture2D[8][];
            }

            if (_waterTextures[i] == null)
            {
                _waterTextures[i] = new Texture2D[32];

                for (int j = 0; j < 32; j++)
                {
                    string sj = j.ToString(CultureInfo.InvariantCulture);

                    if (sj.Length == 1)
                        sj = "0" + sj;

                    _waterTextures[i][j] = ContentManager.Instance.Load<Texture2D>(string.Format(@"data\texture\¿öÅÍ\water{0}{1}.jpg", i, sj));
                }
            }
        }

        /*private void UpdateWaterVertices(Map Owner)
        {
            VertexPositionNormalTexture[] vertexdata = new VertexPositionNormalTexture[4];
            short[] indexdata = new short[4];

            float xmax = Owner.Ground.Zoom * Owner.Ground.Width;
            float ymax = Owner.Ground.Zoom * Owner.Ground.Height;

            Vector3[] position = new Vector3[4];
            Vector2[] tex = new Vector2[4];

            float x0 = (-Owner.Ground.Width / 2) * Owner.Ground.Zoom;
            float x1 = ((Owner.Ground.Width - 1) - Owner.Ground.Width / 2 + 1) * Owner.Ground.Zoom;

            float z0 = (-Owner.Ground.Height / 2) * Owner.Ground.Zoom;
            float z1 = ((Owner.Ground.Height - 1) - Owner.Ground.Height / 2 + 1) * Owner.Ground.Zoom;

            position[0] = new Vector3(x0, WaterInfo.Level, z0);
            position[1] = new Vector3(x1, WaterInfo.Level, z0);
            position[2] = new Vector3(x0, WaterInfo.Level, z1);
            position[3] = new Vector3(x1, WaterInfo.Level, z1);

            tex[0] = new Vector2(0, 0);
            tex[1] = new Vector2(Owner.Ground.Width / 8, 0);
            tex[2] = new Vector2(0, Owner.Ground.Height / 8);
            tex[3] = new Vector2(Owner.Ground.Width / 8, Owner.Ground.Height / 8);

            vertexdata[0] = new VertexPositionNormalTexture(position[0], new Vector3(1.0F), tex[0]);
            vertexdata[1] = new VertexPositionNormalTexture(position[1], new Vector3(1.0F), tex[1]);
            vertexdata[2] = new VertexPositionNormalTexture(position[2], new Vector3(1.0F), tex[2]);
            vertexdata[3] = new VertexPositionNormalTexture(position[3], new Vector3(1.0F), tex[3]);

            indexdata[0] = 0;
            indexdata[1] = 1;
            indexdata[2] = 2;
            indexdata[3] = 3;

            _vertices = new VertexBuffer(_graphicsDevice, typeof(VertexPositionNormalTexture), 4, BufferUsage.WriteOnly);
            _vertices.SetData(vertexdata);

            _indexes = new IndexBuffer(_graphicsDevice, typeof(short), 4, BufferUsage.WriteOnly);
            _indexes.SetData(indexdata);
        }*/

        private double _totalElapsed;
        public void UpdateWater(double elapsed)
        {
            _totalElapsed += elapsed;

            if (_totalElapsed >= 50.0F)
            {
                WaterIndex++;
                WaterIndex %= _waterTextures[WaterInfo.Type].Length;
                _totalElapsed -= 50.0F;
            }
        }

        /*public void DrawWater(Effect effect)
        {
            effect.Parameters["Texture"].SetValue(_waterTextures[WaterInfo.Type][WaterIndex]);

            _graphicsDevice.SetVertexBuffer(_vertices);
            _graphicsDevice.Indices = _indexes;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, _vertices.VertexCount, 0, _indexes.IndexCount / 2);
        }*/

        #endregion

        /*GameTime _gameTime;
        public void UpdateModels(GameTime gametime)
        {
            _gameTime = gametime;
        }

        public void DrawModels(Effect effect)
        {
            for (int i = 0; i < Models.Count; i++)
                Models[i].Draw(effect, _gameTime);
        }*/
    }
}
