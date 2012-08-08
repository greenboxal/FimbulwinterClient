#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: D3D9TextureManager.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Collections;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Media;
using D3D9 = SharpDX.Direct3D9;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9
{
    public class D3D9TextureManager : TextureManager
    {
        [OgreVersion(1, 7, 2)]
        public D3D9TextureManager()
        {
            // register with group manager
            ResourceGroupManager.Instance.RegisterResourceManager(ResourceType, this);
        }

        [OgreVersion(1, 7, 2, "~D3D9TextureManager")]
        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    // unregister with group manager
                    ResourceGroupManager.Instance.UnregisterResourceManager(ResourceType);
                }
            }

            base.dispose(disposeManagedResources);
        }

        [OgreVersion(1, 7, 2)]
        protected override Resource _create(string name, ulong handle, string group, bool isManual,
                                            IManualResourceLoader loader, NameValuePairList createParams)
        {
            return new D3D9Texture(this, name, handle, group, isManual, loader);
        }

        // This ends up just discarding the format passed in; the C# methods don't let you supply
        // a "recommended" format.  Ah well.
        [OgreVersion(1, 7, 2)]
        public override Axiom.Media.PixelFormat GetNativeFormat(TextureType ttype, PixelFormat format,
                                                                TextureUsage usage)
        {
            // Basic filtering
            D3D9.Format d3dPF = D3D9Helper.ConvertEnum(D3D9Helper.GetClosestSupported(format));

            // Calculate usage
            D3D9.Usage d3dusage = D3D9.Usage.None;
            D3D9.Pool pool = D3D9.Pool.Managed;
            if ((usage & TextureUsage.RenderTarget) != 0)
            {
                d3dusage |= D3D9.Usage.RenderTarget;
                pool = D3D9.Pool.Default;
            }
            if ((usage & TextureUsage.Dynamic) != 0)
            {
                d3dusage |= D3D9.Usage.Dynamic;
                pool = D3D9.Pool.Default;
            }

            D3D9.Device curDevice = D3D9RenderSystem.ActiveD3D9Device;

            // Use D3DX to adjust pixel format
            switch (ttype)
            {
                case TextureType.OneD:
                case TextureType.TwoD:
                    D3D9.TextureRequirements tReqs = D3D9.Texture.CheckRequirements(curDevice, 0, 0, 0, d3dusage,
                                                                                    D3D9Helper.ConvertEnum(format), pool);
                    d3dPF = tReqs.Format;
                    break;

                case TextureType.ThreeD:
                    D3D9.VolumeTextureRequirements volReqs = D3D9.VolumeTexture.CheckRequirements(curDevice, 0, 0, 0, 0,
                                                                                                  d3dusage,
                                                                                                  D3D9Helper.ConvertEnum
                                                                                                      (format), pool);
                    d3dPF = volReqs.Format;
                    break;

                case TextureType.CubeMap:
                    D3D9.CubeTextureRequirements cubeReqs = D3D9.CubeTexture.CheckRequirements(curDevice, 0, 0, d3dusage,
                                                                                               D3D9Helper.ConvertEnum(
                                                                                                   format),
                                                                                               pool);
                    d3dPF = cubeReqs.Format;
                    break;
            }

            return D3D9Helper.ConvertEnum(d3dPF);
        }

        /// <see cref="Axiom.Core.TextureManager.IsHardwareFilteringSupported(TextureType, PixelFormat, TextureUsage, bool)" />
        [OgreVersion(1, 7, 2)]
        public override bool IsHardwareFilteringSupported(TextureType ttype, PixelFormat format, TextureUsage usage,
                                                          bool preciseFormatOnly)
        {
            if (!preciseFormatOnly)
            {
                format = GetNativeFormat(ttype, format, usage);
            }

            D3D9RenderSystem rs = (D3D9RenderSystem) Root.Instance.RenderSystem;
            return rs.CheckTextureFilteringSupported(ttype, format, usage);
        }
    };
}