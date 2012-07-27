using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.Content
{
    public class Map
    {
        public class Ground
        {
            public class Lightmap
            {
                private byte[] _brightness;
                public byte[] Brightness
                {
                    get { return _brightness; }
                }

                private Color[] _intensity;
                public Color[] Intensity
                {
                    get { return _intensity; }
                }

                public void Load(BinaryReader br)
                {
                    _brightness = br.ReadBytes(8 * 8);

                    _intensity = new Color[8 * 8];
                    for (int i = 0; i < _intensity.Length; i++)
                    {
                        int r = br.ReadByte();
                        int g = br.ReadByte();
                        int b = br.ReadByte();

                        _intensity[i] = Color.FromNonPremultiplied(r, g, b, 0);
                    }
                }
            }

            public class Cell
            {
                private float[] _height;
                public float[] Height
                {
                    get { return _height; }
                }

                private int _tileUp;
                public int TileUp
                {
                    get { return _tileUp; }
                }

                private int _tileSide;
                public int TileSide
                {
                    get { return _tileSide; }
                }

                private int _tileOtherSide;
                public int TileOtherSide
                {
                    get { return _tileOtherSide; }
                }

                private bool _selected;
                public bool Selected
                {
                    get { return _selected; }
                }

                private bool _hasModelOnTop;
                public bool HasModelOnTop
                {
                    get { return _hasModelOnTop; }
                }

                private Vector3[] _normal;
                public Vector3[] Normal
                {
                    get { return _normal; }
                }

                public void Load(BinaryReader br)
                {
                    _height = new float[4];
                    _height[0] = br.ReadSingle();
                    _height[1] = br.ReadSingle();
                    _height[2] = br.ReadSingle();
                    _height[3] = br.ReadSingle();

                    _tileUp = br.ReadInt32();
                    _tileSide = br.ReadInt32();
                    _tileOtherSide = br.ReadInt32();
                }

                public void CalculateNormal()
                {
                    Vector3 b1 = default(Vector3);
                    Vector3 b2 = default(Vector3);

                    b1 = new Vector3(10, -_height[0], -10) - new Vector3(0, -_height[3], 0);
                    b2 = new Vector3(0, -_height[2], -10) - new Vector3(0, -_height[3], 0);

                    _normal = new Vector3[5];
                    _normal[0] = Vector3.Cross(b1, b2);
                    _normal[0].Normalize();
                }
            }

            public class Surface
            {
                private float[] _u;
                public float[] U
                {
                    get { return _u; }
                }

                private float[] _v;
                public float[] V
                {
                    get { return _v; }
                }

                private int _texture;
                public int Texture
                {
                    get { return _texture; }
                }

                private int _lightmap;
                public int Lightmap
                {
                    get { return _lightmap; }
                }

                private Color _color;
                public Color Color
                {
                    get { return _color; }
                }

                private bool _used;
                public bool Used
                {
                    get { return _used; }
                }

                public void Load(Ground owner, BinaryReader br)
                {
                    _u = new float[4];
                    _u[0] = br.ReadSingle();
                    _u[1] = br.ReadSingle();
                    _u[2] = br.ReadSingle();
                    _u[3] = br.ReadSingle();

                    _v = new float[4];
                    _v[0] = br.ReadSingle();
                    _v[1] = br.ReadSingle();
                    _v[2] = br.ReadSingle();
                    _v[3] = br.ReadSingle();

                    _texture = br.ReadInt16();
                    _lightmap = br.ReadInt16();

                    int r = br.ReadByte();
                    int g = br.ReadByte();
                    int b = br.ReadByte();
                    int a = br.ReadByte();

                    _color = Color.FromNonPremultiplied(r, g, b, a);
                }
            }

            private int _width;
            public int Width
            {
                get { return _width; }
            }

            private int _height;
            public int Height
            {
                get { return _height; }
            }

            private float _zoom;
            public float Zoom
            {
                get { return _zoom; }
            }

            private Texture2D[] _textures;
            public Texture2D[] Textures
            {
                get { return _textures; }
            }

            private Lightmap[] _lightmaps;
            public Lightmap[] Lightmaps
            {
                get { return _lightmaps; }
            }

            private Surface[] _surfaces;
            public Surface[] Surfaces
            {
                get { return _surfaces; }
            }

            private Cell[] _cells;
            public Cell[] Cells
            {
                get { return _cells; }
            }

            private VertexBuffer[] _vertices;
            public VertexBuffer[] Vertices
            {
                get { return _vertices; }
            }

            private GraphicsDevice _graphicsDevice;
            public GraphicsDevice GraphicsDevice
            {
                get { return _graphicsDevice; }
            }

            protected byte minorVersion;
            protected byte majorVersion;

            public Ground(GraphicsDevice gd)
            {
                _graphicsDevice = gd;
            }

            public bool Load(Stream gnd)
            {
                int len;
                BinaryReader br = new BinaryReader(gnd);
                string header = ((char)br.ReadByte()).ToString() + ((char)br.ReadByte()) + ((char)br.ReadByte()) + ((char)br.ReadByte());

                if (header != "GRGN")
                    return false;

                majorVersion = br.ReadByte();
                minorVersion = br.ReadByte();

                if (majorVersion != 1 || minorVersion != 7)
                    return false;

                _width = br.ReadInt32();
                _height = br.ReadInt32();
                _zoom = br.ReadSingle();

                _textures = new Texture2D[br.ReadInt32()];
                len = br.ReadInt32();
                for (int i = 0; i < _textures.Length; i++)
                {
                    _textures[i] = ROClient.Singleton.ContentManager.LoadContent<Texture2D>(Path.Combine("data\\texture\\", br.ReadCString(len).Korean()));
                }

                _lightmaps = new Lightmap[br.ReadInt32()];

                br.ReadInt32(); // Lightmap Width  = 8
                br.ReadInt32(); // Lightmap Height = 8
                br.ReadInt32(); // Lightmap Cells  = 1

                for (int i = 0; i < _lightmaps.Length; i++)
                {
                    Lightmap l = new Lightmap();

                    l.Load(br);

                    _lightmaps[i] = l;
                }

                _surfaces = new Surface[br.ReadInt32()];
                for (int i = 0; i < _surfaces.Length; i++)
                {
                    Surface s = new Surface();

                    s.Load(this, br);

                    _surfaces[i] = s;
                }

                _cells = new Cell[_width * _height];
                for (int i = 0; i < _cells.Length; i++)
                {
                    Cell c = new Cell();

                    c.Load(br);

                    _cells[i] = c;
                }

                CalculateNormals();
                SetupVertices();

                return true;
            }

            private void CalculateNormals()
            {
                int xfrom = 0;
                int yfrom = 0;
                int xto = _width;
                int yto = _height;

                for (int y = yfrom; y <= yto - 1; y++)
                {
                    for (int x = xfrom; x <= xto - 1; x++)
                    {
                        _cells[y * _width + x].CalculateNormal();
                    }
                }

                for (int y = yfrom; y <= yto - 1; y++)
                {
                    for (int x = xfrom; x <= xto - 1; x++)
                    {
                        int i = y * _width + x;
                        int iN = (y - 1) * _width + x;
                        int iNW = (y - 1) * _width + (x - 1);
                        int iNE = (y - 1) * _width + (x + 1);
                        int iW = y * _width + (x - 1);
                        int iSW = (y + 1) * _width + (x - 1);
                        int iS = (y + 1) * _width + x;
                        int iSE = (y + 1) * _width + (x + 1);
                        int iE = y * _width + (x + 1);

                        _cells[i].Normal[1] = _cells[i].Normal[0];
                        _cells[i].Normal[2] = _cells[i].Normal[0];
                        _cells[i].Normal[3] = _cells[i].Normal[0];
                        _cells[i].Normal[4] = _cells[i].Normal[0];

                        if (y > 0)
                        {
                            _cells[i].Normal[1] += _cells[iN].Normal[0];
                            _cells[i].Normal[3] += _cells[iN].Normal[0];

                            if (x > 0)
                                _cells[i].Normal[1] += _cells[iNW].Normal[0];

                            if (x < _width - 1)
                                _cells[i].Normal[3] += _cells[iNE].Normal[0];
                        }

                        if (x > 0)
                        {
                            _cells[i].Normal[1] += _cells[iW].Normal[0];
                            _cells[i].Normal[2] += _cells[iW].Normal[0];

                            if (y < _height - 1)
                                _cells[i].Normal[2] += _cells[iSW].Normal[0];
                        }

                        if (y < _height - 1)
                        {
                            _cells[i].Normal[2] += _cells[iS].Normal[0];
                            _cells[i].Normal[4] += _cells[iS].Normal[0];

                            if (x < _width - 1)
                                _cells[i].Normal[4] += _cells[iSE].Normal[0];
                        }

                        if (x < _width - 1)
                        {
                            _cells[i].Normal[3] += _cells[iE].Normal[0];
                            _cells[i].Normal[4] += _cells[iE].Normal[0];
                        }

                        _cells[i].Normal[1].Normalize();
                        _cells[i].Normal[2].Normalize();
                        _cells[i].Normal[3].Normalize();
                        _cells[i].Normal[4].Normalize();
                    }
                }
            }

            private void SetupVertices()
            {
                List<VertexPositionNormalTexture>[] vertices = new List<VertexPositionNormalTexture>[_textures.Length];
                List<VertexPositionNormalColor> vertices_notexture = new List<VertexPositionNormalColor>();

                for (int i = 0; i < vertices.Length; i++)
                    vertices[i] = new List<VertexPositionNormalTexture>();

                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        int i = y * _width + x;
                        Cell c = _cells[i];

                        if (c.TileUp > -1 && c.TileUp < _surfaces.Length)
                        {
                            Surface t = _surfaces[c.TileUp];

                            vertices[t.Texture].Add(new VertexPositionNormalTexture(new Vector3(x * 10, -c.Height[0], (_height - y) * 10), c.Normal[1], new Vector2(t.U[0], t.V[0])));
                            vertices[t.Texture].Add(new VertexPositionNormalTexture(new Vector3(x * 10, -c.Height[2], (_height - y) * 10 - 10), c.Normal[3], new Vector2(t.U[2], t.V[2])));
                            vertices[t.Texture].Add(new VertexPositionNormalTexture(new Vector3(x * 10 + 10, -c.Height[1], (_height - y) * 10), c.Normal[2], new Vector2(t.U[1], t.V[1])));
                            vertices[t.Texture].Add(new VertexPositionNormalTexture(new Vector3(x * 10 + 10, -c.Height[3], (_height - y) * 10 - 10), c.Normal[4], new Vector2(t.U[3], t.V[3])));
                        }
                        else
                        {
                            vertices_notexture.Add(new VertexPositionNormalColor(new Vector3(x * 10, -c.Height[0], (_height - y) * 10), Color.White, c.Normal[1]));
                            vertices_notexture.Add(new VertexPositionNormalColor(new Vector3(x * 10, -c.Height[2], (_height - y) * 10 - 10), Color.White, c.Normal[3]));
                            vertices_notexture.Add(new VertexPositionNormalColor(new Vector3(x * 10 + 10, -c.Height[1], (_height - y) * 10), Color.White, c.Normal[2]));
                            vertices_notexture.Add(new VertexPositionNormalColor(new Vector3(x * 10 + 10, -c.Height[3], (_height - y) * 10 - 10), Color.White, c.Normal[4]));
                        }
                    }
                }
                
                _vertices = new VertexBuffer[_textures.Length + 1];

                _vertices[0] = new VertexBuffer(_graphicsDevice, VertexPositionNormalColor.VertexDeclaration, vertices_notexture.Count, BufferUsage.None);
                _vertices[0].SetData(vertices_notexture.ToArray());

                for (int i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].Count > 0)
                    {
                        _vertices[i + 1] = new VertexBuffer(_graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertices[i].Count, BufferUsage.None);
                        _vertices[i + 1].SetData(vertices[i].ToArray());
                    }
                }
            }

            public void Draw(Effect effect)
            {
                for (int i = 0; i < _vertices.Length; i++)
                {
                    if (_vertices[i] == null)
                        continue;

                    if (i > 0)
                        effect.Parameters["xTexture"].SetValue(_textures[i - 1]);

                    _graphicsDevice.SetVertexBuffer(_vertices[i]);
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                    }
                    _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, _vertices[i].VertexCount);
                }
            }
        }

        private Ground _ground;
        public Ground GroundInfo
        {
            get { return _ground; }
            set { _ground = value; }
        }

        private GraphicsDevice _graphicsDevice;
        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        public Map(GraphicsDevice gd)
        {
            _graphicsDevice = gd;
        }

        public bool Load(Stream gat, Stream gnd, Stream rsw)
        {
            _ground = new Ground(_graphicsDevice);

            if (!_ground.Load(gnd))
                return false;

            return true;
        }

        public void Draw(Effect worldEffect)
        {
            _ground.Draw(worldEffect);
        }
    }

    public struct VertexPositionNormalColor
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;
        public Vector2 TexturePosition;

        public VertexPositionNormalColor(Vector3 position, Color color, Vector3 normal)
        {
            Position = position;
            Color = color;
            Normal = normal;
            TexturePosition = new Vector2(0.0f, 0.0f);
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
             (
                  new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                  new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                  new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                  new VertexElement(sizeof(float) * 7, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
             );
    }
}
