#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: D3D9DriverList.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Linq;
using Axiom.Core;
using D3D9 = SharpDX.Direct3D9;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9
{
    public class D3D9DriverList : List<D3D9Driver>, IDisposable
    {
        [OgreVersion(1, 7, 2, "D3D9DriverList::item( const String &name )")]
        public D3D9Driver this[string description]
        {
            get { return this.FirstOrDefault(x => x.DriverDescription == description); }
        }

        [OgreVersion(1, 7, 2)]
        public D3D9DriverList()
        {
            Enumerate();
        }

        ~D3D9DriverList()
        {
            Dispose();
        }

        [OgreVersion(1, 7, 2, "~D3D9DriverList")]
        public void Dispose()
        {
            foreach (D3D9Driver it in this)
            {
                it.SafeDispose();
            }

            Clear();
            GC.SuppressFinalize(this);
        }

        [OgreVersion(1, 7, 2)]
        public bool Enumerate()
        {
            D3D9.Direct3D lpD3D9 = D3D9RenderSystem.Direct3D9;

            LogManager.Instance.Write("D3D9: Driver Detection Starts");

            for (int iAdapter = 0; iAdapter < lpD3D9.AdapterCount; ++iAdapter)
            {
                D3D9.AdapterDetails adapterIdentifier = lpD3D9.GetAdapterIdentifier(iAdapter);
                D3D9.DisplayMode d3ddm = lpD3D9.GetAdapterDisplayMode(iAdapter);
                D3D9.Capabilities d3dcaps9 = lpD3D9.GetDeviceCaps(iAdapter, D3D9.DeviceType.Hardware);

                Add(new D3D9Driver(iAdapter, d3dcaps9, adapterIdentifier, d3ddm));
            }

            LogManager.Instance.Write("D3D9: Driver Detection Ends");

            return true;
        }
    };
}