#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: TriangleBuilder.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Axiom.Collections;
using Axiom.Core;
using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    /// <summary>
    ///   General utility class for collecting the set of all
    ///   triangles in a mesh; used for picking
    /// </summary>
    public class TriangleListBuilder : AnyBuilder
    {
        #region Methods

        public IEnumerable<TriangleVertices> Build()
        {
            List<TriangleVertices> triangles = new List<TriangleVertices>();

            //Iterate index sets
            for (int indexSet = 0; indexSet < indexDataList.Count; indexSet++)
            {
                IndexData indexData = indexDataList[indexSet];
                VertexData vertexData = vertexDataList[indexDataVertexDataSetList[indexSet]];
                IndexType indexType = indexData.indexBuffer.Type;
                OperationType opType = operationTypes[indexSet];

                int iterations = 0;

                switch (opType)
                {
                    case OperationType.TriangleList:
                        iterations = indexData.indexCount/3;
                        break;

                    case OperationType.TriangleFan:
                    case OperationType.TriangleStrip:
                        iterations = indexData.indexCount - 2;
                        break;

                    default:
                        throw new AxiomException("Operation type not supported: {0}", opType);
                }

                triangles.Capacity = triangles.Count + iterations;

                //Get the vertex buffer
                VertexElement posElement =
                    vertexData.vertexDeclaration.FindElementBySemantic(VertexElementSemantic.Position);
                HardwareVertexBuffer vertexBuffer = vertexData.vertexBufferBinding.GetBuffer(posElement.Source);

                //Lock buffers
                BufferBase vxPtr = vertexBuffer.Lock(BufferLocking.ReadOnly);
                try
                {
                    BufferBase idxPtr = indexData.indexBuffer.Lock(BufferLocking.ReadOnly);
                    try
                    {
#if !AXIOM_SAFE_ONLY
                        unsafe
#endif
                        {
                            BufferBase pVertexPos = vxPtr + posElement.Offset; // positional element of the base vertex
                            //float* pReal;										 // for vector component retrieval
                            int icount = 0; // index into the index buffer
                            int index; // index into the vertex buffer
                            short* p16Idx = idxPtr.ToShortPointer();
                            int* p32Idx = idxPtr.ToIntPointer();

                            // iterate over all the groups of 3 indices
                            for (int t = 0; t < iterations; t++)
                            {
                                Vector3[] v = new Vector3[3];
                                    //vertices of a single triangle, new instance needed each iteration

                                //assemble a triangle
                                for (int i = 0; i < 3; i++)
                                {
                                    // standard 3-index read for tri list or first tri in strip / fan
                                    if (opType == OperationType.TriangleList || t == 0)
                                    {
                                        if (indexType == IndexType.Size32)
                                        {
                                            index = p32Idx[icount++];
                                        }
                                        else
                                        {
                                            index = p16Idx[icount++];
                                        }
                                    }
                                    else
                                    {
                                        // Strips and fans are formed from last 2 indexes plus the
                                        // current one for triangles after the first

                                        if (indexType == IndexType.Size32)
                                        {
                                            index = p32Idx[icount + i - 2];
                                        }
                                        else
                                        {
                                            index = p16Idx[icount + i - 2];
                                        }

                                        if (i == 2)
                                        {
                                            icount++;
                                        }
                                    }

                                    //retrieve vertex position
                                    float* pReal = (pVertexPos + (index*vertexBuffer.VertexSize)).ToFloatPointer();
                                    v[i].x = pReal[0];
                                    v[i].y = pReal[1];
                                    v[i].z = pReal[2];
                                }

                                // Put the points in in counter-clockwise order
                                if (((v[0].x - v[2].x)*(v[1].y - v[2].y) - (v[1].x - v[2].x)*(v[0].y - v[2].y)) < 0)
                                {
                                    // Clockwise, so reverse points 1 and 2
                                    Vector3 tmp = v[1];
                                    v[1] = v[2];
                                    v[2] = tmp;
                                }

                                Debug.Assert(
                                    ((v[0].x - v[2].x)*(v[1].y - v[2].y) - (v[1].x - v[2].x)*(v[0].y - v[2].y)) >= 0,
                                    "Failed to arrange triangle points counter-clockwise.");

                                // Add to the list of triangles
                                triangles.Add(new TriangleVertices(v));
                            } // for iterations
                        } // unsafe
                    }
                    finally
                    {
                        indexData.indexBuffer.Unlock();
                    }
                }
                finally
                {
                    vertexBuffer.Unlock();
                }
            }

            return triangles;
        }

        #endregion Methods
    }

    public class TriangleVertices
    {
        protected Vector3[] vertices;

        public TriangleVertices(Vector3[] vertices)
        {
            this.vertices = vertices;
        }

        public Vector3[] Vertices
        {
            get { return this.vertices; }
        }
    }

    public class TriangleIntersector
    {
        //changes:
        // instead of 'Vector3 modelBase' utilizes Matrix4 specifying
        // transforms that need to be applied to each triangle before testing for
        // intersection.

        #region Fields

        protected List<TriangleVertices> triangles;

        #endregion Fields

        #region Contructor

        public TriangleIntersector(IEnumerable<TriangleVertices> triangles)
        {
            this.triangles = new List<TriangleVertices>(triangles);
        }

        #endregion Constructor

        #region Methods

        public bool ClosestRayIntersection(Ray ray, Vector3 modelBase, float ignoreDistance, out Vector3 intersection)
        {
            bool intersects = false;
            intersection = Vector3.Zero;
            float minDistSquared = float.MaxValue;
            float ignoreDistanceSquared = ignoreDistance*ignoreDistance;
            Vector3 where = Vector3.Zero;

            // Iterate over the triangles
            for (int i = 0; i < this.triangles.Count; i++)
            {
                TriangleVertices t = this.triangles[i];
                if (RayIntersectsTriangle(ray, t.Vertices, ref modelBase, ref where))
                {
                    float distSquared = (where - ray.Origin).LengthSquared;
                    if (distSquared > ignoreDistanceSquared && distSquared < minDistSquared)
                    {
                        minDistSquared = distSquared;
                        intersection = where;
                        intersects = true;
                    }
                }
            }
            return intersects;
        }

        public bool ClosestRayIntersection(Ray ray, Matrix4 transform, float ignoreDistance, out Vector3 intersection)
        {
            bool intersects = false;
            intersection = Vector3.Zero;
            float minDistSquared = float.MaxValue;
            float ignoreDistanceSquared = ignoreDistance*ignoreDistance;
            Vector3 where = Vector3.Zero;

            // Iterate over the triangles
            for (int i = 0; i < this.triangles.Count; i++)
            {
                TriangleVertices t = this.triangles[i];
                if (RayIntersectsTriangle(ray, t.Vertices, ref transform, ref where))
                {
                    float distSquared = (where - ray.Origin).LengthSquared;
                    if (distSquared > ignoreDistanceSquared && distSquared < minDistSquared)
                    {
                        minDistSquared = distSquared;
                        intersection = where;
                        intersects = true;
                    }
                }
            }
            return intersects;
        }

        // Given line pq and ccw triangle abc, return whether line pierces triangle. If
        // so, also return the barycentric coordinates (u,v,w) of the intersection point
        protected bool RayIntersectsTriangle(Ray ray, Vector3[] Vertices, ref Vector3 modelBase, ref Vector3 where)
        {
            // Place the end beyond any conceivable triangle, 1000 meters away
            Vector3 a = modelBase + Vertices[0];
            Vector3 b = modelBase + Vertices[1];
            Vector3 c = modelBase + Vertices[2];
            Vector3 start = ray.Origin;
            Vector3 end = start + ray.Direction*1000000f;
            Vector3 pq = end - start;
            Vector3 pa = a - start;
            Vector3 pb = b - start;
            Vector3 pc = c - start;
            // Test if pq is inside the edges bc, ca and ab. Done by testing
            // that the signed tetrahedral volumes, computed using scalar triple
            // products, are all positive
            float u = pq.Cross(pc).Dot(pb);
            if (u < 0.0f)
            {
                return false;
            }
            float v = pq.Cross(pa).Dot(pc);
            if (v < 0.0f)
            {
                return false;
            }
            float w = pq.Cross(pb).Dot(pa);
            if (w < 0.0f)
            {
                return false;
            }
            float denom = 1.0f/(u + v + w);
            // Finally fill in the intersection point
            where = (u*a + v*b + w*c)*denom;
            return true;
        }

        // Given line pq and ccw triangle abc, return whether line pierces triangle. If
        // so, also return the barycentric coordinates (u,v,w) of the intersection point
        protected bool RayIntersectsTriangle(Ray ray, Vector3[] Vertices, ref Matrix4 transform, ref Vector3 where)
        {
            // Place the end beyond any conceivable triangle, 1000 meters away
            Vector3 a = transform*Vertices[0];
            Vector3 b = transform*Vertices[1];
            Vector3 c = transform*Vertices[2];
            Vector3 start = ray.Origin;
            Vector3 end = start + ray.Direction*1000000f;
            Vector3 pq = end - start;
            Vector3 pa = a - start;
            Vector3 pb = b - start;
            Vector3 pc = c - start;
            // Test if pq is inside the edges bc, ca and ab. Done by testing
            // that the signed tetrahedral volumes, computed using scalar triple
            // products, are all positive
            float u = pq.Cross(pc).Dot(pb);
            if (u < 0.0f)
            {
                return false;
            }
            float v = pq.Cross(pa).Dot(pc);
            if (v < 0.0f)
            {
                return false;
            }
            float w = pq.Cross(pb).Dot(pa);
            if (w < 0.0f)
            {
                return false;
            }
            float denom = 1.0f/(u + v + w);
            // Finally fill in the intersection point
            where = (u*a + v*b + w*c)*denom;
            return true;
        }

        #endregion Protected Methods
    }
}