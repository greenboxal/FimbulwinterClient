#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: GLSurfaceDesc.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Graphics;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    /// <summary>
    ///   GL surface descriptor. Points to a 2D surface that can be rendered to.
    /// </summary>
    internal struct GLSurfaceDesc
    {
        public GLHardwarePixelBuffer Buffer;
        public int ZOffset;
    }
}