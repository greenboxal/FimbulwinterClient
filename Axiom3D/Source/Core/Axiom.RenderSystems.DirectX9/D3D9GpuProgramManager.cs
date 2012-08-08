#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: D3D9GpuProgramManager.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Collections;
using Axiom.Core;
using Axiom.Graphics;
using ResourceHandle = System.UInt64;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9
{
    /// <summary>
    ///   Summary description for D3DGpuProgramManager.
    /// </summary>
    public class D3D9GpuProgramManager : GpuProgramManager
    {
        [OgreVersion(1, 7, 2)]
        internal D3D9GpuProgramManager()
        {
            // Superclass sets up members 

            // Register with resource group manager
            ResourceGroupManager.Instance.RegisterResourceManager(ResourceType, this);
        }

        [OgreVersion(1, 7, 2, "~D3D9GpuProgramManager")]
        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    // Unregister with resource group manager
                    ResourceGroupManager.Instance.UnregisterResourceManager(ResourceType);
                }
            }

            base.dispose(disposeManagedResources);
        }

        #region GpuProgramManager Implementation

        /// <see
        ///   cref="Axiom.Core.ResourceManager._create(string, ResourceHandle, string, bool, IManualResourceLoader, NameValuePairList)" />
        [OgreVersion(1, 7, 2)]
        protected override Resource _create(string name, ResourceHandle handle, string group, bool isManual,
                                            IManualResourceLoader loader, NameValuePairList createParams)
        {
            if (createParams == null || !createParams.ContainsKey("type"))
            {
                throw new AxiomException("You must supply a 'type' parameter.");
            }

            if (createParams["type"] == "vertex_program")
            {
                return new D3D9GpuVertexProgram(this, name, handle, group, isManual, loader);
            }
            else
            {
                return new D3D9GpuFragmentProgram(this, name, handle, group, isManual, loader);
            }
        }

        /// <summary>
        ///   Specialised create method with specific parameters
        /// </summary>
        [OgreVersion(1, 7, 2)]
        protected override Resource _create(string name, ResourceHandle handle, string group, bool isManual,
                                            IManualResourceLoader loader, GpuProgramType type, string syntaxCode)
        {
            if (type == GpuProgramType.Vertex)
            {
                return new D3D9GpuVertexProgram(this, name, handle, group, isManual, loader);
            }
            else
            {
                return new D3D9GpuFragmentProgram(this, name, handle, group, isManual, loader);
            }
        }

        #endregion GpuProgramManager Implementation
    };
}