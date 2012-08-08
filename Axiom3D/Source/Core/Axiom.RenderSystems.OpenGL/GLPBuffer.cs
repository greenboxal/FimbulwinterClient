#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: GLPBuffer.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;
using Axiom.Media;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    /// <summary>
    ///   An off-screen rendering context. These contexts are always RGBA for simplicity, speed and
    ///   convenience, but the component format is configurable.
    /// </summary>
    internal abstract class GLPBuffer
    {
        #region Fields and Properties

        #region Format Property

        public PixelComponentType Format { get; protected set; }

        #endregion Format Property

        #region Width Property

        public int Width { get; protected set; }

        #endregion Width Property

        #region Height Property

        public int Height { get; protected set; }

        #endregion Height Property

        #region GLContext Property

        /// <summary>
        ///   Get the GL context that needs to be active to render to this PBuffer.
        /// </summary>
        /// <returns> </returns>
        public abstract GLContext Context { get; }

        #endregion GLContext Property

        #endregion Fields and Properties

        #region Construction and Destruction

        /// <summary>
        /// </summary>
        /// <param name="format"> </param>
        /// <param name="width"> </param>
        /// <param name="height"> </param>
        public GLPBuffer(PixelComponentType format, int width, int height)
        {
        }

        #endregion Construction and Destruction

        #region Methods

        /// <summary>
        ///   Get PBuffer component format for an OGRE pixel format.
        /// </summary>
        /// <param name="fmt"> </param>
        /// <returns> </returns>
        public static PixelComponentType GetPixelComponentType(PixelFormat fmt)
        {
            return PixelUtil.GetComponentType(fmt);
        }

        #endregion Methods
    }
}