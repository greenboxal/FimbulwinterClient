#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: GpuProgramParameters.GpuConstantDefinitionMap.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

#endregion Namespace Declarations

using System.Collections.Generic;

namespace Axiom.Graphics
{
    [OgreVersion(1, 7, 2790)]
    public partial class GpuProgramParameters
    {
        /// <summary>
        ///   Named Gpu constant lookup table
        /// </summary>
        [OgreVersion(1, 7, 2790)]
        public class GpuConstantDefinitionMap : Dictionary<string, GpuConstantDefinition>
        {
            public static GpuConstantDefinitionMap Empty
            {
                get { return new GpuConstantDefinitionMap(); }
            }
        }
    }
}