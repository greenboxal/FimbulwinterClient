using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FimbulwinterClient.Content.MapInternals
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

            private byte[] _intensity;
            public byte[] Intensity
            {
                get { return _intensity; }
            }

            public void Load(BinaryReader br)
            {
                _brightness = br.ReadBytes(8 * 8);
                _intensity = br.ReadBytes(8 * 8 * 3);
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
            private Vector2[] _texCoord;
            public Vector2[] TexCoord
            {
                get { return _texCoord; }
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

            public void Load(Ground owner, BinaryReader br)
            {
                _texCoord = new Vector2[4];

                _texCoord[0].X = br.ReadSingle();
                _texCoord[1].X = br.ReadSingle();
                _texCoord[2].X = br.ReadSingle();
                _texCoord[3].X = br.ReadSingle();

                _texCoord[0].Y = br.ReadSingle();
                _texCoord[1].Y = br.ReadSingle();
                _texCoord[2].Y = br.ReadSingle();
                _texCoord[3].Y = br.ReadSingle();

                _texture = br.ReadInt16();
                _lightmap = br.ReadInt16();

                int r = br.ReadByte();
                int g = br.ReadByte();
                int b = br.ReadByte();
                int a = br.ReadByte();

                _color = new Color(r, g, b, a);
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

            VertexPositionTextureNormalLightmap[] vertexdata = new VertexPositionTextureNormalLightmap[objectCount * 4];
            List<int>[] indexdata = new List<int>[_textures.Length];

            for (int i = 0; i < indexdata.Length; i++)
            {
                indexdata[i] = new List<int>();
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

            _vertices = new VertexBuffer(_graphicsDevice, typeof(VertexPositionTextureNormalLightmap), vertexdata.Length, BufferUsage.WriteOnly);
            _vertices.SetData(vertexdata);

            _indexes = new IndexBuffer[_textures.Length];
            for (int i = 0; i < _indexes.Length; i++)
            {
                _indexes[i] = new IndexBuffer(_graphicsDevice, typeof(int), indexdata[i].Count, BufferUsage.WriteOnly);
                _indexes[i].SetData(indexdata[i].ToArray());
            }
        }

        private void SetupSurface(VertexPositionTextureNormalLightmap[] vertexdata, List<int> indexdata, int surface_id, int current_surface, int x, int y, int type)
        {
            float xoffset = -1.0F * (float)_width * _zoom / 2.0F;
            float yoffset = -1.0F * (float)_height * _zoom / 2.0F;

            int idx = current_surface * 4;
            int cell_idx = y * _width + x;
            Cell cell = _cells[cell_idx];
            Surface surface = _surfaces[surface_id];

            Vector3[] position = new Vector3[4];

            switch (type)
            {
                case 0:
                    {
                        float x0 = (float)x * _zoom + xoffset;
                        float y0 = (float)(_height - y) * _zoom + yoffset;

                        float x1 = (float)(x + 1) * _zoom + xoffset;
                        float y1 = (float)((_height - y) - 1) * _zoom + yoffset;

                        position[0] = new Vector3(x0, -cell.Height[0], y0);
                        position[1] = new Vector3(x1, -cell.Height[1], y0);
                        position[2] = new Vector3(x0, -cell.Height[2], y1);
                        position[3] = new Vector3(x1, -cell.Height[3], y1);
                    }
                    break;
                case 1:
                    {
                        Cell cell2 = _cells[(y + 1) * _width + x];

                        float x0 = (float)x * _zoom + xoffset;
                        float x1 = (float)(x + 1) * _zoom + xoffset;

                        float yy = (float)((_height - y) - 1) * _zoom + yoffset;

                        position[0] = new Vector3(x0, -cell.Height[2], yy);
                        position[1] = new Vector3(x1, -cell.Height[3], yy);
                        position[2] = new Vector3(x0, -cell2.Height[0], yy);
                        position[3] = new Vector3(x1, -cell2.Height[1], yy);
                    }
                    break;
                case 2:
                    {
                        Cell cell2 = _cells[y * _width + x + 1];

                        float xx = (float)x * _zoom + xoffset;

                        float y0 = (float)(_height - y) * _zoom + yoffset;
                        float y1 = (float)((_height - y) - 1) * _zoom + yoffset;

                        position[0] = new Vector3(xx, -cell.Height[3], y1);
                        position[1] = new Vector3(xx, -cell.Height[1], y0);
                        position[2] = new Vector3(xx, -cell2.Height[2], y1);
                        position[3] = new Vector3(xx, -cell2.Height[0], y0);
                    }
                    break;
            }

            int lm_w = (int)Math.Floor(Math.Sqrt(_lightmaps.Length));
            int lm_h = (int)Math.Ceiling((float)_lightmaps.Length / lm_w);
            int lm_x = (int)Math.Floor((float)surface.Lightmap / lm_h);
            int lm_y = surface.Lightmap % lm_h;

            float[] lightmapU = new float[2];
            float[] lightmapV = new float[2];
            lightmapU[0] = (float)(0.1f + lm_x) / lm_w;
            lightmapU[1] = (float)(0.9f + lm_x) / lm_w;
            lightmapV[0] = (float)(0.1f + lm_y) / lm_h;
            lightmapV[1] = (float)(0.9f + lm_y) / lm_h;

            vertexdata[idx + 0] = new VertexPositionTextureNormalLightmap(position[0], cell.Normal[1], surface.TexCoord[0], new Vector2(lightmapU[0], lightmapV[0]));
            vertexdata[idx + 1] = new VertexPositionTextureNormalLightmap(position[1], cell.Normal[2], surface.TexCoord[1], new Vector2(lightmapU[1], lightmapV[0]));
            vertexdata[idx + 2] = new VertexPositionTextureNormalLightmap(position[2], cell.Normal[3], surface.TexCoord[2], new Vector2(lightmapU[0], lightmapV[1]));
            vertexdata[idx + 3] = new VertexPositionTextureNormalLightmap(position[3], cell.Normal[4], surface.TexCoord[3], new Vector2(lightmapU[1], lightmapV[1]));

            indexdata.Add(idx + 0);
            indexdata.Add(idx + 1);
            indexdata.Add(idx + 2);
            indexdata.Add(idx + 2);
            indexdata.Add(idx + 1);
            indexdata.Add(idx + 3);
        }

        public void Draw(Effect effect)
        {
            _graphicsDevice.SetVertexBuffer(_vertices);

            for (int i = 0; i < _textures.Length; i++)
            {
                _graphicsDevice.Indices = _indexes[i];
                effect.Parameters["Texture"].SetValue(_textures[i]);

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.VertexCount, 0, _indexes[i].IndexCount / 3);
            }
        }
    }
}
