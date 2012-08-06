using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IrrlichtLime.Video;
using IrrlichtLime.Core;

namespace FimbulwinterClient.Core.Assets.MapInternals
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
                    byte r = br.ReadByte();
                    byte g = br.ReadByte();
                    byte b = br.ReadByte();

                    _intensity[i] = new Color(r, g, b, 255);
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

            private Vector3Df[] _normal;
            public Vector3Df[] Normal
            {
                get { return _normal; }
            }

            public void Load(BinaryReader br, byte majorVersion, byte minorVersion)
            {
                _height = new float[4];
                _height[0] = br.ReadSingle();
                _height[1] = br.ReadSingle();
                _height[2] = br.ReadSingle();
                _height[3] = br.ReadSingle();

                if (majorVersion >= 1 && minorVersion >= 6)
                {
                    _tileUp = br.ReadInt32();
                    _tileSide = br.ReadInt32();
                    _tileOtherSide = br.ReadInt32();
                }
                else if (majorVersion == 0 && minorVersion == 0)
                {
                    br.ReadBytes(8); // ??
                }
                else
                {
                    _tileUp = br.ReadInt16();
                    _tileSide = br.ReadInt16();
                    _tileOtherSide = br.ReadInt16();
                    br.ReadInt16(); // ??
                }
            }

            public void SetTiles(int tileUp, int tileSide, int tileOtherSide)
            {
                _tileUp = tileUp;
                _tileSide = tileSide;
                _tileOtherSide = tileOtherSide;
            }

            public void CalculateNormal(Ground ground)
            {
                Vector3Df b1 = default(Vector3Df);
                Vector3Df b2 = default(Vector3Df);

                b1 = new Vector3Df(ground.Zoom, _height[1], ground.Zoom) - new Vector3Df(ground.Zoom, _height[3], ground.Zoom);
                b2 = new Vector3Df(ground.Zoom, _height[2], ground.Zoom) - new Vector3Df(ground.Zoom, _height[3], ground.Zoom);
                
                _normal = new Vector3Df[5];
                _normal[0] = b1.CrossProduct(b2);
                _normal[0].Normalize();
            }
        }

        public class Surface
        {
            private Vector2Df[] _texCoord;
            public Vector2Df[] TexCoord
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

            public void Load(Ground owner, BinaryReader br, byte majorVersion, byte minorVersion, int texture)
            {
                _texCoord = new Vector2Df[4];

                if (majorVersion == 0 && minorVersion == 0)
                {
                    _texCoord[0].X = br.ReadSingle();
                    _texCoord[0].Y = br.ReadSingle();

                    _texCoord[1].X = br.ReadSingle();
                    _texCoord[1].Y = br.ReadSingle();

                    _texCoord[2].X = br.ReadSingle();
                    _texCoord[2].Y = br.ReadSingle();

                    _texCoord[3].X = br.ReadSingle();
                    _texCoord[3].Y = br.ReadSingle();

                    _texture = texture;
                    _lightmap = 0;
                }
                else
                {
                    _texCoord[0] = new Vector2Df();
                    _texCoord[1] = new Vector2Df();
                    _texCoord[2] = new Vector2Df();
                    _texCoord[3] = new Vector2Df();

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

                _color = new Color(255, 255, 255);
            }

            public void Load(Ground owner, BinaryReader br, byte majorVersion, byte minorVersion)
            {
                Load(owner, br, majorVersion, minorVersion, 0);
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

        private Material[] _textures;
        public Material[] Textures
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

        private Vertex3D[] _vertices;
        public Vertex3D[] Vertices
        {
            get { return _vertices; }
        }

        private uint[][] _indexes;
        public uint[][] Indexes
        {
            get { return _indexes; }
        }

        protected byte minorVersion;
        protected byte majorVersion;

        public bool Load(Stream gnd)
        {
            int texChunkSize, textureCount;
            BinaryReader br = new BinaryReader(gnd);
            string header = ((char)br.ReadByte()).ToString() + ((char)br.ReadByte()) + ((char)br.ReadByte()) + ((char)br.ReadByte());

            if (header == "GRGN")
            {
                majorVersion = br.ReadByte();
                minorVersion = br.ReadByte();

                if (majorVersion != 1 || minorVersion < 5 || minorVersion > 7)
                    return false;

                _width = br.ReadInt32();
                _height = br.ReadInt32();
                _zoom = br.ReadSingle();

                textureCount = br.ReadInt32();

                texChunkSize = br.ReadInt32();
            }
            else
            {
                majorVersion = 0;
                minorVersion = 0;

                br.BaseStream.Position = 0;

                textureCount = br.ReadInt32();

                _width = br.ReadInt32();
                _height = br.ReadInt32();
                _zoom = 10.0f;

                texChunkSize = 80;
            }

            _textures = new Material[textureCount];

            for (int i = 0; i < _textures.Length; i++)
            {
                Material m = new Material();
                Texture t = SharedInformation.ContentManager.Load<Texture>(@"data\texture\" + br.ReadCString(texChunkSize));

                m.Wireframe = false;
                m.Lighting = false;
                m.Type = MaterialType.Solid;
                m.SetTexture(0, t);

                _textures[i] = m;
            }

            if (majorVersion == 0 && minorVersion == 0)
            {
                int idx = 0;
                _lightmaps = new Lightmap[0];
                _cells = new Cell[_width * _height];
                // Biggest possible amount of surfaces in this version is 3 * Width * Height.
                // We resize it to the real amount after iterating through the cells.
                _surfaces = new Surface[3 * _width * _height];
                for (int i = 0; i < _cells.Length; i++)
                {
                    int textureUp, textureOtherSide, textureSide;

                    textureUp = br.ReadInt32();
                    textureOtherSide = br.ReadInt32();
                    textureSide = br.ReadInt32();

                    Cell c = new Cell();

                    c.Load(br, majorVersion, minorVersion);

                    if (textureUp != -1)
                    {
                        Surface s = new Surface();

                        s.Load(this, br, majorVersion, minorVersion, textureUp);

                        textureUp = idx;

                        _surfaces[idx++] = s;
                    }
                    else
                    {
                        br.ReadBytes(32);
                    }

                    if (textureOtherSide != -1)
                    {
                        Surface s = new Surface();

                        s.Load(this, br, majorVersion, minorVersion, textureOtherSide);

                        textureOtherSide = idx;
                        
                        _surfaces[idx++] = s;
                    }
                    else
                    {
                        br.ReadBytes(32);
                    }

                    if (textureSide != -1)
                    {
                        Surface s = new Surface();

                        s.Load(this, br, majorVersion, minorVersion, textureSide);

                        textureSide = idx;

                        _surfaces[idx++] = s;
                    }
                    else
                    {
                        br.ReadBytes(32);
                    }

                    c.SetTiles(textureUp, textureSide, textureOtherSide);

                    _cells[i] = c;
                }
                Array.Resize<Surface>(ref _surfaces, idx);
            }
            else
            {

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

                    s.Load(this, br, majorVersion, minorVersion);

                    _surfaces[i] = s;
                }

                _cells = new Cell[_width * _height];
                for (int i = 0; i < _cells.Length; i++)
                {
                    Cell c = new Cell();

                    c.Load(br, majorVersion, minorVersion);

                    _cells[i] = c;
                }
            }

            SharedInformation.Logger.Write("Calculating normals...");
            CalculateNormals();

            SharedInformation.Logger.Write("Creating ground vertex buffer...");
            SetupVertices();

            SharedInformation.Logger.Write("Ground v{0}.{1} status: {2} textures - {3} lightmaps - {4} surfaces - {5} cells - {6} vertices", majorVersion, minorVersion, _textures.Length, _lightmaps.Length, _surfaces.Length, _cells.Length, _vertices.Length);

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
                    _cells[y * _width + x].CalculateNormal(this);
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
        public void SetupVertices()
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

            _vertices = new Vertex3D[objectCount * 4];
            List<uint>[] indexdata = new List<uint>[_textures.Length];

            for (int i = 0; i < indexdata.Length; i++)
            {
                indexdata[i] = new List<uint>();
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

                        SetupSurface(_vertices, indexdata[tid], _cells[idx].TileUp, cur_surface, x, y, 0);

                        cur_surface++;
                    }

                    if (_cells[idx].TileSide != -1)
                    {
                        int tid = _surfaces[_cells[idx].TileSide].Texture;

                        SetupSurface(_vertices, indexdata[tid], _cells[idx].TileSide, cur_surface, x, y, 1);

                        cur_surface++;
                    }

                    if (_cells[idx].TileOtherSide != -1)
                    {
                        int tid = _surfaces[_cells[idx].TileOtherSide].Texture;

                        SetupSurface(_vertices, indexdata[tid], _cells[idx].TileOtherSide, cur_surface, x, y, 2);

                        cur_surface++;
                    }
                }
            }

            _indexes = new uint[_textures.Length][];
            for (int i = 0; i < _indexes.Length; i++)
            {
                _indexes[i] = indexdata[i].ToArray();
            }
        }

        private void SetupSurface(Vertex3D[] vertexdata, List<uint> indexdata, int surface_id, int current_surface, int x, int y, int type)
        {
            int idx = current_surface * 4;
            int cell_idx = y * _width + x;
            Cell cell = _cells[cell_idx];

            Surface surface = _surfaces[surface_id];

            Vector3Df[] position = new Vector3Df[4];
            Vector3Df[] normal = new Vector3Df[4];

            switch (type)
            {
                case 0:
                    {
                        float x0 = (x - _width / 2) * _zoom;
                        float x1 = (x - _width / 2 + 1) * _zoom;

                        float z0 = (y - _height / 2) * _zoom;
                        float z1 = (y - _height / 2 + 1) * _zoom;

                        position[0] = new Vector3Df(x0, cell.Height[0], z0);
                        position[1] = new Vector3Df(x1, cell.Height[1], z0);
                        position[2] = new Vector3Df(x0, cell.Height[2], z1);
                        position[3] = new Vector3Df(x1, cell.Height[3], z1);

                        normal[0] = cell.Normal[1];
                        normal[1] = cell.Normal[2];
                        normal[2] = cell.Normal[3];
                        normal[3] = cell.Normal[4];
                    }
                    break;
                case 1:
                    {
                        Cell cell2 = _cells[(y + 1) * _width + x];

                        float x0 = (x - _width / 2) * _zoom;
                        float x1 = (x - _width / 2 + 1) * _zoom;

                        float z0 = (y - _height / 2 + 1) * _zoom;

                        position[0] = new Vector3Df(x0, cell.Height[2], z0);
                        position[1] = new Vector3Df(x1, cell.Height[3], z0);
                        position[2] = new Vector3Df(x0, cell2.Height[0], z0);
                        position[3] = new Vector3Df(x1, cell2.Height[1], z0);

                        normal[0] = new Vector3Df(0, 0, cell2.Height[0] > cell.Height[3] ? -1 : 1);
                        normal[1] = normal[0];
                        normal[2] = normal[0];
                        normal[3] = normal[0];
                    }
                    break;
                case 2:
                    {
                        Cell cell2 = _cells[y * _width + x + 1];

                        float x0 = (x - _width / 2 + 1) * _zoom;

                        float z0 = (y - _height / 2) * _zoom;
                        float z1 = (y - _height / 2 + 1) * _zoom;

                        position[0] = new Vector3Df(x0, cell.Height[3], z1);
                        position[1] = new Vector3Df(x0, cell.Height[1], z0);
                        position[2] = new Vector3Df(x0, cell2.Height[2], z1);
                        position[3] = new Vector3Df(x0, cell2.Height[0], z0);

                        normal[0] = new Vector3Df(cell.Height[3] > cell2.Height[2] ? -1 : 1, 0, 0);
                        normal[1] = normal[0];
                        normal[2] = normal[0];
                        normal[3] = normal[0];
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

            vertexdata[idx + 0] = new Vertex3D(position[0], normal[0], surface.Color, surface.TexCoord[0]);
            vertexdata[idx + 1] = new Vertex3D(position[1], normal[1], surface.Color, surface.TexCoord[1]);
            vertexdata[idx + 2] = new Vertex3D(position[2], normal[2], surface.Color, surface.TexCoord[2]);
            vertexdata[idx + 3] = new Vertex3D(position[3], normal[3], surface.Color, surface.TexCoord[3]);

            indexdata.Add((uint)(idx + 0));
            indexdata.Add((uint)(idx + 1));
            indexdata.Add((uint)(idx + 2));
            indexdata.Add((uint)(idx + 2));
            indexdata.Add((uint)(idx + 1));
            indexdata.Add((uint)(idx + 3));
        }

        public void Draw()
        {
            for (int i = 0; i < _textures.Length; i++)
            {
                SharedInformation.Graphics.SetMaterial(_textures[i]);
                SharedInformation.Graphics.DrawVertexPrimitiveList(_vertices, _indexes[i]); //.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.VertexCount, 0, _indexes[i].IndexCount / 3);
            }
        }
    }
}