#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: GLCopyingRTTManager.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Axiom.Core;
using Axiom.Media;
using Axiom.Graphics;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    internal class GLCopyingRTTManager : GLRTTManager
    {
        #region Construction and Destruction

        internal GLCopyingRTTManager(BaseGLSupport glSupport)
            : base(glSupport)
        {
        }

        #endregion Construction and Destruction

        #region GLRTTManager Implementation

        public override RenderTexture CreateRenderTexture(string name, GLSurfaceDesc target, bool writeGamma, int fsaa)
        {
            return new GLCopyingRenderTexture(this, name, target, writeGamma, fsaa);
        }

        public override bool CheckFormat(PixelFormat format)
        {
            return true;
        }

        public override void Bind(RenderTarget target)
        {
            // nothing to do here
        }

        public override void Unbind(RenderTarget target)
        {
            // copy on unbind
            object attr = target.GetCustomAttribute("target");
            if (attr != null)
            {
                GLSurfaceDesc surface = (GLSurfaceDesc) attr;
                if (surface.Buffer != null)
                {
                    ((GLTextureBuffer) surface.Buffer).CopyFromFrameBuffer(surface.ZOffset);
                }
            }
        }

        #endregion GLRTTManager Implementation
    }
}