#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: D3D9TextureType.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

namespace Axiom.RenderSystems.DirectX9
{
    ///<summary>
    ///  Enum identifying D3D9 texture types
    ///</summary>
    [OgreVersion(1, 7, 2)]
    public enum D3D9TextureType
    {
        /// <summary>
        ///   Standard texture
        /// </summary>
        Normal,

        /// <summary>
        ///   Cube texture
        /// </summary>
        Cube,

        /// <summary>
        ///   Volume texture
        /// </summary>
        Volume,

        /// <summary>
        ///   Just to have it...
        /// </summary>
        None
    };
}