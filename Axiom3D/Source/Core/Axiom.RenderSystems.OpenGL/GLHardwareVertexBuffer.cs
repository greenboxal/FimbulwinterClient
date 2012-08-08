#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: GLHardwareVertexBuffer.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using Axiom.Graphics;
using Tao.OpenGl;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    /// <summary>
    ///   Summary description for GLHardwareVertexBuffer.
    /// </summary>
    public class GLHardwareVertexBuffer : HardwareVertexBuffer
    {
        #region Member variables

        /// <summary>
        ///   Saves the GL buffer ID for this buffer.
        /// </summary>
        private int bufferID;

        #endregion Member variables

        #region Constructors

        public GLHardwareVertexBuffer(HardwareBufferManagerBase manager, VertexDeclaration vertexDeclaration,
                                      int numVertices,
                                      BufferUsage usage, bool useShadowBuffer)
            : base(manager, vertexDeclaration, numVertices, usage, false, useShadowBuffer)
        {
            this.bufferID = 0;

            Gl.glGenBuffersARB(1, out this.bufferID);

            if (this.bufferID == 0)
            {
                throw new Exception("Cannot create GL vertex buffer");
            }

            Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, this.bufferID);

            // initialize this buffer.  we dont have data yet tho
            Gl.glBufferDataARB(Gl.GL_ARRAY_BUFFER_ARB, new IntPtr(sizeInBytes), IntPtr.Zero, GLHelper.ConvertEnum(usage));
            // TAO 2.0
            //Gl.glBufferDataARB( Gl.GL_ARRAY_BUFFER_ARB, sizeInBytes, IntPtr.Zero, GLHelper.ConvertEnum( usage ) );
        }

        #endregion Constructors

        #region HardwareVertexBuffer Implementation

        ///<summary>
        ///</summary>
        ///<param name="offset"> </param>
        ///<param name="length"> </param>
        ///<param name="locking"> </param>
        ///<returns> </returns>
        protected override BufferBase LockImpl(int offset, int length, BufferLocking locking)
        {
            int access = 0;

            if (isLocked)
            {
                throw new Exception("Invalid attempt to lock an index buffer that has already been locked.");
            }

            // bind this buffer
            Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, this.bufferID);

            if (locking == BufferLocking.Discard)
            {
                // fixes issues with ATI cards
                //Gl.glBufferDataARB(Gl.GL_ARRAY_BUFFER_ARB, length, IntPtr.Zero, GLHelper.ConvertEnum(usage));

                // find out how we shall access this buffer
                access = (usage == BufferUsage.Dynamic) ? Gl.GL_READ_WRITE_ARB : Gl.GL_WRITE_ONLY_ARB;
            }
            else if (locking == BufferLocking.ReadOnly)
            {
                if (usage == BufferUsage.WriteOnly)
                {
                    LogManager.Instance.Write("Invalid attempt to lock a write-only vertex buffer as read-only.");
                }

                access = Gl.GL_READ_ONLY_ARB;
            }
            else if (locking == BufferLocking.Normal || locking == BufferLocking.NoOverwrite)
            {
                access = (usage == BufferUsage.Dynamic) ? Gl.GL_READ_WRITE_ARB : Gl.GL_WRITE_ONLY_ARB;
            }

            IntPtr ptr = Gl.glMapBufferARB(Gl.GL_ARRAY_BUFFER_ARB, access);

            if (ptr == IntPtr.Zero)
            {
                throw new Exception("GL Vertex Buffer: Out of memory");
            }

            isLocked = true;

            return BufferBase.Wrap(new IntPtr(ptr.ToInt64() + offset), length);
        }

        ///<summary>
        ///</summary>
        protected override void UnlockImpl()
        {
            Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, this.bufferID);

            // Unmap the buffer to unlock it
            if (Gl.glUnmapBufferARB(Gl.GL_ARRAY_BUFFER_ARB) == 0)
            {
                throw new Exception("Buffer data currupted!");
            }

            isLocked = false;
        }

        ///<summary>
        ///</summary>
        ///<param name="offset"> </param>
        ///<param name="length"> </param>
        ///<param name="src"> </param>
        ///<param name="discardWholeBuffer"> </param>
        public override void WriteData(int offset, int length, BufferBase src, bool discardWholeBuffer)
        {
            Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, this.bufferID);

            if (useShadowBuffer)
            {
                // lock the buffer for reading
                BufferBase dest = shadowBuffer.Lock(offset, length,
                                                    discardWholeBuffer ? BufferLocking.Discard : BufferLocking.Normal);

                // copy that data in there
                Memory.Copy(src, dest, length);

                // unlock the buffer
                shadowBuffer.Unlock();
            }

            if (discardWholeBuffer)
            {
                Gl.glBufferDataARB(Gl.GL_ARRAY_BUFFER_ARB, new IntPtr(sizeInBytes), IntPtr.Zero,
                                   GLHelper.ConvertEnum(usage));
                // TAO 2.0
                //Gl.glBufferDataARB( Gl.GL_ARRAY_BUFFER_ARB,
                //    sizeInBytes,
                //    IntPtr.Zero,
                //    GLHelper.ConvertEnum( usage ) );
            }

            Gl.glBufferSubDataARB(Gl.GL_ARRAY_BUFFER_ARB, new IntPtr(offset), new IntPtr(length), src.Pin()); // TAO 2.0
            src.UnPin();
            //Gl.glBufferSubDataARB(
            //    Gl.GL_ARRAY_BUFFER_ARB,
            //    offset,
            //    length,
            //    src );
        }

        ///<summary>
        ///</summary>
        ///<param name="offset"> </param>
        ///<param name="length"> </param>
        ///<param name="dest"> </param>
        public override void ReadData(int offset, int length, BufferBase dest)
        {
            if (useShadowBuffer)
            {
                // lock the buffer for reading
                BufferBase src = shadowBuffer.Lock(offset, length, BufferLocking.ReadOnly);

                // copy that data in there
                Memory.Copy(src, dest, length);

                // unlock the buffer
                shadowBuffer.Unlock();
            }
            else
            {
                Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, this.bufferID);

                Gl.glGetBufferSubDataARB(Gl.GL_ARRAY_BUFFER_ARB, new IntPtr(offset), new IntPtr(length), dest.Pin());
                // TAO 2.0
                dest.UnPin();
                //Gl.glGetBufferSubDataARB(
                //    Gl.GL_ARRAY_BUFFER_ARB,
                //    offset,
                //    length,
                //    dest );
            }
        }

        /// <summary>
        ///   Called to destroy this buffer.
        /// </summary>
        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                }

                try
                {
                    Gl.glDeleteBuffersARB(1, ref this.bufferID);
                }
                catch (AccessViolationException ave)
                {
                    LogManager.Instance.Write("Error Deleting Vertexbuffer[{0}].", this.bufferID);
                }
            }

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            base.dispose(disposeManagedResources);
        }

        #endregion HardwareVertexBuffer Implementation

        #region Properties

        ///<summary>
        ///  Gets the GL buffer ID for this buffer.
        ///</summary>
        public int GLBufferID
        {
            get { return this.bufferID; }
        }

        #endregion Properties
    }
}