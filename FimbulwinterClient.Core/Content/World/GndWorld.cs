using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Axiom.Collections;
using Axiom.Core;
using Axiom.Math;
using Axiom.Graphics;
using System.Runtime.InteropServices;
using Axiom.Core.Collections;

namespace FimbulwinterClient.Core.Content.World
{
    public class GndWorld : Resource, IRenderable
    {
        public class Lightmap
        {
            public struct LightmapColor
            {
                public byte R;
                public byte G;
                public byte B;
            }

            private byte[] _brightness;
            public byte[] Brightness
            {
                get { return _brightness; }
            }

            private LightmapColor[] _intensity;
            public LightmapColor[] Intensity
            {
                get { return _intensity; }
            }

            public void Load(BinaryReader br)
            {
                _brightness = br.ReadBytes(8 * 8);

                _intensity = new LightmapColor[8 * 8];
                for (int i = 0; i < _intensity.Length; i++)
                {
                    _intensity[i].R = br.ReadByte();
                    _intensity[i].G = br.ReadByte();
                    _intensity[i].B = br.ReadByte();
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

            private Vector3[] _normal;
            public Vector3[] Normal
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

            public void CalculateNormal(GndWorld ground)
            {
                Vector3 b1 = default(Vector3);
                Vector3 b2 = default(Vector3);

                b1 = new Vector3(ground.Zoom, _height[1], ground.Zoom) - new Vector3(ground.Zoom, _height[3], ground.Zoom);
                b2 = new Vector3(ground.Zoom, _height[2], ground.Zoom) - new Vector3(ground.Zoom, _height[3], ground.Zoom);

                _normal = new Vector3[5];
                _normal[0] = b1.Cross(b2);
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

            private ColorEx _color;
            public ColorEx Color
            {
                get { return _color; }
            }

            public void Load(GndWorld owner, BinaryReader br, byte majorVersion, byte minorVersion, int texture)
            {
                _texCoord = new Vector2[4];

                if (majorVersion == 0 && minorVersion == 0)
                {
                    _texCoord[0].x = br.ReadSingle();
                    _texCoord[0].y = br.ReadSingle();

                    _texCoord[1].x = br.ReadSingle();
                    _texCoord[1].y = br.ReadSingle();

                    _texCoord[2].x = br.ReadSingle();
                    _texCoord[2].y = br.ReadSingle();

                    _texCoord[3].x = br.ReadSingle();
                    _texCoord[3].y = br.ReadSingle();

                    _texture = texture;
                    _lightmap = 0;
                }
                else
                {
                    _texCoord[0].x = br.ReadSingle();
                    _texCoord[1].x = br.ReadSingle();
                    _texCoord[2].x = br.ReadSingle();
                    _texCoord[3].x = br.ReadSingle();

                    _texCoord[0].y = br.ReadSingle();
                    _texCoord[1].y = br.ReadSingle();
                    _texCoord[2].y = br.ReadSingle();
                    _texCoord[3].y = br.ReadSingle();

                    _texture = br.ReadInt16();
                    _lightmap = br.ReadInt16();

                    int r = br.ReadByte();
                    int g = br.ReadByte();
                    int b = br.ReadByte();
                    int a = br.ReadByte();

                    _color = new ColorEx(r / 255.0F, g / 255.0F, b / 255.0F, a / 255.0F);
                }

                _color = ColorEx.White;
            }

            public void Load(GndWorld owner, BinaryReader br, byte majorVersion, byte minorVersion)
            {
                Load(owner, br, majorVersion, minorVersion, 0);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GndVertex
        {
            public Vector3 position;
            public Vector3 normal;
            public Vector2 texCoords;

            public GndVertex(Vector3 position, Vector3 normal, Vector2 tex, Vector2 lmap, ColorEx color)
            {
                this.position = position;
                this.normal = normal;
                this.texCoords = tex;
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

        private Material[] _materials;
        public Material[] Materials
        {
            get { return _materials; }
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

        private VertexData _vertices;
        public VertexData Vertices
        {
            get { return _vertices; }
        }

        private HardwareIndexBuffer[] _indexes;
        public HardwareIndexBuffer[] Indexes
        {
            get { return _indexes; }
        }

        protected byte minorVersion;
        protected byte majorVersion;

        public GndWorld(ResourceManager parent, string name, ulong handle, string group, bool isManual, IManualResourceLoader loader, NameValuePairList createParams)
            : base(parent, name, handle, group, isManual, loader)
        {
        }

        public void Load(Stream gnd)
        {
            int texChunkSize, textureCount;
            BinaryReader br = new BinaryReader(gnd);
            string header = ((char)br.ReadByte()).ToString() + ((char)br.ReadByte()) + ((char)br.ReadByte()) + ((char)br.ReadByte());

            if (header == "GRGN")
            {
                majorVersion = br.ReadByte();
                minorVersion = br.ReadByte();

                if (majorVersion != 1 || minorVersion < 5 || minorVersion > 7)
                    throw new AxiomException("Invalid GND header: {0}", header);

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

            _materials = new Material[textureCount];
            for (int i = 0; i < _materials.Length; i++)
            {
                Material m = (Material)MaterialManager.Instance.Create("worldTexture" + i, "World");
                Pass p = m.GetTechnique(0).GetPass(0);
                TextureUnitState t = p.CreateTextureUnitState(@"data\texture\" + br.ReadCString(texChunkSize));

                if (t != null)
                {
                    t.SetColorOperation(LayerBlendOperation.Replace);
                    t.SetTextureAddressingMode(TextureAddressing.Wrap);
                }

                m.Lighting = false;
                m.CullingMode = CullingMode.None;
                m.Load();

                _materials[i] = m;
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

            CalculateNormals();
            SetupVertices();
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

            GndVertex[] vertexdata = new GndVertex[objectCount * 4];
            List<int>[] indexdata = new List<int>[_materials.Length];
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

                        SetupSurface(vertexdata, indexdata[tid], _cells[idx].TileSide, cur_surface, x, y, 1);

                        cur_surface++;
                    }

                    if (_cells[idx].TileOtherSide != -1)
                    {
                        int tid = _surfaces[_cells[idx].TileOtherSide].Texture;

                        SetupSurface(vertexdata, indexdata[tid], _cells[idx].TileOtherSide, cur_surface, x, y, 2);

                        cur_surface++;
                    }
                }
            }

            _vertices = new VertexData();

            VertexDeclaration decl = _vertices.vertexDeclaration;
            int offset = 0;
            decl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.Position);
            offset += VertexElement.GetTypeSize(VertexElementType.Float3);
            decl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.Normal);
            offset += VertexElement.GetTypeSize(VertexElementType.Float3);
            decl.AddElement(0, offset, VertexElementType.Float2, VertexElementSemantic.TexCoords, 0);
            //offset += VertexElement.GetTypeSize(VertexElementType.Float2);
            //decl.AddElement(0, offset, VertexElementType.Float2, VertexElementSemantic.TexCoords, 1);

            HardwareVertexBuffer vbuff = HardwareBufferManager.Instance.CreateVertexBuffer(decl.Clone(0), vertexdata.Length, BufferUsage.DynamicWriteOnly);
            vbuff.WriteData(0, Memory.SizeOf(typeof(GndVertex)) * vertexdata.Length, vertexdata.ToArray());

            _vertices.vertexBufferBinding.SetBinding(0, vbuff);
            _vertices.vertexStart = 0;
            _vertices.vertexCount = vertexdata.Length;

            _indexes = new HardwareIndexBuffer[_materials.Length];
            for (int i = 0; i < _indexes.Length; i++)
            {
                HardwareIndexBuffer ibuff = HardwareBufferManager.Instance.CreateIndexBuffer(IndexType.Size32, indexdata[i].Count, BufferUsage.DynamicWriteOnly);

                ibuff.WriteData(0, Memory.SizeOf(typeof(int)) * indexdata[i].Count, indexdata[i].ToArray());
                
                _indexes[i] = ibuff;
            }
        }

        private void SetupSurface(GndVertex[] vertexdata, List<int> indexdata, int surface_id, int current_surface, int x, int y, int type)
        {
            int idx = current_surface * 4;
            int cell_idx = y * _width + x;
            Cell cell = _cells[cell_idx];

            Surface surface = _surfaces[surface_id];

            Vector3[] position = new Vector3[4];
            Vector3[] normal = new Vector3[4];

            switch (type)
            {
                case 0:
                    {
                        float x0 = (x - _width / 2) * _zoom;
                        float x1 = (x - _width / 2 + 1) * _zoom;

                        float z0 = (y - _height / 2) * _zoom;
                        float z1 = (y - _height / 2 + 1) * _zoom;

                        position[0] = new Vector3(x0, cell.Height[0], z0);
                        position[1] = new Vector3(x1, cell.Height[1], z0);
                        position[2] = new Vector3(x0, cell.Height[2], z1);
                        position[3] = new Vector3(x1, cell.Height[3], z1);

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

                        position[0] = new Vector3(x0, cell.Height[2], z0);
                        position[1] = new Vector3(x1, cell.Height[3], z0);
                        position[2] = new Vector3(x0, cell2.Height[0], z0);
                        position[3] = new Vector3(x1, cell2.Height[1], z0);

                        normal[0] = new Vector3(0, 0, cell2.Height[0] > cell.Height[3] ? -1 : 1);
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

                        position[0] = new Vector3(x0, cell.Height[3], z1);
                        position[1] = new Vector3(x0, cell.Height[1], z0);
                        position[2] = new Vector3(x0, cell2.Height[2], z1);
                        position[3] = new Vector3(x0, cell2.Height[0], z0);

                        normal[0] = new Vector3(cell.Height[3] > cell2.Height[2] ? -1 : 1, 0, 0);
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

            vertexdata[idx + 0] = new GndVertex(position[0], normal[0], surface.TexCoord[0], new Vector2(lightmapU[0], lightmapV[0]), surface.Color);
            vertexdata[idx + 1] = new GndVertex(position[1], normal[1], surface.TexCoord[1], new Vector2(lightmapU[1], lightmapV[0]), surface.Color);
            vertexdata[idx + 2] = new GndVertex(position[2], normal[2], surface.TexCoord[2], new Vector2(lightmapU[0], lightmapV[1]), surface.Color);
            vertexdata[idx + 3] = new GndVertex(position[3], normal[3], surface.TexCoord[3], new Vector2(lightmapU[1], lightmapV[1]), surface.Color);

            indexdata.Add(idx + 0);
            indexdata.Add(idx + 1);
            indexdata.Add(idx + 2);
            indexdata.Add(idx + 2);
            indexdata.Add(idx + 1);
            indexdata.Add(idx + 3);
        }

        protected override void load()
        {
            Stream stream = ResourceGroupManager.Instance.OpenResource(Name);
            Load(stream);
            stream.Close();
        }

        protected override void unload()
        {

        }

        public void Render()
        {
            RenderOperation op = new RenderOperation();
            op.vertexData = _vertices;
            op.operationType = OperationType.TriangleList;

            for (int i = 0; i < _indexes.Length; i++)
            {
                op.indexData = new IndexData();
                op.indexData.indexBuffer = _indexes[i];
                op.indexData.indexCount = _indexes[i].IndexCount;
                op.indexData.indexStart = 0;

                Root.Instance.RenderSystem.Render(op);
            }
        }

        public bool CastsShadows
        {
            get { return false; }
        }

        public Vector4 GetCustomParameter(int index)
        {
            throw new NotImplementedException();
        }

        public Real GetSquaredViewDepth(Camera camera)
        {
            throw new NotImplementedException();
        }

        public void GetWorldTransforms(Matrix4[] matrices)
        {
            throw new NotImplementedException();
        }

        public LightList Lights
        {
            get { throw new NotImplementedException(); }
        }

        public Material Material
        {
            get { return _materials[0]; }
        }

        public bool NormalizeNormals
        {
            get { throw new NotImplementedException(); }
        }

        public ushort NumWorldTransforms
        {
            get { throw new NotImplementedException(); }
        }

        public bool PolygonModeOverrideable
        {
            get { throw new NotImplementedException(); }
        }

        public RenderOperation RenderOperation
        {
            get { throw new NotImplementedException(); }
        }

        public void SetCustomParameter(int index, Vector4 val)
        {
            throw new NotImplementedException();
        }

        public Technique Technique
        {
            get { return Material.GetBestTechnique(); }
        }

        public void UpdateCustomGpuParameter(GpuProgramParameters.AutoConstantEntry constant, GpuProgramParameters parameters)
        {
            throw new NotImplementedException();
        }

        public bool UseIdentityProjection
        {
            get { throw new NotImplementedException(); }
        }

        public bool UseIdentityView
        {
            get { throw new NotImplementedException(); }
        }

        public Quaternion WorldOrientation
        {
            get { throw new NotImplementedException(); }
        }

        public Vector3 WorldPosition
        {
            get { throw new NotImplementedException(); }
        }
    }
}
