using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.Core.Assets.MapInternals
{
    public class World
    {
        public struct Water
        {
            private float _level;
            public float Level
            {
                get { return _level; }
            }

            private int _type;
            public int Type
            {
                get { return _type; }
            }

            private float _waveHeight;
            public float WaveHeight
            {
                get { return _waveHeight; }
            }

            private float _waveSpeed;
            public float WaveSpeed
            {
                get { return _waveSpeed; }
            }

            private float _wavePitch;
            public float WavePitch
            {
                get { return _wavePitch; }
            }

            private int _animSpeed;
            public int AnimSpeed
            {
                get { return _animSpeed; }
            }

            public void Load(BinaryReader br, int major, int minor)
            {
                if (major >= 2 || (major == 1 && minor >= 3))
                {
                    _level = br.ReadSingle();
                }
                else
                {
                    _level = 0.0F;
                }

                if (major >= 2 || (major == 1 && minor >= 8))
                {
                    _type = br.ReadInt32();
                    _waveHeight = br.ReadSingle();
                    _waveSpeed = br.ReadSingle();
                    _wavePitch = br.ReadSingle();
                }
                else
                {
                    _type = 0;
                    _waveHeight = 1.0F;
                    _waveSpeed = 2.0F;
                    _wavePitch = 50.0F;
                }

                if (major >= 2 || (major == 1 && minor >= 9))
                {
                    _animSpeed = br.ReadInt32();
                }
                else
                {
                    _animSpeed = 3;
                }
            }
        }

        public struct Light
        {
            private int _longitude;
            public int Longitude
            {
                get { return _longitude; }
            }

            private int _latitude;
            public int Latitude
            {
                get { return _latitude; }
            }

            private Vector3 _diffuse;
            public Vector3 Diffuse
            {
                get { return _diffuse; }
            }

            private Vector3 _ambient;
            public Vector3 Ambient
            {
                get { return _ambient; }
            }

            private Vector3 _position;
            public Vector3 Position
            {
                get { return _position; }
            }

            public void Load(BinaryReader br, int major, int minor)
            {
                if (major >= 2 || (major == 1 && minor >= 5))
                {
                    _longitude = br.ReadInt32();
                    _latitude = br.ReadInt32();

                    float r, g, b;

                    r = br.ReadSingle();
                    g = br.ReadSingle();
                    b = br.ReadSingle();

                    _diffuse = new Vector3(r, g, b);

                    r = br.ReadSingle();
                    g = br.ReadSingle();
                    b = br.ReadSingle();

                    _ambient = new Vector3(r, g, b);
                }
                else
                {
                    _longitude = 45;
                    _latitude = 45;

                    _diffuse = new Vector3(1.0F, 1.0F, 1.0F);
                    _ambient = new Vector3(0.3F, 0.3F, 0.3F);
                }

                if (major >= 2 || (major == 1 && minor >= 7))
                {
                    br.ReadSingle();
                }

                float x = (float)Math.Cos((_longitude + 90) * MathHelper.Pi / 180.0) * (float)Math.Cos((90 - _latitude) * MathHelper.Pi / 180.0);
                float z = (float)Math.Sin((_longitude + 90) * MathHelper.Pi / 180.0) * (float)Math.Sin((90 - _latitude) * MathHelper.Pi / 180.0);
                float y = (float)Math.Cos((90 - _latitude) * MathHelper.Pi / 180.0);

                _position = new Vector3(x, y, z);
            }
        }

        public struct Ground
        {
            private int[] _square;
            public int[] Square
            {
                get { return _square; }
            }

            public int Top
            {
                get { return _square[0]; }
            }

            public int Bottom
            {
                get { return _square[1]; }
            }

            public int Left
            {
                get { return _square[2]; }
            }

            public int Right
            {
                get { return _square[3]; }
            }

            public void Load(BinaryReader br, int major, int minor)
            {
                _square = new int[4];

                if (major >= 2 || (major == 1 && minor >= 6))
                {
                    _square[0] = br.ReadInt32();
                    _square[1] = br.ReadInt32();
                    _square[2] = br.ReadInt32();
                    _square[3] = br.ReadInt32();
                }
                else
                {
                    _square[0] = -500;
                    _square[1] = 500;
                    _square[2] = -500;
                    _square[3] = 500;
                }
            }
        }

        public struct ModelObject
        {
            private string _name;
            public string Name
            {
                get { return _name; }
            }

            private int _animationType;
            public int AnimationType
            {
                get { return _animationType; }
            }

            private float _animationSpeed;
            public float AnimationSpeed
            {
                get { return _animationSpeed; }
            }

            private int _blockType;
            public int BlockType
            {
                get { return _blockType; }
            }

            private string _modelName;
            public string ModelName
            {
                get { return _modelName; }
            }

            private string _nodeName;
            public string NodeName
            {
                get { return _nodeName; }
            }

            private Vector3 _position;
            public Vector3 Position
            {
                get { return _position; }
                set { _position = value; }
            }

            private Vector3 _rotation;
            public Vector3 Rotation
            {
                get { return _rotation; }
            }

            private Vector3 _scale;
            public Vector3 Scale
            {
                get { return _scale; }
            }

            private GravityModel _model;
            public GravityModel Model
            {
                get { return _model; }
            }

            private GravityModelMesh _mesh;
            public GravityModelMesh Mesh
            {
                get { return _mesh; }
            }

            private Matrix _world;
            public Matrix World
            {
                get { return _world; }
            }

            public void Load(BinaryReader br, int major, int minor, MapInternals.Ground gnd)
            {
                if ((major == 1 && minor >= 3) || major > 1)
                {
                    _name = br.ReadCString(40);
                    _animationType = br.ReadInt32();
                    _animationSpeed = br.ReadSingle();

                    if (_animationSpeed < 0.0F || _animationSpeed >= 100.0F)
                        _animationSpeed = 1.0F;

                    _blockType = br.ReadInt32();
                }

                _modelName = br.ReadCString(80);
                _nodeName = br.ReadCString(80);

                _position.X = br.ReadSingle();
                _position.Y = br.ReadSingle();
                _position.Z = br.ReadSingle();

                _rotation.X = br.ReadSingle();
                _rotation.Y = br.ReadSingle();
                _rotation.Z = br.ReadSingle();

                _scale.X = br.ReadSingle();
                _scale.Y = br.ReadSingle();
                _scale.Z = br.ReadSingle();

                _model = SharedInformation.ContentManager.Load<GravityModel>(@"data\model\" + _modelName);
                _mesh = _model.FindMesh(_nodeName);

                _world = Matrix.CreateTranslation(_model.realbbrange.X, _model.realbbrange.Y, _model.realbbrange.Z);
                _world *= Matrix.CreateScale(_scale.X, _scale.Y, _scale.Z);
                _world *= Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y));
                _world *= Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.X));
                _world *= Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation.Z));
                _world *= Matrix.CreateTranslation(_position.X, _position.Y, _position.Z);
            }

            public void Draw(Effect effect, GameTime gameTime)
            {
                if (_mesh == null)
                    return;

                _mesh.Draw(_world, effect, gameTime);
            }
        }

        private string _iniFile;
        public string IniFile
        {
            get { return _iniFile; }
        }

        private string _groundFile;
        public string GroundFile
        {
            get { return _groundFile; }
        }

        private string _altitudeFile;
        public string AltitudeFile
        {
            get { return _altitudeFile; }
        }

        private string _scrFile;
        public string ScrFile
        {
            get { return _scrFile; }
        }

        private Water _waterInfo;
        public Water WaterInfo
        {
            get { return _waterInfo; }
        }

        private Light _lightInfo;
        public Light LightInfo
        {
            get { return _lightInfo; }
        }

        private Ground _groundInfo;
        public Ground GroundInfo
        {
            get { return _groundInfo; }
        }

        private List<ModelObject> _models;
        private List<ModelObject> Models
        {
            get { return _models; }
        }

        private VertexBuffer _vertices;
        public VertexBuffer Vertices
        {
            get { return _vertices; }
        }

        private IndexBuffer _indexes;
        public IndexBuffer Indexes
        {
            get { return _indexes; }
        }

        private GraphicsDevice _graphicsDevice;
        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        private int _waterIndex;
        public int WaterIndex
        {
            get { return _waterIndex; }
        }

        protected byte minorVersion;
        protected byte majorVersion;

        private static Texture2D[][] _waterTextures;
        public static Texture2D[][] WaterTextures
        {
            get { return _waterTextures; }
        }

        public World(GraphicsDevice gd)
        {
            _graphicsDevice = gd;
        }

        public bool Load(Stream rsw, Map owner)
        {
            BinaryReader br = new BinaryReader(rsw);
            string header = ((char)br.ReadByte()).ToString() + ((char)br.ReadByte()) + ((char)br.ReadByte()) + ((char)br.ReadByte());

            if (header != "GRSW")
                return false;

            majorVersion = br.ReadByte();
            minorVersion = br.ReadByte();

            if (!(majorVersion == 1 && minorVersion >= 2 && minorVersion <= 9) &&
                !(majorVersion == 2 && minorVersion <= 2))
                return false;

            _iniFile = br.ReadCString(40);
            _groundFile = br.ReadCString(40);
            _altitudeFile = br.ReadCString(40);
            _scrFile = br.ReadCString(40);

            _waterInfo.Load(br, majorVersion, minorVersion);
            _lightInfo.Load(br, majorVersion, minorVersion);
            _groundInfo.Load(br, majorVersion, minorVersion);

            _models = new List<ModelObject>();

            Logger.TabLevel++;

            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int type = br.ReadInt32();
                int skipSize = 0;

                if (type == 1)
                {
                    ModelObject mo = new ModelObject();

                    mo.Load(br, majorVersion, minorVersion, owner.Ground);

                    Vector3 tmp = mo.Position;
                    tmp.X = (mo.Position.X / 5) + owner.Ground.Width;
                    tmp.Z = (mo.Position.Z / 5) + owner.Ground.Height;
                    mo.Position = tmp;

                    _models.Add(mo);
                }
                else if (type == 2)
                {
                    skipSize = 80 + sizeof(float) * 3 + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(float);
                }
                else if (type == 3)
                {
                    skipSize = 80 + 80 + sizeof(float) * 3 + sizeof(float) + sizeof(int) + sizeof(int) + sizeof(float);

                    if (majorVersion >= 2)
                        skipSize += sizeof(float);
                }
                else if (type == 4)
                {
                    skipSize = 80 + sizeof(float) * 3 + sizeof(int) + sizeof(float) + sizeof(float) * 4;
                }

                if (skipSize > 0)
                    br.ReadBytes(skipSize);
            }

            Logger.TabLevel--;

            Logger.WriteLine("Loading water type {0} textures...", _waterInfo.Type);
            SetupWater(_waterInfo.Type);

            Logger.WriteLine("Creating water vertex buffer...");
            UpdateWaterVertices(owner);

            Logger.WriteLine("World v{0}.{1} status: {2} objects - {3} models", majorVersion, minorVersion, count, _models.Count);
            Logger.WriteLine("    Water height: {0}", _waterInfo.Level);
            Logger.WriteLine("    Water type: {0}", _waterInfo.Type);

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
                    string sj = j.ToString();

                    if (sj.Length == 1)
                        sj = "0" + sj;

                    _waterTextures[i][j] = SharedInformation.ContentManager.Load<Texture2D>(string.Format(@"data\texture\¿öÅÍ\water{0}{1}.jpg", i, sj));
                }
            }
        }

        private void UpdateWaterVertices(Map owner)
        {
            VertexPositionNormalTexture[] vertexdata = new VertexPositionNormalTexture[4];
            short[] indexdata = new short[4];

            float xmax = owner.Ground.Zoom * owner.Ground.Width;
            float ymax = owner.Ground.Zoom * owner.Ground.Height;

            Vector3[] position = new Vector3[4];
            Vector2[] tex = new Vector2[4];

            float x0 = (-owner.Ground.Width / 2) * owner.Ground.Zoom;
            float x1 = ((owner.Ground.Width - 1) - owner.Ground.Width / 2 + 1) * owner.Ground.Zoom;

            float z0 = (-owner.Ground.Height / 2) * owner.Ground.Zoom;
            float z1 = ((owner.Ground.Height - 1) - owner.Ground.Height / 2 + 1) * owner.Ground.Zoom;

            position[0] = new Vector3(x0, _waterInfo.Level, z0);
            position[1] = new Vector3(x1, _waterInfo.Level, z0);
            position[2] = new Vector3(x0, _waterInfo.Level, z1);
            position[3] = new Vector3(x1, _waterInfo.Level, z1);

            tex[0] = new Vector2(0, 0);
            tex[1] = new Vector2(owner.Ground.Width / 8, 0);
            tex[2] = new Vector2(0, owner.Ground.Height / 8);
            tex[3] = new Vector2(owner.Ground.Width / 8, owner.Ground.Height / 8);

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
        }

        private float _totalElapsed;
        public void UpdateWater(GameTime gametime)
        {
            _totalElapsed += (float)gametime.ElapsedGameTime.TotalMilliseconds;

            if (_totalElapsed >= 50.0F)
            {
                _waterIndex++;
                _waterIndex %= _waterTextures[_waterInfo.Type].Length;
                _totalElapsed -= 50.0F;
            }
        }

        public void DrawWater(Effect effect)
        {
            effect.Parameters["Texture"].SetValue(_waterTextures[_waterInfo.Type][_waterIndex]);

            _graphicsDevice.SetVertexBuffer(_vertices);
            _graphicsDevice.Indices = _indexes;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, _vertices.VertexCount, 0, _indexes.IndexCount / 2);
        }

        #endregion

        GameTime _gameTime = new GameTime();
        public void UpdateModels(GameTime gametime)
        {
            _gameTime = gametime;
        }

        public void DrawModels(Effect effect)
        {
            for (int i = 0; i < _models.Count; i++)
                _models[i].Draw(effect, _gameTime);
        }
    }
}
