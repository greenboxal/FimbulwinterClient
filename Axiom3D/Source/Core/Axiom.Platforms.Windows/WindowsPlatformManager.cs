#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: WindowsPlatformManager.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Axiom.Core;
using Axiom.Input;

#endregion Namespace Declarations

namespace Axiom.Platforms.Windows
{
    ///<summary>
    ///  Platform management specialization for Microsoft Windows (r) platform.
    ///</summary>
    [Export(typeof (IPlatformManager))]
    public class WindowsPlatformManager : IPlatformManager
    {
        public WindowsPlatformManager()
        {
            LogManager.Instance.Write("Windows Platform Manager Loaded.");
        }

        /// <summary>
        ///   Called when the engine is being shutdown.
        /// </summary>
        public void Dispose()
        {
            LogManager.Instance.Write("Win32 Platform Manager Shutdown.");
        }
    }
}