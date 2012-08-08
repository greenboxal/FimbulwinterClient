#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Plugin.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.ComponentModel.Composition;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Utilities;
using System.Reflection;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    /// <summary>
    ///   Summary description for Plugin.
    /// </summary>
    [Export(typeof (IPlugin))]
    public sealed class Plugin : IPlugin
    {
        #region Implementation of IPlugin

        /// <summary>
        ///   Reference to the render system instance.
        /// </summary>
        private GLRenderSystem _renderSystem;

        public void Initialize()
        {
#if OPENGL_OTK
			Contract.Requires( PlatformManager.Instance.GetType().Name == "OpenTKPlatformManager", "PlatformManager", "OpenGL OpenTK Renderer requires OpenTK Platform Manager." );
#endif
            Contract.Requires(Root.Instance.RenderSystems.ContainsKey("OpenGL") == false, "OpenGL",
                              "An instance of the OpenGL renderer is already loaded.");

            this._renderSystem = new GLRenderSystem();
            // add an instance of this plugin to the list of available RenderSystems
            Root.Instance.RenderSystems.Add("OpenGL", this._renderSystem);
        }

        public void Shutdown()
        {
            this._renderSystem.Shutdown();
        }

        #endregion Implementation of IPlugin
    }
}