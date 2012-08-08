#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: DefaultHardwareBufferManagerBase.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    /// <summary>
    ///   Specialization of HardwareBufferManagerBase to emulate hardware buffers.
    /// </summary>
    /// <remarks>
    ///   You might want to instantiate this class if you want to utilize
    ///   classes like MeshSerializer without having initialized the 
    ///   rendering system (which is required to create a 'real' hardware
    ///   buffer manager.
    /// </remarks>
    public class DefaultHardwareBufferManagerBase : HardwareBufferManagerBase
    {
        ~DefaultHardwareBufferManagerBase()
        {
        }

        /// Creates a vertex buffer
        public override HardwareVertexBuffer CreateVertexBuffer(VertexDeclaration vertexDeclaration, int numVerts,
                                                                BufferUsage usage, bool useShadowBuffer)
        {
            DefaultHardwareVertexBuffer vb = new DefaultHardwareVertexBuffer(this, vertexDeclaration, numVerts, usage);
            return vb;
        }

        /// Create a hardware vertex buffer
        public override HardwareIndexBuffer CreateIndexBuffer(IndexType itype, int numIndices, BufferUsage usage,
                                                              bool useShadowBuffer)
        {
            DefaultHardwareIndexBuffer ib = new DefaultHardwareIndexBuffer(itype, numIndices, usage);
            return ib;
        }

        /// Create a hardware vertex buffer
        //RenderToVertexBuffer createRenderToVertexBuffer();
        protected override void dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                //DisposeAllDeclarations();
                //DisposeAllBindings();
            }
            base.dispose(disposeManagedResources);
        }
    };
}