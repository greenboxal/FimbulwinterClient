using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using FimbulvetrEngine;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using FimbulwinterClient.Core.Content.MapInternals;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FimbulwinterClient.Core.Graphics
{
    public partial class WorldRenderer
    {
        public VertexBuffer GroundBuffer { get; private set; }
        public Tuple<Texture2D, IndexBuffer>[] GroundMeshes { get; private set; }
        public Texture2D GroundLightmap { get; private set; }
        public GroundShaderProgram GroundShaderProgram { get; private set; }

        private enum CellTileType
        {
            Top,
            Side,
            OtherSide
        }

        private void LoadGround(bool background)
        {
            Dispatcher.Instance.DispatchCoreTask(o =>
                {
                    GroundShaderProgram = new GroundShaderProgram();
                });

            CreateGroundBuffers(background);
            CreateGroundLightmap(background);
        }

        private void CreateGroundLightmap(bool background)
        {
            if (Map.Ground.Lightmaps.Length == 0)
                return;

            int w = (int)Math.Floor(Math.Sqrt(Map.Ground.Lightmaps.Length));
            int h = (int)Math.Ceiling((float)Map.Ground.Lightmaps.Length / w);

            Color4[] color = new Color4[8 * 8 * w * h];

            int x = 0, y = 0;
            foreach (Ground.Lightmap t in Map.Ground.Lightmaps)
            {
                for (int j = 0; j < 8; j++)
                {
                    int offset = y * w * 8 * 8 + j * w * 8 + x * 8;

                    for (int n = 0; n < 8; n++)
                    {
                        color[offset + n] = Color.FromArgb(t.Brightness[j * 8 + n], t.Intensity[j * 8 + n]);
                    }
                }

                if (++y >= h)
                {
                    y = 0;
                    x++;
                }
            }

            Dispatcher.Instance.DispatchCoreTask(o =>
            {
                GroundLightmap = new Texture2D(w * 8, h * 8);
                GroundLightmap.SetData(PixelFormat.Rgba, PixelInternalFormat.Rgba, PixelType.Float, color);
            });
        }

        private void CreateGroundBuffers(bool background)
        {
            int objectCount = 0;
            int currentSurface = 0;

            for (int x = 0; x < Map.Ground.Width; x++)
            {
                for (int y = 0; y < Map.Ground.Height; y++)
                {
                    Ground.Cell cell = Map.Ground.Cells[y * Map.Ground.Width + x];

                    if (cell.TileUp != -1)
                        objectCount++;

                    if (cell.TileSide != -1)
                        objectCount++;

                    if (cell.TileOtherSide != -1)
                        objectCount++;
                }
            }

            VertexPositionTextureNormalLightmap[] vertexdata = new VertexPositionTextureNormalLightmap[objectCount * 4];
            List<int>[] indexdata = new List<int>[Map.Ground.Textures.Length];

            for (int i = 0; i < indexdata.Length; i++)
                indexdata[i] = new List<int>();

            for (int x = 0; x < Map.Ground.Width; x++)
            {
                for (int y = 0; y < Map.Ground.Height; y++)
                {
                    Ground.Cell cell = Map.Ground.Cells[y * Map.Ground.Width + x];

                    if (cell.TileUp != -1)
                    {
                        int tid = Map.Ground.Surfaces[cell.TileUp].Texture;

                        SetupGroundSurface(vertexdata, indexdata[tid], cell.TileUp, currentSurface, x, y, CellTileType.Top);

                        currentSurface++;
                    }

                    if (cell.TileSide != -1)
                    {
                        int tid = Map.Ground.Surfaces[cell.TileSide].Texture;

                        SetupGroundSurface(vertexdata, indexdata[tid], cell.TileSide, currentSurface, x, y, CellTileType.Side);

                        currentSurface++;
                    }

                    if (cell.TileOtherSide != -1)
                    {
                        int tid = Map.Ground.Surfaces[cell.TileOtherSide].Texture;

                        SetupGroundSurface(vertexdata, indexdata[tid], cell.TileOtherSide, currentSurface, x, y, CellTileType.OtherSide);

                        currentSurface++;
                    }
                }
            }


            Dispatcher.Instance.DispatchCoreTask(o =>
            {
                GroundBuffer = new VertexBuffer(VertexPositionTextureNormalLightmap.VertexDeclaration);
                GroundMeshes = new Tuple<Texture2D, IndexBuffer>[Map.Ground.Textures.Length];

                GroundBuffer.SetData(vertexdata.ToArray(), BufferUsageHint.StaticDraw);
                GroundMeshes = new Tuple<Texture2D, IndexBuffer>[Map.Ground.Textures.Length];
                for (int i = 0; i < GroundMeshes.Length; i++)
                {
                    Texture2D texture = ContentManager.Instance.Load<Texture2D>(@"data\texture\" + Map.Ground.Textures[i], true);

                    IndexBuffer buffer = new IndexBuffer(DrawElementsType.UnsignedInt);
                    buffer.SetData(indexdata[i].ToArray(), BufferUsageHint.StaticDraw);

                    GroundMeshes[i] = new Tuple<Texture2D, IndexBuffer>(texture, buffer);
                }
            });
        }

        private void SetupGroundSurface(IList<VertexPositionTextureNormalLightmap> vertexdata, ICollection<int> indexdata, int surfaceId, int currentSurface, int x, int y, CellTileType cellTileType)
        {
            int idx = currentSurface * 4;
            int cellIdx = y * Map.Ground.Width + x;

            Ground.Cell cell = Map.Ground.Cells[cellIdx];
            Ground.Surface surface = Map.Ground.Surfaces[surfaceId];

            Vector3[] position = new Vector3[4];
            Vector3[] normal = new Vector3[4];

            switch (cellTileType)
            {
                case CellTileType.Top:
                    {
                        float x0 = (x - Map.Ground.Width / 2) * Map.Ground.Zoom;
                        float x1 = (x - Map.Ground.Width / 2 + 1) * Map.Ground.Zoom;

                        float z0 = (y - Map.Ground.Height / 2) * Map.Ground.Zoom;
                        float z1 = (y - Map.Ground.Height / 2 + 1) * Map.Ground.Zoom;

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
                case CellTileType.Side:
                    {
                        Ground.Cell cell2 = Map.Ground.Cells[(y + 1) * Map.Ground.Width + x];

                        float x0 = (x - Map.Ground.Width / 2) * Map.Ground.Zoom;
                        float x1 = (x - Map.Ground.Width / 2 + 1) * Map.Ground.Zoom;

                        float z0 = (y - Map.Ground.Height / 2 + 1) * Map.Ground.Zoom;

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
                case CellTileType.OtherSide:
                    {
                        Ground.Cell cell2 = Map.Ground.Cells[y * Map.Ground.Width + x + 1];

                        float x0 = (x - Map.Ground.Width / 2 + 1) * Map.Ground.Zoom;

                        float z0 = (y - Map.Ground.Height / 2) * Map.Ground.Zoom;
                        float z1 = (y - Map.Ground.Height / 2 + 1) * Map.Ground.Zoom;

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

            int lmW = (int)Math.Floor(Math.Sqrt(Map.Ground.Lightmaps.Length));
            int lmH = (int)Math.Ceiling((float)Map.Ground.Lightmaps.Length / lmW);
            int lmX = (int)Math.Floor((float)surface.Lightmap / lmH);
            int lmY = surface.Lightmap % lmH;

            float[] lightmapU = new float[2];
            float[] lightmapV = new float[2];
            lightmapU[0] = (0.1f + lmX) / lmW;
            lightmapU[1] = (0.9f + lmX) / lmW;
            lightmapV[0] = (0.1f + lmY) / lmH;
            lightmapV[1] = (0.9f + lmY) / lmH;

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
    }
}
