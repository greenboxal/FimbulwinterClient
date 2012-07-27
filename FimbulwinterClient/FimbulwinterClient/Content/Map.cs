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

            private VertexBuffer _vertices;
            public VertexBuffer Vertices
            {
                get { return _vertices; }
            }

            private IndexBuffer[] _indexes;
            public IndexBuffer[] Indexes
            {
                get { return _indexes; }
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

            private int objectCount;
            private void SetupVertices()
            {
                objectCount = 0;

                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        int idx = y * _width + x;

                        if (_cells[idx].TileUp != -1)
                            objectCount++;

                        if (_cells[idx].TileSide != -1)
                            objectCount++;

                        if (_cells[idx].TileOtherSide != -1)
                            objectCount++;
                    }
                }

                VertexPositionNormalTexture[] vertexdata = new VertexPositionNormalTexture[objectCount * 4];
                List<short>[] indexdata = new List<short>[_textures.Length];

                for (int i = 0; i < indexdata.Length; i++)
                {
                    indexdata[i] = new List<short>();
                }

                int cur_surface = 0;
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        int idx = y * _width + x;

                        if (_cells[idx].TileUp != -1)
                        {
                            int tid = _surfaces[_cells[idx].TileUp].Texture;

                            SetupSurface(vertexdata, indexdata[tid], _cells[idx].TileUp, cur_surface, x, y, 0);

                            cur_surface++;
                        }

                        if (_cells[idx].TileSide != -1)
                        {
                            int tid = _surfaces[_cells[idx].TileSide].Texture;

                            SetupSurface(vertexdata, indexdata[tid], _cells[idx].TileUp, cur_surface, x, y, 1);

                            cur_surface++;
                        }

                        if (_cells[idx].TileOtherSide != -1)
                        {
                            int tid = _surfaces[_cells[idx].TileOtherSide].Texture;

                            SetupSurface(vertexdata, indexdata[tid], _cells[idx].TileUp, cur_surface, x, y, 2);

                            cur_surface++;
                        }
                    }
                }

                _vertices = new VertexBuffer(_graphicsDevice, typeof(VertexPositionNormalTexture), vertexdata.Length, BufferUsage.None);
                _vertices.SetData(vertexdata);

                _indexes = new IndexBuffer[_textures.Length];
                for (int i = 0; i < _indexes.Length; i++)
                {
                    _indexes[i] = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, sizeof(short) * indexdata[i].Count, BufferUsage.None);
                    _indexes[i].SetData(indexdata[i].ToArray());
                }
            }

            private void SetupSurface(VertexPositionNormalTexture[] vertexdata, List<short> indexdata, int surface_id, int current_surface, int x, int y, int type)
            {
                float xoffset = -1.0F * (float)_width * _zoom / 2.0F;
                float yoffset = -1.0F * (float)_height * _zoom / 2.0F;

                int idx = current_surface * 4;
                int cell_idx = y * _width + x;
                Cell cell = _cells[cell_idx];
                Surface surface = _surfaces[surface_id];

                Vector3[] position = new Vector3[4];
                Vector3[] normal = new Vector3[4];
                Vector2[] tex = new Vector2[4];

                tex[0] = new Vector2(surface.U[0], surface.V[0]);
                tex[1] = new Vector2(surface.U[1], surface.V[1]);
                tex[2] = new Vector2(surface.U[2], surface.V[2]);
                tex[3] = new Vector2(surface.U[3], surface.V[3]);

                switch (type)
                {
                    case 0:
                        {
                            float x0 = (float)x * _zoom + xoffset;
                            float y0 = (float)y * _zoom + yoffset;

                            float x1 = (float)(x + 1) * _zoom + xoffset;
                            float y1 = (float)(y + 1) * _zoom + yoffset;

                            position[0] = new Vector3(x0, cell.Height[0], y0);
                            position[1] = new Vector3(x1, cell.Height[1], y0);
                            position[2] = new Vector3(x0, cell.Height[2], y1);
                            position[3] = new Vector3(x1, cell.Height[3], y1);
                        }
                        break;
                    case 1:
                        {
                            Cell cell2 = _cells[(y + 1) * _width + x];

                            float x0 = (float)x * _zoom + xoffset;
                            float x1 = (float)(x + 1) * _zoom + xoffset;

                            float yy = (float)(y + 1) * _zoom + yoffset;

                            position[0] = new Vector3(x0, cell.Height[2], yy);
                            position[1] = new Vector3(x1, cell.Height[3], yy);
                            position[2] = new Vector3(x0, cell2.Height[0], yy);
                            position[3] = new Vector3(x1, cell2.Height[1], yy);
                        }
                        break;
                    case 2:
                        {
                            Cell cell2 = _cells[(y + 1) * _width + x];

                            float xx = (float)x * _zoom + xoffset;

                            float y0 = (float)y * _zoom + yoffset;
                            float y1 = (float)(y + 1) * _zoom + yoffset;

                            position[0] = new Vector3(xx, cell.Height[3], y1);
                            position[1] = new Vector3(xx, cell.Height[1], y0);
                            position[2] = new Vector3(xx, cell2.Height[2], y1);
                            position[3] = new Vector3(xx, cell2.Height[0], y0);
                        }
                        break;
                }

                for (int i = 0; i < 4; i++)
                    normal[i] = cell.Normal[i + 1];

                vertexdata[idx + 0] = new VertexPositionNormalTexture(position[0], normal[0], tex[0]);
                vertexdata[idx + 1] = new VertexPositionNormalTexture(position[1], normal[1], tex[1]);
                vertexdata[idx + 2] = new VertexPositionNormalTexture(position[2], normal[2], tex[2]);
                vertexdata[idx + 3] = new VertexPositionNormalTexture(position[3], normal[3], tex[3]);

                indexdata.Add((short)(idx + 0));
                indexdata.Add((short)(idx + 1));
                indexdata.Add((short)(idx + 2));
                indexdata.Add((short)(idx + 2));
                indexdata.Add((short)(idx + 1));
                indexdata.Add((short)(idx + 3));
            }

            public void Draw(Effect effect)
            {
                effect.CurrentTechnique = effect.Techniques["Textured"];

                _graphicsDevice.SetVertexBuffer(_vertices);

                for (int i = 0; i < _textures.Length; i++)
                {
                    _graphicsDevice.Indices = _indexes[i];
                    effect.Parameters["xTexture"].SetValue(_textures[i]);

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                    }

                    _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.VertexCount, 0, _indexes[i].IndexCount / 3);
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
