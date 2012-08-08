#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: GLCopyingRenderTexture.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    internal class GLCopyingRenderTexture : GLRenderTexture
    {
        public GLCopyingRenderTexture(GLCopyingRTTManager manager, string name, GLSurfaceDesc target, bool writeGamma,
                                      int fsaa)
            : base(name, target, writeGamma, fsaa)
        {
        }

        public override object this[string attribute]
        {
            get
            {
                if (attribute.ToLower() == "target")
                {
                    GLSurfaceDesc desc;
                    desc.Buffer = pixelBuffer as GLHardwarePixelBuffer;
                    desc.ZOffset = zOffset;
                    return desc;
                }

                return null;
            }
        }
    }
}