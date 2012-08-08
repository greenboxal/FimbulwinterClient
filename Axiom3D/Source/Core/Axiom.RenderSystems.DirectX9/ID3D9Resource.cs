#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ID3D9Resource.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using D3D9 = SharpDX.Direct3D9;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9
{
    /// <summary>
    ///   Represents a Direct3D rendering resource.
    ///   Provide unified interface to
    ///   handle various device states.
    /// </summary>
    /// <note>romeoxbm: LockDeviceAccess and UnlockDeviceAccess have been removed because those interface members
    ///   cannot be implemented as static, and they has been moved to ID3D9ResourceExtensions class as
    ///   extension methods.</note>
    [OgreVersion(1, 7, 2790)]
    public interface ID3D9Resource
    {
        /// <summary>
        ///   Called immediately after the Direct3D device has been created.
        /// </summary>
        [OgreVersion(1, 7, 2790)]
        void NotifyOnDeviceCreate(D3D9.Device d3d9Device);

        /// <summary>
        ///   Called before the Direct3D device is going to be destroyed.
        /// </summary>
        [OgreVersion(1, 7, 2790)]
        void NotifyOnDeviceDestroy(D3D9.Device d3d9Device);

        /// <summary>
        ///   Called immediately after the Direct3D device has entered a lost state.
        ///   This is the place to release non-managed resources.
        /// </summary>
        [OgreVersion(1, 7, 2790)]
        void NotifyOnDeviceLost(D3D9.Device d3d9Device);

        /// <summary>
        ///   Called immediately after the Direct3D device has been reset.
        ///   This is the place to create non-managed resources.
        /// </summary>
        [OgreVersion(1, 7, 2790)]
        void NotifyOnDeviceReset(D3D9.Device d3d9Device);
    };

    public static class ID3D9ResourceExtensions
    {
#if AXIOM_THREAD_SUPPORT
		private static readonly object deviceLockMutex = new object();
#endif

        [OgreVersion(1, 7, 2)]
        public static void LockDeviceAccess(this ID3D9Resource res)
        {
#if AXIOM_THREAD_SUPPORT
			if ( Configuration.Config.AxiomThreadLevel == 1 )
				System.Threading.Monitor.Enter( deviceLockMutex );
#endif
        }

        [OgreVersion(1, 7, 2)]
        public static void UnlockDeviceAccess(this ID3D9Resource res)
        {
#if AXIOM_THREAD_SUPPORT
			if ( Configuration.Config.AxiomThreadLevel == 1 )
				System.Threading.Monitor.Exit( deviceLockMutex );
#endif
        }
    };
}