#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: D3D9ResourceManager.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Collections;
using Axiom.Core;
using Axiom.Utilities;
using D3D9 = SharpDX.Direct3D9;
using ResourceContainer = System.Collections.Generic.List<Axiom.RenderSystems.DirectX9.ID3D9Resource>;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9
{
    public class D3D9ResourceManager : ResourceManager
    {
        public enum ResourceCreationPolicy
        {
            CreateOnActiveDevice,
            CreateOnAllDevices
        };

        [OgreVersion(1, 7, 2790)] private static readonly object _resourcesMutex = new object();

        [OgreVersion(1, 7, 2790)] protected new ResourceContainer Resources = new ResourceContainer();

        [OgreVersion(1, 7, 2790)] private int _deviceAccessLockCount;

        [OgreVersion(1, 7, 2790)]
        public ResourceCreationPolicy CreationPolicy { get; set; }

        [OgreVersion(1, 7, 2790)]
        public bool AutoHardwareBufferManagement { get; set; }

        #region Constructor

        [OgreVersion(1, 7, 2790)]
        public D3D9ResourceManager()
        {
            CreationPolicy = ResourceCreationPolicy.CreateOnAllDevices;
        }

        #endregion Constructor

        protected override Resource _create(string name, ulong handle, string group, bool isManual,
                                            IManualResourceLoader loader, NameValuePairList createParams)
        {
            throw new NotImplementedException("Base class needs update to 1.7.2790");
        }

        #region LockDeviceAccess

        [OgreVersion(1, 7, 2790)]
        public void LockDeviceAccess()
        {
            Contract.Requires(this._deviceAccessLockCount >= 0);
            this._deviceAccessLockCount++;
            if (this._deviceAccessLockCount == 1)
            {
#if AXIOM_THREAD_SUPPORT
				System.Threading.Monitor.Enter( _resourcesMutex );
#endif
                foreach (ID3D9Resource it in this.Resources)
                {
                    it.LockDeviceAccess();
                }

                D3D9HardwarePixelBuffer.LockDeviceAccess();
            }
        }

        #endregion LockDeviceAccess

        #region UnlockDeviceAccess

        [OgreVersion(1, 7, 2790)]
        public void UnlockDeviceAccess()
        {
            Contract.Requires(this._deviceAccessLockCount > 0);
            this._deviceAccessLockCount--;
            if (this._deviceAccessLockCount == 0)
            {
                // outermost recursive lock release, propagte unlock
                foreach (ID3D9Resource it in this.Resources)
                {
                    it.UnlockDeviceAccess();
                }

                D3D9HardwarePixelBuffer.UnlockDeviceAccess();
#if AXIOM_THREAD_SUPPORT
				System.Threading.Monitor.Exit( _resourcesMutex );
#endif
            }
        }

        #endregion UnlockDeviceAccess

        #region NotifyOnDeviceCreate

        [OgreVersion(1, 7, 2790)]
        public void NotifyOnDeviceCreate(D3D9.Device d3D9Device)
        {
            lock (_resourcesMutex)
            {
                foreach (ID3D9Resource it in this.Resources)
                {
                    it.NotifyOnDeviceCreate(d3D9Device);
                }
            }
        }

        #endregion NotifyOnDeviceCreate

        #region NotifyOnDeviceDestroy

        [OgreVersion(1, 7, 2790)]
        public void NotifyOnDeviceDestroy(D3D9.Device d3D9Device)
        {
            lock (_resourcesMutex)
            {
                foreach (ID3D9Resource it in this.Resources)
                {
                    it.NotifyOnDeviceDestroy(d3D9Device);
                }
            }
        }

        #endregion NotifyOnDeviceDestroy

        #region NotifyOnDeviceLost

        [OgreVersion(1, 7, 2790)]
        public void NotifyOnDeviceLost(D3D9.Device d3D9Device)
        {
            lock (_resourcesMutex)
            {
                foreach (ID3D9Resource it in this.Resources)
                {
                    it.NotifyOnDeviceLost(d3D9Device);
                }
            }
        }

        #endregion NotifyOnDeviceLost

        #region NotifyOnDeviceReset

        [OgreVersion(1, 7, 2790)]
        public void NotifyOnDeviceReset(D3D9.Device d3D9Device)
        {
            lock (_resourcesMutex)
            {
                foreach (ID3D9Resource it in this.Resources)
                {
                    it.NotifyOnDeviceReset(d3D9Device);
                }
            }
        }

        #endregion NotifyOnDeviceReset

        #region NotifyResourceCreated

        [OgreVersion(1, 7, 2790)]
        public void NotifyResourceCreated(ID3D9Resource pResource)
        {
            lock (_resourcesMutex)
            {
                this.Resources.Add(pResource);
            }
        }

        #endregion NotifyResourceCreated

        #region NotifyResourceDestroyed

        [OgreVersion(1, 7, 2790)]
        public void NotifyResourceDestroyed(ID3D9Resource pResource)
        {
            lock (_resourcesMutex)
            {
                if (this.Resources.Contains(pResource))
                {
                    this.Resources.Remove(pResource);
                }
            }
        }

        #endregion NotifyResourceDestroyed
    };
}