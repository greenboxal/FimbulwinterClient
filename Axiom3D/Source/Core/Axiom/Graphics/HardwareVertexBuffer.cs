#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: HardwareVertexBuffer.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    ///<summary>
    ///  Describes the graphics API independent functionality required by a hardware
    ///  vertex buffer.
    ///</summary>
    ///<remarks>
    ///</remarks>
    public abstract class HardwareVertexBuffer : HardwareBuffer
    {
        #region Member variables

        protected HardwareBufferManagerBase Manager;
        protected int numVertices;
        protected VertexDeclaration vertexDeclaration;
        protected int useCount;

        #endregion

        #region Construction and destruction

        [OgreVersion(1, 7, 2)]
        public HardwareVertexBuffer(HardwareBufferManagerBase manager, VertexDeclaration vertexDeclaration,
                                    int numVertices,
                                    BufferUsage usage, bool useSystemMemory, bool useShadowBuffer)
            : base(usage, useSystemMemory, useShadowBuffer)
        {
            this.vertexDeclaration = vertexDeclaration;
            this.numVertices = numVertices;
            this.Manager = manager;

            // calculate the size in bytes of this buffer
            sizeInBytes = vertexDeclaration.GetVertexSize()*numVertices;

            // create a shadow buffer if required
            if (useShadowBuffer)
            {
                shadowBuffer = new DefaultHardwareVertexBuffer(this.Manager, vertexDeclaration, numVertices,
                                                               BufferUsage.Dynamic);
            }

            this.useCount = 0;
        }

        [OgreVersion(1, 7, 2, "~HardwareVertexBuffer")]
        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    if (this.Manager != null)
                    {
                        this.Manager.NotifyVertexBufferDestroyed(this);
                    }

                    shadowBuffer.SafeDispose();
                    shadowBuffer = null;
                }
            }

            base.dispose(disposeManagedResources);
        }

        #endregion Construction and destruction

        #region Properties

        public VertexDeclaration VertexDeclaration
        {
            get { return this.vertexDeclaration; }
        }

        public int VertexSize
        {
            get { return this.vertexDeclaration.GetVertexSize(); }
        }

        public int VertexCount
        {
            get { return this.numVertices; }
        }

        public int UseCount
        {
            get { return this.useCount; }
        }

        #region IsInstanceData

        [OgreVersion(1, 7, 2790)] protected bool isInstanceData;

        [OgreVersion(1, 7, 2790)]
        public bool IsInstanceData
        {
            get { return this.isInstanceData; }
            set
            {
                if (value && !CheckIfVertexInstanceDataIsSupported())
                {
                    throw new AxiomException("vertex instance data is not supported by the render system.");
                }
                // else
                {
                    this.isInstanceData = value;
                }
            }
        }

        #endregion

        #region InstanceDataStepRate

        [OgreVersion(1, 7, 2790)] protected int instanceDataStepRate;

        [OgreVersion(1, 7, 2790)]
        public int InstanceDataStepRate
        {
            get { return this.instanceDataStepRate; }
            set
            {
                if (value > 0)
                {
                    this.instanceDataStepRate = value;
                }
                else
                {
                    throw new AxiomException("Instance data step rate must be bigger then 0.");
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///   Checks if vertex instance data is supported by the render system
        /// </summary>
        /// <returns> </returns>
        protected virtual bool CheckIfVertexInstanceDataIsSupported()
        {
            // Use the current render system
            RenderSystem rs = Root.Instance.RenderSystem;

            // Check if the supported  
            throw new NotImplementedException();
            //return rs.Capabilities.HasCapability(Capabilities.VertexBufferInstanceData);
        }

        #endregion
    }
}