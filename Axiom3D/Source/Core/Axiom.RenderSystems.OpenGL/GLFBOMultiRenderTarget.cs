#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: GLFBOMultiRenderTarget.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Graphics;
using Axiom.Utilities;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    internal class GLFBOMultiRenderTarget : MultiRenderTarget
    {
        #region Fields and Properties

        protected GLFBORTTManager _manager;
        protected GLFrameBufferObject _fbo;

        #endregion Fields and Properties

        #region Construction and Destruction

        public GLFBOMultiRenderTarget(GLFBORTTManager manager, string name)
            : base(name)
        {
            this._manager = manager;
        }

        #endregion Construction and Destruction

        #region Methods

        /// <summary>
        ///   Bind a surface to a certain attachment point.
        /// </summary>
        /// <param name="attachment"> 0 .. capabilities.MultiRenderTargetCount-1 </param>
        /// <param name="target"> RenderTexture to bind. </param>
        /// <remarks>
        ///   It does not bind the surface and fails with an exception (ERR_INVALIDPARAMS) if:
        ///   - Not all bound surfaces have the same size
        ///   - Not all bound surfaces have the same internal format
        /// </remarks>
        [OgreVersion(1, 7, 2)]
        protected override void BindSurfaceImpl(int attachment, RenderTexture target)
        {
            /// Check if the render target is in the rendertarget->FBO map
            GLFrameBufferObject fbObject = (GLFrameBufferObject) target["FBO"];
            Proclaim.NotNull(fbObject);

            this._fbo.BindSurface(attachment, fbObject.SurfaceDesc);

            // Initialize?

            // Set width and height
            width = this._fbo.Width;
            height = this._fbo.Height;
        }

        /// <summary>
        ///   Unbind Attachment
        /// </summary>
        [OgreVersion(1, 7, 2)]
        protected override void UnbindSurfaceImpl(int attachment)
        {
            this._fbo.UnbindSurface(attachment);
            width = this._fbo.Width;
            height = this._fbo.Height;
        }

        #endregion Methods

        #region RenderTarget Implementation

        public override object this[string attribute]
        {
            get
            {
                if (attribute == "FBO")
                {
                    return this._fbo;
                }

                return base[attribute];
            }
        }

        public override bool RequiresTextureFlipping
        {
            get { return true; }
        }

        #endregion RenderTarget Implementation
    }
}