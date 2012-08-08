#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: D3D9Plugin.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System.ComponentModel.Composition;
using Axiom.Core;
using Axiom.Graphics;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9
{
    /// <summary>
    ///   Summary description for Plugin.
    /// </summary>
    [Export(typeof (IPlugin))]
    public sealed class D3D9Plugin : IPlugin
    {
        #region Fields

        /// <summary>
        ///   Reference to the render system instance.
        /// </summary>
        private RenderSystem _renderSystem;

        #endregion Fields

        #region Implementation of IPlugin

        public void Initialize()
        {
            // Render system creation has been moved here ( like Ogre does in Install method )
            // since the Plugin.ctor is called twice during startup.
            this._renderSystem = new D3D9RenderSystem();

            // add an instance of this plugin to the list of available RenderSystems
            Root.Instance.RenderSystems.Add("DirectX9", this._renderSystem);
        }

        public void Shutdown()
        {
            this._renderSystem.SafeDispose();
            this._renderSystem = null;
        }

        #endregion Implementation of IPlugin
    };
}