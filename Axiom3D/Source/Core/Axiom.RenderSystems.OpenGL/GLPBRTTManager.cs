#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: GLPBRTTManager.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
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
    internal class GLPBRTTManager : GLRTTManager
    {
        #region Inner Classes and Structures

        /// <summary>
        ///   Provides Usage counting for PixelBuffers
        /// </summary>
        private struct PixelBufferUsage
        {
            public GLPBuffer PixelBuffer;
            public uint InUseCount;
        };

        #endregion Inner Classes and Structures

        #region Fields and Properties

        private readonly BaseGLSupport _glSupport;
        private readonly GLContext _mainGLContext;
        private readonly RenderTarget _mainWindow;

        private readonly PixelBufferUsage[] pBuffers = new PixelBufferUsage[(int) PixelComponentType.Count];

        #endregion Fields and Properties

        #region Construction and Destruction

        internal GLPBRTTManager(BaseGLSupport glSupport, RenderTarget target)
            : base(glSupport)
        {
            this._glSupport = glSupport;
            this._mainWindow = target;

            this._mainGLContext = (GLContext) target.GetCustomAttribute("GLCONTEXT");
        }

        #endregion Construction and Destruction

        #region GLRTTManager Implementation

        public override RenderTexture CreateRenderTexture(string name, GLSurfaceDesc target, bool writeGamma, int fsaa)
        {
            return new GLPBRenderTexture(this, name, target, writeGamma, fsaa);
        }

        public override bool CheckFormat(PixelFormat format)
        {
            return true;
        }

        public override void Bind(RenderTarget target)
        {
            // Nothing to do here
            // Binding of context is done by GL subsystem, as contexts are also used for RenderWindows
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

        protected override void dispose(bool disposeManagedResources)
        {
            if (!isDisposed)
            {
                if (disposeManagedResources)
                {
                    for (int i = 0; i < (int) PixelComponentType.Count; i++)
                    {
                        this.pBuffers[i].PixelBuffer = null;
                    }
                }
            }
            base.dispose(disposeManagedResources);
        }

        #endregion GLRTTManager Implementation

        #region Methods

        /// <summary>
        ///   Create PBuffer for a certain pixel format and size
        /// </summary>
        /// <param name="pcType"> </param>
        /// <param name="width"> </param>
        /// <param name="height"> </param>
        public void RequestPBuffer(PixelComponentType pcType, int width, int height)
        {
            // Check Size
            GLPBuffer pBuffer = this.pBuffers[(int) pcType].PixelBuffer;
            if (pBuffer != null)
            {
                if (pBuffer.Width < width || pBuffer.Height < height)
                {
                    // if the current buffer is too small destroy it and recreate it
                    pBuffer = null;
                    this.pBuffers[(int) pcType].PixelBuffer = null;
                }
            }

            if (pBuffer == null)
            {
                // create pixelbuffer via rendersystem
                this.pBuffers[(int) pcType].PixelBuffer = this._glSupport.CreatePBuffer(pcType, width, height);
            }
            this.pBuffers[(int) pcType].InUseCount++;
        }

        /// <summary>
        ///   Release PBuffer for a certain pixel format
        /// </summary>
        /// <param name="pcType"> </param>
        public void ReleasePBuffer(PixelComponentType pcType)
        {
            --this.pBuffers[(int) pcType].InUseCount;
            if (this.pBuffers[(int) pcType].InUseCount == 0)
            {
                this.pBuffers[(int) pcType].PixelBuffer = null;
            }
        }

        /// <summary>
        ///   Get GL rendering context for a certain component type and size.
        /// </summary>
        /// <param name="pcType"> </param>
        /// <param name="width"> </param>
        /// <param name="height"> </param>
        /// <returns> </returns>
        public GLContext GetContextFor(PixelComponentType pcType, int width, int height)
        {
            // Faster to return main context if the RTT is smaller than the window size
            // and pcType is PixelComponentType.Byte. This must be checked every time because the window might have been resized
            if (pcType == PixelComponentType.Byte)
            {
                if (width <= this._mainWindow.Width && height <= this._mainWindow.Height)
                {
                    return this._mainGLContext;
                }
            }
            Debug.Assert(this.pBuffers[(int) pcType].PixelBuffer != null);
            return this.pBuffers[(int) pcType].PixelBuffer.Context;
        }

        #endregion Methods
    }
}