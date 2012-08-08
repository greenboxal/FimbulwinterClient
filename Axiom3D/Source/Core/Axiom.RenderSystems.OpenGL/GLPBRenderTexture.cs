#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id:"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Media;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    internal class GLPBRenderTexture : GLRenderTexture
    {
        #region Fields and Properties

        protected GLPBRTTManager manager;
        protected PixelComponentType pbFormat;

        #endregion Fields and Properties

        #region Construction and Destruction

        public GLPBRenderTexture(GLPBRTTManager manager, string name, GLSurfaceDesc target, bool writeGamma, int fsaa)
            : base(name, target, writeGamma, fsaa)
        {
            this.manager = manager;

            this.pbFormat = PixelUtil.GetComponentType(target.Buffer.Format);
            manager.RequestPBuffer(this.pbFormat, Width, Height);
        }

        #endregion Construction and Destruction

        #region GLRenderTexture Implementation

        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    this.manager.ReleasePBuffer(this.pbFormat);
                }
            }
            base.dispose(disposeManagedResources);
        }

        #endregion GLRenderTexture Implementation

        #region Methods

        public override object this[string attribute]
        {
            get
            {
                switch (attribute.ToUpper())
                {
                    case "TARGET":
                        GLSurfaceDesc target = new GLSurfaceDesc();
                        target.Buffer = (GLHardwarePixelBuffer) pixelBuffer;
                        target.ZOffset = zOffset;
                        return target;
                        break;
                    case "GLCONTEXT":
                        // Get PBuffer for our internal format
                        return this.manager.GetContextFor(this.pbFormat, Width, Height);
                        break;
                    default:
                        return base[attribute];
                        break;
                }
            }
        }

        #endregion Methods
    }
}