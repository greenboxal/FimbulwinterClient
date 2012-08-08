using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;
using Axiom.Core.Collections;
using FimbulwinterClient.Core.Content.World.Internals;
using System.Runtime.InteropServices;

namespace FimbulwinterClient.Core.Content.World
{
    public class GroundRenderable : MovableObject, IRenderable
    {
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

        private GndWorld _ground;

        public GndWorld Ground
        {
            get { return _ground; }
        }

        private RswWorld _world;

        public RswWorld World
        {
            get { return _world; }
        }

        private AxisAlignedBox _boudingBox;

        public override AxisAlignedBox BoundingBox
        {
            get { return _boudingBox; }
        }

        public override Real BoundingRadius
        {
            get { return 0; }
        }

        public bool CastsShadows
        {
            get { return false; }
        }

        public Material Material
        {
            get { return null; }
        }

        public Technique Technique
        {
            get { return null; }
        }

        public RenderOperation RenderOperation
        {
            get { return null; }
        }

        public LightList Lights
        {
            get { return QueryLights(); }
        }

        public bool NormalizeNormals
        {
            get { return false; }
        }

        public ushort NumWorldTransforms
        {
            get { return 1; }
        }

        public bool UseIdentityProjection
        {
            get { return false; }
        }

        public bool UseIdentityView
        {
            get { return false; }
        }

        public bool PolygonModeOverrideable
        {
            get { return true; }
        }

        public Quaternion WorldOrientation
        {
            get { return parentNode.DerivedOrientation; }
        }

        public Vector3 WorldPosition
        {
            get { return parentNode.DerivedPosition; }
        }

        public override uint TypeFlags
        {
            get { return (uint) SceneQueryTypeMask.WorldGeometry; }
        }

        public void GetWorldTransforms(Matrix4[] matrices)
        {
            parentNode.GetWorldTransforms(matrices);
        }

        public override void UpdateRenderQueue(RenderQueue queue)
        {
            queue.AddRenderable(this);
        }

        public Real GetSquaredViewDepth(Camera camera)
        {
            return 0; // (_center - camera.DerivedPosition).LengthSquared;
        }

        public Vector4 GetCustomParameter(int index)
        {
            return default(Vector4);
        }

        public void SetCustomParameter(int index, Vector4 val)
        {
        }

        public void UpdateCustomGpuParameter(GpuProgramParameters.AutoConstantEntry constant,
                                             GpuProgramParameters parameters)
        {
        }

        private Material[] _materials;

