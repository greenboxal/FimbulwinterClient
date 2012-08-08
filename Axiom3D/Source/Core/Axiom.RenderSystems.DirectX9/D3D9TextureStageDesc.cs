#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: D3D9TextureStageDesc.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Core;
using Axiom.Graphics;
using D3D9 = SharpDX.Direct3D9;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9
{
    ///<summary>
    ///  Structure holding texture unit settings for every stage
    ///</summary>
    [OgreVersion(1, 7, 2)]
    internal struct D3D9TextureStageDesc
    {
        /// <summary>
        ///   The type of the texture
        /// </summary>
        public D3D9TextureType TexType;

        /// <summary>
        ///   Which texCoordIndex to use
        /// </summary>
        public int CoordIndex;

        /// <summary>
        ///   Type of auto tex. calc. used
        /// </summary>
        public TexCoordCalcMethod AutoTexCoordType;

        /// <summary>
        ///   Frustum, used if the above is projection
        /// </summary>
        public Frustum Frustum;

        /// <summary>
        ///   Texture
        /// </summary>
        public D3D9.BaseTexture Tex;

        /// <summary>
        ///   Vertex texture
        /// </summary>
        public D3D9.BaseTexture VertexTex;
    };
}