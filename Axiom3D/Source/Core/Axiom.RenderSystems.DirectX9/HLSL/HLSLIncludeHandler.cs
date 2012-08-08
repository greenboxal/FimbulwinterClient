#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: HLSLIncludeHandler.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System.IO;
using Axiom.Core;
using D3D9 = SharpDX.Direct3D9;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9.HLSL
{
    public class HLSLIncludeHandler : DisposableObject, D3D9.Include
    {
        protected Resource program;

        public System.IDisposable Shadow { get; set; }


        [OgreVersion(1, 7, 2)]
        public HLSLIncludeHandler(Resource sourceProgram)
        {
            this.program = sourceProgram;
        }

        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed && disposeManagedResources)
            {
                this.program = null;
            }

            base.dispose(disposeManagedResources);
        }

        public void Open(D3D9.IncludeType type, string fileName, out Stream fileStream)
        {
            fileStream = ResourceGroupManager.Instance.OpenResource(fileName, this.program.Group, true, this.program);
        }

        public Stream Open(D3D9.IncludeType type, string fileName, Stream parentStream)
        {
            return ResourceGroupManager.Instance.OpenResource(fileName, this.program.Group, true, this.program);
        }

        public void Close(Stream fileStream)
        {
            fileStream.Close();
        }
    };
}