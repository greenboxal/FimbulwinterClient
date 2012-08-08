#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: HLSLProgramFactory.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Core;
using Axiom.Graphics;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9.HLSL
{
    /// <summary>
    ///   Summary description for HLSLProgramFactory.
    /// </summary>
    public class D3D9HLSLProgramFactory : HighLevelGpuProgramFactory
    {
        private string language = "hlsl";

        #region HighLevelGpuProgramFactory Implementation

        /// <summary>
        ///   Gets the high level language that this factory handles requests for.
        /// </summary>
        [OgreVersion(1, 7, 2)]
        public override string Language
        {
            get { return this.language; }
        }

        [OgreVersion(1, 7, 2)]
        public override HighLevelGpuProgram CreateInstance(ResourceManager creator, string name, ulong handle,
                                                           string group,
                                                           bool isManual, IManualResourceLoader loader)
        {
            return new D3D9HLSLProgram(creator, name, handle, group, isManual, loader);
        }

        #endregion HighLevelGpuProgramFactory Implementation
    };
}