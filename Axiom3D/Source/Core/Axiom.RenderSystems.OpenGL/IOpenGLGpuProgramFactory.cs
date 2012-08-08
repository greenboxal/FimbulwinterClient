#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: IOpenGLGpuProgramFactory.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Graphics;
using Axiom.Core;
using ResourceHandle = System.UInt64;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    /// <summary>
    ///   Interface that can be implemented by a class that is intended to
    ///   handle creation of low level gpu program in OpenGL for a particular
    ///   syntax code.
    /// </summary>
    public interface IOpenGLGpuProgramFactory
    {
        /// <summary>
        ///   Creates a gpu program for the specified syntax code (i.e. arbfp1, fp30, etc).
        /// </summary>
        /// <param name="name"> </param>
        /// <param name="type"> </param>
        /// <param name="syntaxCode"> </param>
        /// <returns> </returns>
        GLGpuProgram Create(ResourceManager parent, string name, ResourceHandle handle, string group, bool isManual,
                            IManualResourceLoader loader, GpuProgramType type, string syntaxCode);
    }
}