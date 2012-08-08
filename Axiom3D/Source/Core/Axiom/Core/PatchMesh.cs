#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: PatchMesh.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Graphics;
using ResourceHandle = System.UInt64;

#endregion Namespace Declarations

namespace Axiom.Core
{
    /// <summary>
    ///   Patch specialization of <see cref="Mesh" />.
    /// </summary>
    /// <remarks>
    ///   Instances of this class should be created by calling
    ///   <see cref="MeshManager.CreateBezierPatch" />.
    /// </remarks>
    public class PatchMesh : Mesh
    {
        #region Fields

        /// <summary>
        ///   Internal surface definition.
        /// </summary>
        protected PatchSurface patchSurface = new PatchSurface();

        /// <summary>
        ///   Vertex declaration, cloned from the input.
        /// </summary>
        protected VertexDeclaration vertexDeclaration;

        #endregion Fields

        /// <summary>
        ///   Creates a new PatchMesh.
        /// </summary>
        /// <remarks>
        ///   As defined in <see cref="MeshManager.CreateBezierPatch" />.
        /// </remarks>
        public PatchMesh(ResourceManager parent, string name, ResourceHandle handle, string group)
            : base(parent, name, handle, group, false, null)
        {
        }


        public void Define(Array controlPointArray, VertexDeclaration declaration, int width, int height,
                           int uMaxSubdivisionLevel, int vMaxSubdivisionLevel, VisibleSide visibleSide,
                           BufferUsage vbUsage,
                           BufferUsage ibUsage, bool vbUseShadow, bool ibUseShadow)
        {
            VertexBufferUsage = vbUsage;
            UseVertexShadowBuffer = vbUseShadow;
            IndexBufferUsage = ibUsage;
            UseIndexShadowBuffer = ibUseShadow;

            // Init patch builder
            // define the surface
            // NB clone the declaration to make it independent
            this.vertexDeclaration = (VertexDeclaration) declaration.Clone();
            this.patchSurface.DefineSurface(controlPointArray, this.vertexDeclaration, width, height,
                                            PatchSurfaceType.Bezier,
                                            uMaxSubdivisionLevel, vMaxSubdivisionLevel, visibleSide);
        }

        public float Subdivision
        {
            get { return this.patchSurface.SubdivisionFactor; }
            set
            {
                this.patchSurface.SubdivisionFactor = value;
                SubMesh sm = GetSubMesh(0);
                sm.indexData.indexCount = this.patchSurface.CurrentIndexCount;
            }
        }

        protected override void load()
        {
            SubMesh sm = CreateSubMesh();
            sm.vertexData = new VertexData();
            sm.useSharedVertices = false;

            // Set up the vertex buffer
            sm.vertexData.vertexStart = 0;
            sm.vertexData.vertexCount = this.patchSurface.RequiredVertexCount;
            sm.vertexData.vertexDeclaration = this.vertexDeclaration;

            HardwareVertexBuffer buffer =
                HardwareBufferManager.Instance.CreateVertexBuffer(this.vertexDeclaration.Clone(0),
                                                                  sm.vertexData.vertexCount, VertexBufferUsage,
                                                                  UseVertexShadowBuffer);

            // bind the vertex buffer
            sm.vertexData.vertexBufferBinding.SetBinding(0, buffer);

            // create the index buffer
            sm.indexData.indexStart = 0;
            sm.indexData.indexCount = this.patchSurface.RequiredIndexCount;
            sm.indexData.indexBuffer = HardwareBufferManager.Instance.CreateIndexBuffer(IndexType.Size16,
                                                                                        sm.indexData.indexCount,
                                                                                        IndexBufferUsage,
                                                                                        UseIndexShadowBuffer);

            // build the path
            this.patchSurface.Build(buffer, 0, sm.indexData.indexBuffer, 0);

            // set the bounds
            BoundingBox = this.patchSurface.Bounds;
            BoundingSphereRadius = this.patchSurface.BoundingSphereRadius;
        }
    }
}