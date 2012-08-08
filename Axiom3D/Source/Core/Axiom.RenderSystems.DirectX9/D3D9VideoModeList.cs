#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: D3D9VideoModeList.cs 3362 2012-07-29 00:40:37Z romeoxbm $"/>
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
    /// <summary>
    ///   Summary description for VideoModeCollection.
    /// </summary>
    public class D3D9VideoModeList : List<D3D9VideoMode>, IDisposable
    {
        private D3D9Driver _mpDriver;

        [OgreVersion(1, 7, 2, "D3D9VideoModeList::item( const String &name )")]
        public D3D9VideoMode this[string description]
        {
            get { return this.FirstOrDefault(x => x.Description == description); }
        }

        [OgreVersion(1, 7, 2)]
        public D3D9VideoModeList(D3D9Driver pDriver)
        {
            if (pDriver == null)
            {
                throw new AxiomException("pDriver parameter is NULL");
            }

            this._mpDriver = pDriver;
            Enumerate();
        }

        ~D3D9VideoModeList()
        {
            Dispose();
        }

        [OgreVersion(1, 7, 2)]
        public void Dispose()
        {
            this._mpDriver = null;

            foreach (D3D9VideoMode currentVideoMode in this)
                currentVideoMode.SafeDispose();

            Clear();

            GC.SuppressFinalize(this);
        }

        [OgreVersion(1, 7, 2)]
        public bool Enumerate()
        {
            _enumerateByFormat(D3D9.Format.R5G6B5);
            _enumerateByFormat(D3D9.Format.X8R8G8B8);

            return true;
        }

        [AxiomHelper(0, 9)]
        private void _enumerateByFormat(D3D9.Format format)
        {
            D3D9.Direct3D pD3D = D3D9RenderSystem.Direct3D9;
            int adapter = this._mpDriver.AdapterNumber;

            for (int iMode = 0; iMode < pD3D.GetAdapterModeCount(adapter, format); iMode++)
            {
                D3D9.DisplayMode displayMode = pD3D.EnumAdapterModes(adapter, format, iMode);

                // Filter out low-resolutions
                if (displayMode.Width < 640 || displayMode.Height < 400)
                {
                    continue;
                }

                // Check to see if it is already in the list (to filter out refresh rates)
                bool found = false;
                for (int it = 0; it < Count; it++)
                {
                    D3D9.DisplayMode oldDisp = this[it].DisplayMode;
                    if (oldDisp.Width == displayMode.Width && oldDisp.Height == displayMode.Height &&
                        oldDisp.Format == displayMode.Format)
                    {
                        // Check refresh rate and favour higher if poss
                        if (oldDisp.RefreshRate < displayMode.RefreshRate)
                        {
                            this[it].RefreshRate = displayMode.RefreshRate;
                        }

                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Add(new D3D9VideoMode(displayMode));
                }
            }
        }
    };
}