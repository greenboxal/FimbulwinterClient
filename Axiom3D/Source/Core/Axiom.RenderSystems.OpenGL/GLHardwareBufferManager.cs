#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: GLHardwareBufferManager.cs 3322 2012-07-16 20:58:00Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Utilities;
using Tao.OpenGl;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    /// <summary>
    ///   Summary description for GLHardwareBufferManager.
    /// </summary>
    public class GLHardwareBufferManagerBase : HardwareBufferManagerBase
    {
        #region Member variables

        #endregion

        #region Constructors

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="type"> </param>
        /// <param name="numIndices"> </param>
        /// <param name="usage"> </param>
        /// <param name="useShadowBuffer"> </param>
        /// <returns> </returns>
        [OgreVersion(1, 7, 2)]
        public override HardwareIndexBuffer CreateIndexBuffer(IndexType type, int numIndices, BufferUsage usage,
                                                              bool useShadowBuffer)
        {
            GLHardwareIndexBuffer buffer = new GLHardwareIndexBuffer(this, type, numIndices, usage, useShadowBuffer);
            lock (IndexBuffersMutex)
                indexBuffers.Add(buffer);
            return buffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="vertexSize"> </param>
        /// <param name="numVerts"> </param>
        /// <param name="usage"> </param>
        /// <param name="useShadowBuffer"> </param>
        /// <returns> </returns>
        [OgreVersion(1, 7, 2)]
        public override HardwareVertexBuffer CreateVertexBuffer(VertexDeclaration vertexDeclaration, int numVerts,
                                                                BufferUsage usage, bool useShadowBuffer)
        {
            GLHardwareVertexBuffer buffer = new GLHardwareVertexBuffer(this, vertexDeclaration, numVerts, usage,
                                                                       useShadowBuffer);
            lock (VertexBuffersMutex)
                vertexBuffers.Add(buffer);
            return buffer;
        }

        #endregion
    }

    public class GLHardwareBufferManager : HardwareBufferManager
    {
        public GLHardwareBufferManager()
            : base(new GLHardwareBufferManagerBase())
        {
        }

        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    _baseInstance.SafeDispose();
                }
            }

            base.dispose(disposeManagedResources);
        }

        public static int GetGLType(VertexElementType type)
        {
            switch (type)
            {
                case VertexElementType.Float1:
                case VertexElementType.Float2:
                case VertexElementType.Float3:
                case VertexElementType.Float4:
                    return Gl.GL_FLOAT;
                case VertexElementType.Short1:
                case VertexElementType.Short2:
                case VertexElementType.Short3:
                case VertexElementType.Short4:
                    return Gl.GL_SHORT;
                case VertexElementType.Color:
                case VertexElementType.Color_ABGR:
                case VertexElementType.Color_ARGB:
                case VertexElementType.UByte4:
                    return Gl.GL_UNSIGNED_BYTE;
                default:
                    return 0;
            }
            ;
        }
    }
}