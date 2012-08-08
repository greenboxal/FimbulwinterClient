#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: HardwareIndexBuffer.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Runtime.InteropServices;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    ///<summary>
    ///  Describes the graphics API independent functionality required by a hardware
    ///  index buffer.
    ///</summary>
    ///<remarks>
    ///  NB subclasses should override lock, unlock, readData, writeData
    ///</remarks>
    public abstract class HardwareIndexBuffer : HardwareBuffer
    {
        #region Fields

        protected HardwareBufferManagerBase Manager;

        #endregion Fields

        #region Properties and Fields

        #region Type

        ///<summary>
        ///  Type of index (16 or 32 bit).
        ///</summary>
        protected IndexType type;

        ///<summary>
        ///  Gets an enum specifying whether this index buffer is 16 or 32 bit elements.
        ///</summary>
        public IndexType Type
        {
            get { return this.type; }
        }

        #endregion Type

        #region IndexCount

        ///<summary>
        ///  Number of indices in this buffer.
        ///</summary>
        protected int numIndices;

        ///<summary>
        ///  Gets the number of indices in this buffer.
        ///</summary>
        public int IndexCount
        {
            get { return this.numIndices; }
        }

        #endregion IndexCount

        #region IndexSize

        /// <summary>
        ///   Size of each index.
        /// </summary>
        protected int indexSize;

        /// <summary>
        ///   Gets the size (in bytes) of each index element.
        /// </summary>
        /// <value> </value>
        public int IndexSize
        {
            get { return this.indexSize; }
        }

        #endregion IndexSize

        #endregion Properties and Fields

        #region Construction and Destruction

        ///<summary>
        ///  Constructor.
        ///</summary>
        ///<param name="type"> Type of index (16 or 32 bit). </param>
        ///<param name="numIndices"> Number of indices to create in this buffer. </param>
        ///<param name="usage"> Buffer usage. </param>
        ///<param name="useSystemMemory"> Create in system memory? </param>
        ///<param name="useShadowBuffer"> Use a shadow buffer for reading/writing? </param>
        [OgreVersion(1, 7, 2)]
        public HardwareIndexBuffer(HardwareBufferManagerBase manager, IndexType type, int numIndices, BufferUsage usage,
                                   bool useSystemMemory, bool useShadowBuffer)
            : base(usage, useSystemMemory, useShadowBuffer)
        {
            this.type = type;
            this.numIndices = numIndices;
            this.Manager = manager;
            // calc the index buffer size
            sizeInBytes = numIndices;

            if (type == IndexType.Size32)
            {
                this.indexSize = Memory.SizeOf(typeof (int));
            }
            else
            {
                this.indexSize = Memory.SizeOf(typeof (short));
            }

            sizeInBytes *= this.indexSize;

            // create a shadow buffer if required
            if (useShadowBuffer)
            {
                shadowBuffer = new DefaultHardwareIndexBuffer(this.Manager, type, numIndices, BufferUsage.Dynamic);
            }
        }

        [OgreVersion(1, 7, 2, "~HardwareIndexBuffer")]
        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    if (this.Manager != null)
                    {
                        this.Manager.NotifyIndexBufferDestroyed(this);
                    }

                    shadowBuffer.SafeDispose();
                    shadowBuffer = null;
                }
            }

            base.dispose(disposeManagedResources);
        }

        #endregion Construction and Destruction
    };
}