        public Material[] Materials
        {
            get { return _materials; }
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

        public GroundRenderable(GndWorld ground, RswWorld world)
        {
            _ground = ground;
            _world = world;

            SetupMaterials();
            SetupVertices();

            _boudingBox = new AxisAlignedBox(new Vector3(-500), new Vector3(500));
        }

        private void SetupMaterials()
        {
            _materials = new Material[_ground.Textures.Length];
            for (int i = 0; i < _ground.Textures.Length; i++)
            {
                Material m = (Material) MaterialManager.Instance.Create("worldTexture" + i, "World");
                Pass p = m.GetTechnique(0).GetPass(0);
                TextureUnitState t = p.CreateTextureUnitState(@"data\texture\" + _ground.Textures[i]);

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
        }

        private int objectCount;

        public void SetupVertices()
        {
            objectCount = 0;

            for (int x = 0; x < _ground.Width; x++)
            {
                for (int y = 0; y < _ground.Height; y++)
                {
                    int idx = y*_ground.Width + x;

                    if (_ground.Cells[idx].TileUp != -1)
                        objectCount++;

                    if (_ground.Cells[idx].TileSide != -1)
                        objectCount++;

                    if (_ground.Cells[idx].TileOtherSide != -1)
                        objectCount++;
                }
            }

            GndVertex[] vertexdata = new GndVertex[objectCount*4];
            List<int>[] indexdata = new List<int>[_materials.Length];
            for (int i = 0; i < indexdata.Length; i++)
            {
                indexdata[i] = new List<int>();
            }

            int cur_surface = 0;
            for (int x = 0; x < _ground.Width; x++)
            {
                for (int y = 0; y < _ground.Height; y++)
                {
                    int idx = y*_ground.Width + x;

                    if (_ground.Cells[idx].TileUp != -1)
                    {
                        int tid = _ground.Surfaces[_ground.Cells[idx].TileUp].Texture;

                        SetupSurface(vertexdata, indexdata[tid], _ground.Cells[idx].TileUp, cur_surface, x, y, 0);

                        cur_surface++;
                    }

                    if (_ground.Cells[idx].TileSide != -1)
                    {
                        int tid = _ground.Surfaces[_ground.Cells[idx].TileSide].Texture;

                        SetupSurface(vertexdata, indexdata[tid], _ground.Cells[idx].TileSide, cur_surface, x, y, 1);

                        cur_surface++;
                    }

                    if (_ground.Cells[idx].TileOtherSide != -1)
                    {
                        int tid = _ground.Surfaces[_ground.Cells[idx].TileOtherSide].Texture;

                        SetupSurface(vertexdata, indexdata[tid], _ground.Cells[idx].TileOtherSide, cur_surface, x, y, 2);

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
            offset += VertexElement.GetTypeSize(VertexElementType.Float2);
            //decl.AddElement(0, offset, VertexElementType.Float2, VertexElementSemantic.TexCoords, 1);
            //offset += VertexElement.GetTypeSize(VertexElementType.Float2);

            HardwareVertexBuffer vbuff = HardwareBufferManager.Instance.CreateVertexBuffer(decl.Clone(0),
                                                                                           vertexdata.Length,
                                                                                           BufferUsage.DynamicWriteOnly);
            vbuff.WriteData(0, Memory.SizeOf(typeof (GndVertex))*vertexdata.Length, vertexdata.ToArray());

            _vertices.vertexBufferBinding.SetBinding(0, vbuff);
            _vertices.vertexStart = 0;
            _vertices.vertexCount = vertexdata.Length;

            _indexes = new HardwareIndexBuffer[_materials.Length];
            for (int i = 0; i < _indexes.Length; i++)
            {
                HardwareIndexBuffer ibuff = HardwareBufferManager.Instance.CreateIndexBuffer(IndexType.Size32,
                                                                                             indexdata[i].Count,
                                                                                             BufferUsage.
                                                                                                 DynamicWriteOnly);

                ibuff.WriteData(0, Memory.SizeOf(typeof (int))*indexdata[i].Count, indexdata[i].ToArray());

                _indexes[i] = ibuff;
            }
        }

        private void SetupSurface(GndVertex[] vertexdata, List<int> indexdata, int surface_id, int current_surface,
                                  int x, int y, int type)
        {
            int idx = current_surface*4;
            int cell_idx = y*_ground.Width + x;
            GndWorld.Cell cell = _ground.Cells[cell_idx];

            GndWorld.Surface surface = _ground.Surfaces[surface_id];

            Vector3[] position = new Vector3[4];
            Vector3[] normal = new Vector3[4];

            switch (type)
            {
                case 0:
                    {
                        float x0 = (x - _ground.Width/2)*_ground.Zoom;
                        float x1 = (x - _ground.Width/2 + 1)*_ground.Zoom;

                        float z0 = (y - _ground.Height/2)*_ground.Zoom;
                        float z1 = (y - _ground.Height/2 + 1)*_ground.Zoom;

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
                        GndWorld.Cell cell2 = _ground.Cells[(y + 1)*_ground.Width + x];

                        float x0 = (x - _ground.Width/2)*_ground.Zoom;
                        float x1 = (x - _ground.Width/2 + 1)*_ground.Zoom;

                        float z0 = (y - _ground.Height/2 + 1)*_ground.Zoom;

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
                        GndWorld.Cell cell2 = _ground.Cells[y*_ground.Width + x + 1];

                        float x0 = (x - _ground.Width/2 + 1)*_ground.Zoom;

                        float z0 = (y - _ground.Height/2)*_ground.Zoom;
                        float z1 = (y - _ground.Height/2 + 1)*_ground.Zoom;

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

            int lm_w = (int) Math.Floor(Math.Sqrt(_ground.Lightmaps.Length));
            int lm_h = (int) Math.Ceiling((float) _ground.Lightmaps.Length/lm_w);
            int lm_x = (int) Math.Floor((float) surface.Lightmap/lm_h);
            int lm_y = surface.Lightmap%lm_h;

            float[] lightmapU = new float[2];
            float[] lightmapV = new float[2];
            lightmapU[0] = (0.1f + lm_x)/lm_w;
            lightmapU[1] = (0.9f + lm_x)/lm_w;
            lightmapV[0] = (0.1f + lm_y)/lm_h;
            lightmapV[1] = (0.9f + lm_y)/lm_h;

            vertexdata[idx + 0] = new GndVertex(position[0], normal[0], surface.TexCoord[0],
                                                new Vector2(lightmapU[0], lightmapV[0]), surface.Color);
            vertexdata[idx + 1] = new GndVertex(position[1], normal[1], surface.TexCoord[1],
                                                new Vector2(lightmapU[1], lightmapV[0]), surface.Color);
            vertexdata[idx + 2] = new GndVertex(position[2], normal[2], surface.TexCoord[2],
                                                new Vector2(lightmapU[0], lightmapV[1]), surface.Color);
            vertexdata[idx + 3] = new GndVertex(position[3], normal[3], surface.TexCoord[3],
                                                new Vector2(lightmapU[1], lightmapV[1]), surface.Color);

            indexdata.Add(idx + 0);
            indexdata.Add(idx + 1);
            indexdata.Add(idx + 2);
            indexdata.Add(idx + 2);
            indexdata.Add(idx + 1);
            indexdata.Add(idx + 3);
        }
    }
}