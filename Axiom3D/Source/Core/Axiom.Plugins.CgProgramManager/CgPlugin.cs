#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: CgPlugin.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.ComponentModel.Composition;
using Axiom.Core;
using Axiom.Graphics;

#endregion Namespace Declarations

namespace Axiom.CgPrograms
{
    /// <summary>
    ///   Main plugin class.
    /// </summary>
    [Export(typeof (IPlugin))]
    public class CgPlugin : IPlugin
    {
        private CgProgramFactory factory;

        /// <summary>
        ///   Called when the plugin is started.
        /// </summary>
        public void Initialize()
        {
            // register our Cg Program Factory
            this.factory = new CgProgramFactory();

            HighLevelGpuProgramManager.Instance.AddFactory(this.factory);
        }

        /// <summary>
        ///   Called when the plugin is stopped.
        /// </summary>
        public void Shutdown()
        {
            //factory.Dispose();
        }
    }
}