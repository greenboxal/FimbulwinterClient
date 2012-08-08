#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: GLRenderTexture.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Diagnostics;
using Axiom.Core;
using Axiom.Graphics;
using Tao.OpenGl;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    /// <summary>
    ///   Base class for GL Render Textures.
    /// </summary>
    internal class GLRenderTexture : RenderTexture
    {
        #region Fields and Properties

        protected bool HwGamma;
        protected int Fsaa;

        #endregion Fields and Properties

        #region Construction and Destruction

        public GLRenderTexture(string name, GLSurfaceDesc target, bool writeGamma, int fsaa)
            : base(target.Buffer, target.ZOffset)
        {
            this.name = name;
            this.HwGamma = writeGamma;
            this.Fsaa = fsaa;
        }

        #endregion Construction and Destruction

        #region RenderTexture Implementation

        #region Properties

        public override bool RequiresTextureFlipping
        {
            get { return true; }
        }

        #endregion Properties

        #region Methods

        #endregion Methods

        #endregion RenderTexture Implementation
    }
}