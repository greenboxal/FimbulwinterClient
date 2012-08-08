#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: GLFBORenderTexture.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
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
    internal class GLFBORenderTexture : GLRenderTexture
    {
        #region Fields and Properties

        private readonly GLFrameBufferObject _fbo;

        #endregion Fields and Properties

        #region Construction and Destruction

        public GLFBORenderTexture(GLFBORTTManager manager, string name, GLSurfaceDesc target, bool writeGamma, int fsaa)
            : base(name, target, writeGamma, fsaa)
        {
            this._fbo = new GLFrameBufferObject(manager);

            // Bind target to surface 0 and initialise
            this._fbo.BindSurface(0, target);

            // Get attributes
            width = this._fbo.Width;
            height = this._fbo.Height;
        }

        #endregion Construction and Destruction

        #region GLRenderTexture Implementation

        public override object this[string attribute]
        {
            get
            {
                switch (attribute.ToLower())
                {
                    case "fbo":
                        return this._fbo;
                    default:
                        return null;
                }
            }
        }

        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    // Dispose managed resources.
                    this._fbo.Dispose();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            base.dispose(disposeManagedResources);
        }

        #endregion GLRenderTexture Implementation
    }
}