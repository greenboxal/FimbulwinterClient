#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id:"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using Axiom.Graphics;
using System.Runtime.InteropServices;
using OpenTK;
using System.ComponentModel;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    internal class WindowMessageHandling
    {
        #region Fields and Properties

        #endregion Fields and Properties

        #region Construction and Destruction

        #endregion Construction and Destruction

        #region Methods

        private static bool firstTime = true;

        public static void MessagePump()
        {
            foreach (RenderWindow renderWindow in WindowEventMonitor.Instance.Windows)
            {
                object window = renderWindow["nativewindow"];
                if (null != window && window is INativeWindow)
                {
                    ((INativeWindow) window).ProcessEvents();
                    if (firstTime)
                    {
                        ((INativeWindow) window).Closing +=
                            (sender, args) => WindowEventMonitor.Instance.WindowClosed(renderWindow);
                    }
                }
            }
            firstTime = false;
        }

        #endregion Methods
    }
}