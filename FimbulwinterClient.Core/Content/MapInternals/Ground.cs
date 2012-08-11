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
using OpenTK.Graphics;
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
                    TexCoord[0].Y = 1 - br.ReadSingle();

                    TexCoord[1].X = br.ReadSingle();
                    TexCoord[1].Y = 1 - br.ReadSingle();

                    TexCoord[2].X = br.ReadSingle();
                    TexCoord[2].Y = 1 - br.ReadSingle();

                    TexCoord[3].X = br.ReadSingle();
                    TexCoord[3].Y = 1 - br.ReadSingle();

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

                    TexCoord[0].Y = 1 - br.ReadSingle();
                    TexCoord[1].Y = 1 - br.ReadSingle();
                    TexCoord[2].Y = 1 - br.ReadSingle();
                    TexCoord[3].Y = 1 - br.ReadSingle();

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
        public string[] Textures { get; private set; }

        public Lightmap[] Lightmaps { get; private set; }
        public Cell[] Cells { get; private set; }
        private Surface[] _surfaces;
        public Surface[] Surfaces
        {
            get { return _surfaces; }
        }

        public int ObjectCount { get; set; }

        protected byte MinorVersion;
        protected byte MajorVersion;

        public VertexBuffer VertexBuffer { get; private set; }
        public IndexBuffer[] IndexBuffers { get; private set; }
        public Texture2D LightmapTexture { get; private set; }
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

            Textures = new string[textureCount];
            for (int i = 0; i < Textures.Length; i++)
            {
                Textures[i] = br.ReadCString(texChunkSize);
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

            CalculateNormals();

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
    }
}
