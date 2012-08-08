﻿#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: D3D9MultiRenderTarget.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Configuration;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Media;
using Axiom.Utilities;
using D3D9 = SharpDX.Direct3D9;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9
{
    public sealed class D3D9MultiRenderTarget : MultiRenderTarget
    {
        #region Fields and Properties

        private readonly D3D9HardwarePixelBuffer[] _renderTargets =
            new D3D9HardwarePixelBuffer[Config.MaxMultipleRenderTargets];

        #endregion Fields and Properties

        #region Construction and Destruction

        [OgreVersion(1, 7, 2)]
        public D3D9MultiRenderTarget(string name)
            : base(name)
        {
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
            Contract.Requires(attachment < Config.MaxMultipleRenderTargets);

            // Get buffer and surface to bind to
            D3D9HardwarePixelBuffer buffer = (D3D9HardwarePixelBuffer) (target["BUFFER"]);
            Proclaim.NotNull(buffer);

            // Find first non null target
            int y;
            for (y = 0; y < Config.MaxMultipleRenderTargets && this._renderTargets[y] == null; ++y)
            {
                ;
            }

            if (y != Config.MaxMultipleRenderTargets)
            {
                // If there is another target bound, compare sizes
                if (this._renderTargets[y].Width != buffer.Width || this._renderTargets[y].Height != buffer.Height)
                {
                    throw new AxiomException("MultiRenderTarget surfaces are not the same size.");
                }

                if (!Root.Instance.RenderSystem.Capabilities.HasCapability(Capabilities.MRTDifferentBitDepths) &&
                    (PixelUtil.GetNumElemBits(this._renderTargets[y].Format) != PixelUtil.GetNumElemBits(buffer.Format)))
                {
                    throw new AxiomException(
                        "MultiRenderTarget surfaces are not of same bit depth and hardware requires it");
                }
            }

            this._renderTargets[attachment] = buffer;
            _checkAndUpdate();
        }

        /// <summary>
        ///   Unbind Attachment
        /// </summary>
        [OgreVersion(1, 7, 2)]
        protected override void UnbindSurfaceImpl(int attachment)
        {
            Contract.Requires(attachment < Config.MaxMultipleRenderTargets);
            this._renderTargets[attachment].SafeDispose();
            this._renderTargets[attachment] = null;
            _checkAndUpdate();
        }

        /// <summary>
        ///   Check surfaces and update RenderTarget extent
        /// </summary>
        [OgreVersion(1, 7, 2)]
        private void _checkAndUpdate()
        {
            if (this._renderTargets[0] != null)
            {
                width = this._renderTargets[0].Width;
                height = this._renderTargets[0].Height;
            }
            else
            {
                width = 0;
                height = 0;
            }
        }

        #endregion Methods

        #region RenderTarget Implementation

        /// <see cref="Axiom.Graphics.RenderTarget.Update(bool)" />
        [OgreVersion(1, 7, 2790)]
        public override void Update(bool swapBuffers)
        {
            D3D9DeviceManager deviceManager = D3D9RenderSystem.DeviceManager;
            D3D9Device currRenderWindowDevice = deviceManager.ActiveRenderTargetDevice;

            if (currRenderWindowDevice != null)
            {
                if (currRenderWindowDevice.IsDeviceLost == false)
                {
                    base.Update(swapBuffers);
                }
            }
            else
            {
                foreach (D3D9Device device in deviceManager)
                {
                    if (device.IsDeviceLost == false)
                    {
                        deviceManager.ActiveRenderTargetDevice = device;
                        base.Update(swapBuffers);
                        deviceManager.ActiveRenderTargetDevice = null;
                    }
                }
            }
        }

        public override object this[string attribute]
        {
            [OgreVersion(1, 7, 2)]
            get
            {
                if (attribute.ToUpper() == "DDBACKBUFFER")
                {
                    D3D9.Surface[] surfaces = new D3D9.Surface[Config.MaxMultipleRenderTargets];
                    // Transfer surfaces
                    for (int x = 0; x < Config.MaxMultipleRenderTargets; ++x)
                    {
                        if (this._renderTargets[x] != null)
                        {
                            surfaces[x] = this._renderTargets[x].GetSurface(D3D9RenderSystem.ActiveD3D9Device);
                        }
                    }
                    return surfaces;
                }

                return null;
            }
        }

        [OgreVersion(1, 7, 2790)]
        public override bool RequiresTextureFlipping
        {
            get { return false; }
        }

        #endregion RenderTarget Implementation
    };
}