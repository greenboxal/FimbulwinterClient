#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: D3D9RenderTexture.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Configuration;
using Axiom.Core;
using Axiom.Graphics;
using SharpDX;
using D3D9 = SharpDX.Direct3D9;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9
{
    /// <summary>
    ///   RenderTexture implementation for D3D9
    /// </summary>
    public class D3D9RenderTexture : RenderTexture
    {
        [OgreVersion(1, 7, 2)]
        public override bool RequiresTextureFlipping
        {
            get { return false; }
        }

        [OgreVersion(1, 7, 2)]
        public D3D9RenderTexture(string name, D3D9HardwarePixelBuffer buffer, bool writeGamma, int fsaa)
            : base(buffer, 0)
        {
            this.name = name;
            hwGamma = writeGamma;
            this.fsaa = fsaa;
        }

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

        [OgreVersion(1, 7, 2)]
        public override object this[string attribute]
        {
            get
            {
                switch (attribute.ToUpper())
                {
                    case "DDBACKBUFFER":
                        D3D9.Surface[] surface = new D3D9.Surface[Config.MaxMultipleRenderTargets];
                        if (fsaa > 0)
                        {
                            surface[0] =
                                ((D3D9HardwarePixelBuffer) pixelBuffer).GetFSAASurface(D3D9RenderSystem.ActiveD3D9Device);
                        }
                        else
                        {
                            surface[0] =
                                ((D3D9HardwarePixelBuffer) pixelBuffer).GetSurface(D3D9RenderSystem.ActiveD3D9Device);
                        }

                        return surface;

                    case "HWND":
                        return null;

                    case "BUFFER":
                        return pixelBuffer;

                    default:
                        return null;
                }
            }
        }

        /// <summary>
        ///   Override needed to deal with FSAA
        /// </summary>
        [OgreVersion(1, 7, 2)]
        public override void SwapBuffers(bool waitForVSync)
        {
            // Only needed if we have to blit from AA surface
            if (fsaa > 0)
            {
                D3D9DeviceManager deviceManager = D3D9RenderSystem.DeviceManager;
                D3D9HardwarePixelBuffer buf = (D3D9HardwarePixelBuffer) (pixelBuffer);

                foreach (D3D9Device device in deviceManager)
                {
                    if (device.IsDeviceLost == false)
                    {
                        D3D9.Device d3d9Device = device.D3DDevice;
                        Result res = d3d9Device.StretchRectangle(buf.GetFSAASurface(d3d9Device),
                                                                 buf.GetSurface(d3d9Device),
                                                                 D3D9.TextureFilter.None);

                        if (res.Failure)
                        {
                            throw new AxiomException("Unable to copy AA buffer to final buffer: {0}", res.ToString());
                        }
                    }
                }
            }
        }
    };
}