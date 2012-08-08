#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: Pose.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Axiom.Collections;
using Axiom.Graphics;
using Axiom.Core;
using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Animating
{
    ///<summary>
    ///  A pose is a linked set of vertex offsets applying to one set of vertex data.
    ///</summary>
    ///<remarks>
    ///  The target index referred to by the pose has a meaning set by the user
    ///  of this class; but for example when used by Mesh it refers to either the
    ///  Mesh shared geometry (0) or a SubMesh dedicated geometry (1+).
    ///  Pose instances can be referred to by keyframes in VertexAnimationTrack in
    ///  order to animate based on blending poses together.
    ///</remarks>
    public class Pose
    {
        #region Protected Members

        /// <summary>
        ///   Target geometry index
        /// </summary>
        private readonly ushort target;

        /// Optional name
        private readonly string name;

        /// <summary>
        ///   Primary storage, sparse vertex use
        /// </summary>
        private readonly Dictionary<int, Vector3> vertexOffsetMap = new Dictionary<int, Vector3>();

        /// <summary>
        ///   Derived hardware buffer, covers all vertices
        /// </summary>
        private HardwareVertexBuffer vertexBuffer;

        #endregion Protected Members

        #region Constructors

        ///<summary>
        ///  Constructor
        ///</summary>
        ///<param name="target"> The target vertexdata index (0 for shared, 1+ for dedicated at the submesh index + 1 </param>
        ///<param name="name"> </param>
        public Pose(ushort target, string name)
        {
            this.target = target;
            this.name = name;
        }

        #endregion Constructors

        #region Properties

        public string Name
        {
            get { return this.name; }
        }

        public ushort Target
        {
            get { return this.target; }
        }

        public Dictionary<int, Vector3> VertexOffsetMap
        {
            get { return this.vertexOffsetMap; }
        }

        public HardwareVertexBuffer VertexBuffer
        {
            get { return this.vertexBuffer; }
        }

        #endregion Properties

        #region Public Methods

        /// <summary>
        ///   Adds an offset to a vertex for this pose.
        /// </summary>
        /// <param name="index"> The vertex index </param>
        /// <param name="offset"> The position offset for this pose </param>
        public void AddVertex(int index, Vector3 offset)
        {
            this.vertexOffsetMap[index] = offset;
            DisposeVertexBuffer();
        }

        /// <summary>
        ///   Remove a vertex offset.
        /// </summary>
        public void RemoveVertex(int index)
        {
            if (this.vertexOffsetMap.ContainsKey(index))
            {
                this.vertexOffsetMap.Remove(index);
            }
            DisposeVertexBuffer();
        }

        /// <summary>
        ///   Clear all vertex offsets.
        /// </summary>
        public void ClearVertexOffsets()
        {
            this.vertexOffsetMap.Clear();
            DisposeVertexBuffer();
        }

        protected void DisposeVertexBuffer()
        {
            if (this.vertexBuffer != null)
            {
                this.vertexBuffer.Dispose();
                this.vertexBuffer = null;
            }
        }

        /// <summary>
        ///   Get a hardware vertex buffer version of the vertex offsets.
        /// </summary>
        public HardwareVertexBuffer GetHardwareVertexBuffer(int numVertices)
        {
            if (this.vertexBuffer == null)
            {
                // Create buffer
                VertexDeclaration decl = HardwareBufferManager.Instance.CreateVertexDeclaration();
                decl.AddElement(0, 0, VertexElementType.Float3, VertexElementSemantic.Position);

                this.vertexBuffer = HardwareBufferManager.Instance.CreateVertexBuffer(decl, numVertices,
                                                                                      BufferUsage.StaticWriteOnly,
                                                                                      false);

                // lock the vertex buffer
                BufferBase ipBuf = this.vertexBuffer.Lock(BufferLocking.Discard);

#if !AXIOM_SAFE_ONLY
                unsafe
#endif
                {
                    float* buffer = ipBuf.ToFloatPointer();
                    for (int i = 0; i < numVertices*3; i++)
                    {
                        buffer[i] = 0f;
                    }

                    // Set each vertex
                    foreach (KeyValuePair<int, Vector3> pair in this.vertexOffsetMap)
                    {
                        int offset = 3*pair.Key;
                        Vector3 v = pair.Value;
                        buffer[offset++] = v.x;
                        buffer[offset++] = v.y;
                        buffer[offset] = v.z;
                    }
                    this.vertexBuffer.Unlock();
                }
            }
            return this.vertexBuffer;
        }

        #endregion Public Methods
    }
}