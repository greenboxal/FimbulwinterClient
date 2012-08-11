using System;
using System.Collections.Generic;
using System.Drawing;
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
    public class Ground
    {
        public class Lightmap
        {
            public byte[] Brightness { get; private set; }
            public Color[] Intensity { get; private set; }

            public void Load(BinaryReader br)
            {
                Brightness = br.ReadBytes(8 * 8);

                Intensity = new Color[8 * 8];
                for (int i = 0; i < Intensity.Length; i++)
                {
                    byte r = br.ReadByte();
                    byte g = br.ReadByte();
                    byte b = br.ReadByte();

                    Intensity[i] = Color.FromArgb(255, r, g, b);
                }
            }
        }

        public class Cell
        {
            public float[] Height { get; private set; }
            public int TileUp { get; private set; }
            public int TileSide { get; private set; }
            public int TileOtherSide { get; private set; }
            public Vector3[] Normal { get; private set; }

            public void Load(BinaryReader br, byte majorVersion, byte minorVersion)
            {
                Height = new float[4];
                Height[0] = br.ReadSingle();
                Height[1] = br.ReadSingle();
                Height[2] = br.ReadSingle();
                Height[3] = br.ReadSingle();

                if (majorVersion >= 1 && minorVersion >= 6)
                {
                    TileUp = br.ReadInt32();
                    TileSide = br.ReadInt32();
                    TileOtherSide = br.ReadInt32();
                }
                else if (majorVersion == 0 && minorVersion == 0)
                {
                    br.ReadBytes(8); // ??
                }
                else
                {
                    TileUp = br.ReadInt16();
                    TileSide = br.ReadInt16();
                    TileOtherSide = br.ReadInt16();
                    br.ReadInt16(); // ??
                }
            }

            public void SetTiles(int tileUp, int tileSide, int tileOtherSide)
            {
                TileUp = tileUp;
                TileSide = tileSide;
                TileOtherSide = tileOtherSide;
            }

            public void CalculateNormal(Ground ground)
            {
                Vector3 b1 = new Vector3(ground.Zoom, Height[1], ground.Zoom) - new Vector3(ground.Zoom, Height[3], ground.Zoom);
                Vector3 b2 = new Vector3(ground.Zoom, Height[2], ground.Zoom) - new Vector3(ground.Zoom, Height[3], ground.Zoom);

                Normal = new Vector3[5];
                Normal[0] = Vector3.Cross(b1, b2);
                Normal[0].Normalize();
            }
        }

        public class Surface
        {
            public Vector2[] TexCoord { get; private set; }
            public int Texture { get; private set; }
            public int Lightmap { get; private set; }
            public Color Color { get; private set; }

            public void Load(Ground owner, BinaryReader br, byte majorVersion, byte minorVersion, int texture)
            {
                TexCoord = new Vector2[4];

                if (majorVersion == 0 && minorVersion == 0)
                {
                    TexCoord[0].X = br.ReadSingle();
                    TexCoord[0].Y = br.ReadSingle();

                    TexCoord[1].X = br.ReadSingle();
                    TexCoord[1].Y = br.ReadSingle();

                    TexCoord[2].X = br.ReadSingle();
                    TexCoord[2].Y = br.ReadSingle();

                    TexCoord[3].X = br.ReadSingle();
                    TexCoord[3].Y = br.ReadSingle();

                    Texture = texture;
                    Lightmap = 0;

                    // Is it white?? Seems too bright...
                    Color = Color.White;
                }
                else
                {
                    TexCoord[0].X = br.ReadSingle();
                    TexCoord[1].X = br.ReadSingle();
                    TexCoord[2].X = br.ReadSingle();
                    TexCoord[3].X = br.ReadSingle();

                    TexCoord[0].Y = br.ReadSingle();
                    TexCoord[1].Y = br.ReadSingle();
                    TexCoord[2].Y = br.ReadSingle();
                    TexCoord[3].Y = br.ReadSingle();

                    Texture = br.ReadInt16();
                    Lightmap = br.ReadInt16();

                    int r = br.ReadByte();
                    int g = br.ReadByte();
                    int b = br.ReadByte();
                    int a = br.ReadByte();

                    Color = Color.FromArgb(a, r, g, b);
                }
            }

            public void Load(Ground owner, BinaryReader br, byte majorVersion, byte minorVersion)
            {
                Load(owner, br, majorVersion, minorVersion, 0);
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public float Zoom { get; private set; }
        public Texture2D[] Textures { get; private set; }
        public Lightmap[] Lightmaps { get; private set; }
        public Cell[] Cells { get; private set; }

        private Surface[] _surfaces;
        public Surface[] Surfaces
        {
            get { return _surfaces; }
        }

        public VertexBuffer VertexBuffer { get; private set; }
        public IndexBuffer[] IndexBuffers { get; private set; }

        public int ObjectCount { get; set; }

        protected byte MinorVersion;
        protected byte MajorVersion;

        public GroundShaderProgram ShaderProgram { get; private set; }

        public Ground()
        {
            ShaderProgram = new GroundShaderProgram();
        }

        public bool Load(Stream gnd)
        {
            int texChunkSize, textureCount;
            BinaryReader br = new BinaryReader(gnd);
            string header = ((char)br.ReadByte()).ToString(CultureInfo.InvariantCulture) + ((char)br.ReadByte()) + ((char)br.ReadByte()) + ((char)br.ReadByte());

            if (header == "GRGN")
            {
                MajorVersion = br.ReadByte();
                MinorVersion = br.ReadByte();

                if (MajorVersion != 1 || MinorVersion < 5 || MinorVersion > 7)
                    return false;

                Width = br.ReadInt32();
                Height = br.ReadInt32();
                Zoom = br.ReadSingle();

                textureCount = br.ReadInt32();

                texChunkSize = br.ReadInt32();
            }
            else
            {
                MajorVersion = 0;
                MinorVersion = 0;

                br.BaseStream.Position = 0;

                textureCount = br.ReadInt32();

                Width = br.ReadInt32();
                Height = br.ReadInt32();
                Zoom = 10.0f;

                texChunkSize = 80;
            }

            Textures = new Texture2D[textureCount];

            for (int i = 0; i < Textures.Length; i++)
            {
                Textures[i] = ContentManager.Instance.Load<Texture2D>(@"data\texture\" + br.ReadCString(texChunkSize));
            }

            if (MajorVersion == 0 && MinorVersion == 0)
            {
                int idx = 0;
                Lightmaps = new Lightmap[0];
                Cells = new Cell[Width * Height];
                // Biggest possible amount of surfaces in this version is 3 * Width * Height.
                // We resize it to the real amount after iterating through the cells.
                _surfaces = new Surface[3 * Width * Height];
                for (int i = 0; i < Cells.Length; i++)
                {
                    int textureUp = br.ReadInt32();
                    int textureOtherSide = br.ReadInt32();
                    int textureSide = br.ReadInt32();

                    Cell c = new Cell();

                    c.Load(br, MajorVersion, MinorVersion);

                    if (textureUp != -1)
                    {
                        Surface s = new Surface();

                        s.Load(this, br, MajorVersion, MinorVersion, textureUp);

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

                        s.Load(this, br, MajorVersion, MinorVersion, textureOtherSide);

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

                        s.Load(this, br, MajorVersion, MinorVersion, textureSide);

                        textureSide = idx;

                        _surfaces[idx++] = s;
                    }
                    else
                    {
                        br.ReadBytes(32);
                    }

                    c.SetTiles(textureUp, textureSide, textureOtherSide);

                    Cells[i] = c;
                }

                Array.Resize(ref _surfaces, idx);
            }
            else
            {

                Lightmaps = new Lightmap[br.ReadInt32()];

                br.ReadInt32(); // Lightmap Width  = 8
                br.ReadInt32(); // Lightmap Height = 8
                br.ReadInt32(); // Lightmap Cells  = 1

                for (int i = 0; i < Lightmaps.Length; i++)
                {
                    Lightmap l = new Lightmap();

                    l.Load(br);

                    Lightmaps[i] = l;
                }

                _surfaces = new Surface[br.ReadInt32()];
                for (int i = 0; i < _surfaces.Length; i++)
                {
                    Surface s = new Surface();

                    s.Load(this, br, MajorVersion, MinorVersion);

                    _surfaces[i] = s;
                }

                Cells = new Cell[Width * Height];
                for (int i = 0; i < Cells.Length; i++)
                {
                    Cell c = new Cell();

                    c.Load(br, MajorVersion, MinorVersion);

                    Cells[i] = c;
                }
            }

            //Logger.WriteLine("Calculating normals...");
            CalculateNormals();

            //Logger.WriteLine("Creating ground vertex buffer...");
            SetupVertices();

            //Logger.WriteLine("Ground v{0}.{1} status: {2} textures - {3} lightmaps - {4} surfaces - {5} cells - {6} vertices", MajorVersion, MinorVersion, Textures.Length, Lightmaps.Length, _surfaces.Length, Cells.Length, _vertices.VertexCount);

            return true;
        }

        private void CalculateNormals()
        {
            const int xfrom = 0;
            const int yfrom = 0;
            int xto = Width;
            int yto = Height;

            for (int y = yfrom; y <= yto - 1; y++)
            {
                for (int x = xfrom; x <= xto - 1; x++)
                {
                    Cells[y * Width + x].CalculateNormal(this);
                }
            }

            for (int y = yfrom; y <= yto - 1; y++)
            {
                for (int x = xfrom; x <= xto - 1; x++)
                {
                    int i = y * Width + x;
                    int iN = (y - 1) * Width + x;
                    int iNW = (y - 1) * Width + (x - 1);
                    int iNE = (y - 1) * Width + (x + 1);
                    int iW = y * Width + (x - 1);
                    int iSW = (y + 1) * Width + (x - 1);
                    int iS = (y + 1) * Width + x;
                    int iSE = (y + 1) * Width + (x + 1);
                    int iE = y * Width + (x + 1);

                    Cells[i].Normal[1] = Cells[i].Normal[0];
                    Cells[i].Normal[2] = Cells[i].Normal[0];
                    Cells[i].Normal[3] = Cells[i].Normal[0];
                    Cells[i].Normal[4] = Cells[i].Normal[0];

                    if (y > 0)
                    {
                        Cells[i].Normal[1] += Cells[iN].Normal[0];
                        Cells[i].Normal[3] += Cells[iN].Normal[0];

                        if (x > 0)
                            Cells[i].Normal[1] += Cells[iNW].Normal[0];

                        if (x < Width - 1)
                            Cells[i].Normal[3] += Cells[iNE].Normal[0];
                    }

                    if (x > 0)
                    {
                        Cells[i].Normal[1] += Cells[iW].Normal[0];
                        Cells[i].Normal[2] += Cells[iW].Normal[0];

                        if (y < Height - 1)
                            Cells[i].Normal[2] += Cells[iSW].Normal[0];
                    }

                    if (y < Height - 1)
                    {
                        Cells[i].Normal[2] += Cells[iS].Normal[0];
                        Cells[i].Normal[4] += Cells[iS].Normal[0];

                        if (x < Width - 1)
                            Cells[i].Normal[4] += Cells[iSE].Normal[0];
                    }

                    if (x < Width - 1)
                    {
                        Cells[i].Normal[3] += Cells[iE].Normal[0];
                        Cells[i].Normal[4] += Cells[iE].Normal[0];
                    }

                    Cells[i].Normal[1].Normalize();
                    Cells[i].Normal[2].Normalize();
                    Cells[i].Normal[3].Normalize();
                    Cells[i].Normal[4].Normalize();
                }
            }
        }

        public void SetupVertices()
        {
            ObjectCount = 0;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int idx = y * Width + x;

                    if (Cells[idx].TileUp != -1)
                        ObjectCount++;

                    if (Cells[idx].TileSide != -1)
                        ObjectCount++;

                    if (Cells[idx].TileOtherSide != -1)
                        ObjectCount++;
                }
            }

            VertexPositionTextureNormalLightmap[] vertexdata = new VertexPositionTextureNormalLightmap[ObjectCount * 4];
            List<int>[] indexdata = new List<int>[Textures.Length];

            for (int i = 0; i < indexdata.Length; i++)
            {
                indexdata[i] = new List<int>();
            }

            int curSurface = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int idx = y * Width + x;

                    if (Cells[idx].TileUp != -1)
                    {
                        int tid = _surfaces[Cells[idx].TileUp].Texture;

                        SetupSurface(vertexdata, indexdata[tid], Cells[idx].TileUp, curSurface, x, y, 0);

                        curSurface++;
                    }

                    if (Cells[idx].TileSide != -1)
                    {
                        int tid = _surfaces[Cells[idx].TileSide].Texture;

                        SetupSurface(vertexdata, indexdata[tid], Cells[idx].TileSide, curSurface, x, y, 1);

                        curSurface++;
                    }

                    if (Cells[idx].TileOtherSide != -1)
                    {
                        int tid = _surfaces[Cells[idx].TileOtherSide].Texture;

                        SetupSurface(vertexdata, indexdata[tid], Cells[idx].TileOtherSide, curSurface, x, y, 2);

                        curSurface++;
                    }
                }
            }

            VertexBuffer = new VertexBuffer(VertexPositionTextureNormalLightmap.VertexDeclaration);
            VertexBuffer.SetData(vertexdata.ToArray(), BufferUsageHint.StaticDraw);

            IndexBuffers = new IndexBuffer[Textures.Length];
            for (int i = 0; i < IndexBuffers.Length; i++)
            {
                IndexBuffers[i] = new IndexBuffer(DrawElementsType.UnsignedInt);
                IndexBuffers[i].SetData(indexdata[i].ToArray(), BufferUsageHint.StaticDraw);
            }
        }

        private void SetupSurface(VertexPositionTextureNormalLightmap[] vertexdata, List<int> indexdata, int surface_id, int current_surface, int x, int y, int type)
        {
            int idx = current_surface * 4;
            int cellIdx = y * Width + x;
            Cell cell = Cells[cellIdx];

            Surface surface = _surfaces[surface_id];

            Vector3[] position = new Vector3[4];
            Vector3[] normal = new Vector3[4];

            switch (type)
            {
                case 0:
                    {
                        float x0 = (x - Width / 2) * Zoom;
                        float x1 = (x - Width / 2 + 1) * Zoom;

                        float z0 = (y - Height / 2) * Zoom;
                        float z1 = (y - Height / 2 + 1) * Zoom;

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
                        Cell cell2 = Cells[(y + 1) * Width + x];

                        float x0 = (x - Width / 2) * Zoom;
                        float x1 = (x - Width / 2 + 1) * Zoom;

                        float z0 = (y - Height / 2 + 1) * Zoom;

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
                        Cell cell2 = Cells[y * Width + x + 1];

                        float x0 = (x - Width / 2 + 1) * Zoom;

                        float z0 = (y - Height / 2) * Zoom;
                        float z1 = (y - Height / 2 + 1) * Zoom;

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

            int lmW = (int)Math.Floor(Math.Sqrt(Lightmaps.Length));
            int lmH = (int)Math.Ceiling((float)Lightmaps.Length / lmW);
            int lmX = (int)Math.Floor((float)surface.Lightmap / lmH);
            int lmY = surface.Lightmap % lmH;

            float[] lightmapU = new float[2];
            float[] lightmapV = new float[2];
            lightmapU[0] = (float)(0.1f + lmX) / lmW;
            lightmapU[1] = (float)(0.9f + lmX) / lmW;
            lightmapV[0] = (float)(0.1f + lmY) / lmH;
            lightmapV[1] = (float)(0.9f + lmY) / lmH;

            vertexdata[idx + 0] = new VertexPositionTextureNormalLightmap(position[0], normal[0], surface.TexCoord[0], new Vector2(lightmapU[0], lightmapV[0]), surface.Color);
            vertexdata[idx + 1] = new VertexPositionTextureNormalLightmap(position[1], normal[1], surface.TexCoord[1], new Vector2(lightmapU[1], lightmapV[0]), surface.Color);
            vertexdata[idx + 2] = new VertexPositionTextureNormalLightmap(position[2], normal[2], surface.TexCoord[2], new Vector2(lightmapU[0], lightmapV[1]), surface.Color);
            vertexdata[idx + 3] = new VertexPositionTextureNormalLightmap(position[3], normal[3], surface.TexCoord[3], new Vector2(lightmapU[1], lightmapV[1]), surface.Color);

            indexdata.Add(idx + 0);
            indexdata.Add(idx + 1);
            indexdata.Add(idx + 2);
            indexdata.Add(idx + 2);
            indexdata.Add(idx + 1);
            indexdata.Add(idx + 3);
        }

        public void Draw()
        {
            VertexBuffer.Bind();

            for (int i = 0; i < Textures.Length; i++)
            {
                ShaderProgram.Begin();
                ShaderProgram.SetTexture(Textures[i]);
                VertexBuffer.Render(BeginMode.Triangles, IndexBuffers[i], IndexBuffers[i].Count);
                ShaderProgram.End();
            }
        }
    }
}
