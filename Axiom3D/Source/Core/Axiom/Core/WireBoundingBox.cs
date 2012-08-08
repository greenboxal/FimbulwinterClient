#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: WireBoundingBox.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Math;
using Axiom.Graphics;

#endregion Namespace Declarations

namespace Axiom.Core
{
    /// <summary>
    ///   Summary description for WireBoundingBox.
    /// </summary>
    public class WireBoundingBox : SimpleRenderable
    {
        #region Constants

        private const int PositionBinding = 0;

        #endregion Constants

        #region Field and Properties

        protected Real Radius;

        public new AxisAlignedBox BoundingBox
        {
            get { return base.BoundingBox; }
            set
            {
                // init the vertices to the aabb
                SetupBoundingBoxVertices(value);

                // setup the bounding box of this SimpleRenderable
                box = value;
            }
        }

        #endregion Field and Properties

        #region Constructors

        /// <summary>
        ///   Default constructor.
        /// </summary>
        public WireBoundingBox()
        {
            vertexData = new VertexData();
            vertexData.vertexCount = 24;
            vertexData.vertexStart = 0;

            renderOperation.vertexData = vertexData;
            renderOperation.operationType = OperationType.LineList;
            renderOperation.useIndices = false;

            // get a reference to the vertex declaration and buffer binding
            VertexDeclaration decl = vertexData.vertexDeclaration;
            VertexBufferBinding binding = vertexData.vertexBufferBinding;

            // add elements for position and color only
            decl.AddElement(PositionBinding, 0, VertexElementType.Float3, VertexElementSemantic.Position);

            // create a new hardware vertex buffer for the position data
            HardwareVertexBuffer buffer = HardwareBufferManager.Instance.CreateVertexBuffer(
                decl.Clone(PositionBinding), vertexData.vertexCount,
                BufferUsage.StaticWriteOnly);

            // bind the position buffer
            binding.SetBinding(PositionBinding, buffer);

            material = (Material) MaterialManager.Instance["BaseWhiteNoLighting"];
        }

        #endregion Constructors

        #region Methods

        [Obsolete("Use WireBoundingBox.BoundingBox property.")]
        public void InitAABB(AxisAlignedBox box)
        {
            // store the bounding box locally
            BoundingBox = box;
        }

        [Obsolete("Use WireBoundingBox.BoundingBox property.")]
        public void SetupBoundingBox(AxisAlignedBox aabb)
        {
            // store the bounding box locally
            BoundingBox = box;
        }

        protected virtual void SetupBoundingBoxVertices(AxisAlignedBox aab)
        {
            Vector3 vmax = aab.Maximum;
            Vector3 vmin = aab.Minimum;

            float sqLen = System.Math.Max(vmax.LengthSquared, vmin.LengthSquared);
            //mRadius = System.Math.Sqrt(sqLen);

            float maxx = vmax.x;
            float maxy = vmax.y;
            float maxz = vmax.z;

            float minx = vmin.x;
            float miny = vmin.y;
            float minz = vmin.z;

            HardwareVertexBuffer buffer = vertexData.vertexBufferBinding.GetBuffer(PositionBinding);

#if !AXIOM_SAFE_ONLY
            unsafe
#endif
            {
                float* posPtr = buffer.Lock(BufferLocking.Discard).ToFloatPointer();
                int pPos = 0;

                // line 0
                posPtr[pPos++] = minx;
                posPtr[pPos++] = miny;
                posPtr[pPos++] = minz;
                posPtr[pPos++] = maxx;
                posPtr[pPos++] = miny;
                posPtr[pPos++] = minz;
                // line 1
                posPtr[pPos++] = minx;
                posPtr[pPos++] = miny;
                posPtr[pPos++] = minz;
                posPtr[pPos++] = minx;
                posPtr[pPos++] = miny;
                posPtr[pPos++] = maxz;
                // line 2
                posPtr[pPos++] = minx;
                posPtr[pPos++] = miny;
                posPtr[pPos++] = minz;
                posPtr[pPos++] = minx;
                posPtr[pPos++] = maxy;
                posPtr[pPos++] = minz;
                // line 3
                posPtr[pPos++] = minx;
                posPtr[pPos++] = maxy;
                posPtr[pPos++] = minz;
                posPtr[pPos++] = minx;
                posPtr[pPos++] = maxy;
                posPtr[pPos++] = maxz;
                // line 4
                posPtr[pPos++] = minx;
                posPtr[pPos++] = maxy;
                posPtr[pPos++] = minz;
                posPtr[pPos++] = maxx;
                posPtr[pPos++] = maxy;
                posPtr[pPos++] = minz;
                // line 5
                posPtr[pPos++] = maxx;
                posPtr[pPos++] = miny;
                posPtr[pPos++] = minz;
                posPtr[pPos++] = maxx;
                posPtr[pPos++] = miny;
                posPtr[pPos++] = maxz;
                // line 6
                posPtr[pPos++] = maxx;
                posPtr[pPos++] = miny;
                posPtr[pPos++] = minz;
                posPtr[pPos++] = maxx;
                posPtr[pPos++] = maxy;
                posPtr[pPos++] = minz;
                // line 7
                posPtr[pPos++] = minx;
                posPtr[pPos++] = maxy;
                posPtr[pPos++] = maxz;
                posPtr[pPos++] = maxx;
                posPtr[pPos++] = maxy;
                posPtr[pPos++] = maxz;
                // line 8
                posPtr[pPos++] = minx;
                posPtr[pPos++] = maxy;
                posPtr[pPos++] = maxz;
                posPtr[pPos++] = minx;
                posPtr[pPos++] = miny;
                posPtr[pPos++] = maxz;
                // line 9
                posPtr[pPos++] = maxx;
                posPtr[pPos++] = maxy;
                posPtr[pPos++] = minz;
                posPtr[pPos++] = maxx;
                posPtr[pPos++] = maxy;
                posPtr[pPos++] = maxz;
                // line 10
                posPtr[pPos++] = maxx;
                posPtr[pPos++] = miny;
                posPtr[pPos++] = maxz;
                posPtr[pPos++] = maxx;
                posPtr[pPos++] = maxy;
                posPtr[pPos++] = maxz;
                // line 11
                posPtr[pPos++] = minx;
                posPtr[pPos++] = miny;
                posPtr[pPos++] = maxz;
                posPtr[pPos++] = maxx;
                posPtr[pPos++] = miny;
                posPtr[pPos] = maxz;
            }
            buffer.Unlock();
        }

        #endregion Methods

        #region Implementation of SimpleRenderable

        ///<summary>
        ///</summary>
        ///<param name="matrices"> </param>
        public override void GetWorldTransforms(Matrix4[] matrices)
        {
            // return identity matrix to prevent parent transforms
            matrices[0] = Matrix4.Identity;
        }

        ///<summary>
        ///</summary>
        ///<param name="camera"> </param>
        ///<returns> </returns>
        public override Real GetSquaredViewDepth(Camera camera)
        {
            Vector3 min = box.Minimum,
                    max = box.Maximum,
                    mid = ((max - min)*0.5f) + min,
                    dist = camera.DerivedPosition - mid;

            return dist.LengthSquared;
        }

        /// <summary>
        ///   Get the local bounding radius of the wire bounding box.
        /// </summary>
        public override Real BoundingRadius
        {
            get { return this.Radius; }
        }

        #endregion Implementation of SimpleRenderable
    }
}