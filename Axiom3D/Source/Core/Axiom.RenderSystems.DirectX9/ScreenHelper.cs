#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ScreenHelper.cs 3313 2012-05-31 16:30:24Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9.Helpers
{
    public static class ScreenHelper
    {
        /// <summary>
        ///   Workaround method to get the right Screen associated
        ///   with the input screen/monitor handle.
        /// </summary>
        [AxiomHelper(0, 9)]
        public static Screen FromHandle(IntPtr handle)
        {
            Screen s = Screen.AllScreens.FirstOrDefault(x => x.GetHashCode() == (int) handle) ??
                       Screen.FromHandle(handle);

            return s;
        }

        /// <summary>
        ///   Returns the handle of a Screen from a point
        /// </summary>
        [AxiomHelper(0, 9)]
        public static IntPtr GetHandle(Point p)
        {
            Screen s = Screen.FromPoint(p);
            return new IntPtr(s.GetHashCode());
        }

        /// <summary>
        ///   Returns the handle of a Screen from a window handle
        /// </summary>
        [AxiomHelper(0, 9)]
        public static IntPtr GetHandle(IntPtr windowHandle)
        {
            Screen s = ScreenHelper.FromHandle(windowHandle);
            return new IntPtr(s.GetHashCode());
        }
    };
}