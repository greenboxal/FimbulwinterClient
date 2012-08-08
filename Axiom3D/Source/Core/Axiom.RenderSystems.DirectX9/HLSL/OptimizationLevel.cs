#region SVN Version Information

// <file>
//     <license see="http://axiomengine.sf.net/wiki/index.php/license.txt"/>
//     <id value="$Id $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Scripting;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9.HLSL
{
    /// <summary>
    ///   Shader optimization level
    /// </summary>
    [OgreVersion(1, 7, 2)]
    public enum OptimizationLevel
    {
        /// <summary>
        ///   Default optimization - no optimization in debug mode, LevelOne in release
        /// </summary>
        [ScriptEnum("default")] Default,

        /// <summary>
        ///   No optimization
        /// </summary>
        [ScriptEnum("none")] None,

        /// <summary>
        ///   Optimization level 0
        /// </summary>
        [ScriptEnum("0")] LevelZero,

        /// <summary>
        ///   Optimization level 1
        /// </summary>
        [ScriptEnum("1")] LevelOne,

        /// <summary>
        ///   Optimization level 2
        /// </summary>
        [ScriptEnum("2")] LevelTwo,

        /// <summary>
        ///   Optimization level 3
        /// </summary>
        [ScriptEnum("3")] LevelThree
    };
}