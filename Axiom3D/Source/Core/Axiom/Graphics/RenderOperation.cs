#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: RenderOperation.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    ///<summary>
    ///  Contains all the information required to render a set of vertices.  This includes
    ///  a list of VertexBuffers.
    ///</summary>
    ///<remarks>
    ///  This class contains
    ///</remarks>
    public class RenderOperation : DisposableObject
    {
        #region Member variables

        ///<summary>
        ///  Type of operation to perform.
        ///</summary>
        public OperationType operationType;

        ///<summary>
        ///  Contains a list of hardware vertex buffers for this complete render operation.
        ///</summary>
        public VertexData vertexData;

        ///<summary>
        ///  When <code>useIndices</code> is set to true, this must hold a reference to an index
        ///  buffer containing indices into the vertices stored here.
        ///</summary>
        public IndexData indexData;

        ///<summary>
        ///  Specifies whether or not a list of indices should be used when rendering the vertices in
        ///  the buffers.
        ///</summary>
        public bool useIndices;

        /// <summary>
        ///   The number of instances for the render operation - this option is supported 
        ///   in only a part of the render systems.
        /// </summary>
        public int numberOfInstances;

        /// <summary>
        /// </summary>
        public bool useGlobalInstancingVertexBufferIsAvailable;

        #endregion

        #region Constructors

        ///<summary>
        ///  Default constructor.
        ///</summary>
        public RenderOperation()
        {
            this.numberOfInstances = 1;
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="disposeManagedResources"> </param>
        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    if (this.vertexData != null)
                    {
                        if (!this.vertexData.IsDisposed)
                        {
                            this.vertexData.Dispose();
                        }

                        this.vertexData = null;
                    }

                    if (this.indexData != null)
                    {
                        if (!this.indexData.IsDisposed)
                        {
                            this.indexData.Dispose();
                        }

                        this.indexData = null;
                    }
                }
            }

            base.dispose(disposeManagedResources);
        }
    }
}