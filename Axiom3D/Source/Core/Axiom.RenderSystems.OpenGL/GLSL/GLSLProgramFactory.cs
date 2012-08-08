#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: GLSLProgramFactory.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using Axiom.Graphics;
using ResourceHandle = System.UInt64;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL.GLSL
{
    ///<summary>
    ///  Factory class for GLSL programs.
    ///</summary>
    public sealed class GLSLProgramFactory : HighLevelGpuProgramFactory
    {
        #region Fields

        /// <summary>
        ///   Language string.
        /// </summary>
        private static string languageName = "glsl";

        /// <summary>
        ///   Reference to the link program manager we create.
        /// </summary>
        private readonly GLSLLinkProgramManager glslLinkProgramMgr;

        #endregion Fields

        #region Constructor

        /// <summary>
        ///   Default constructor.
        /// </summary>
        internal GLSLProgramFactory()
        {
            // instantiate the singleton
            this.glslLinkProgramMgr = new GLSLLinkProgramManager();
        }

        #endregion Constructor

        #region HighLevelGpuProgramFactory Implementation

        ///<summary>
        ///  Creates and returns a new GLSL program object.
        ///</summary>
        ///<param name="name"> Name of the object. </param>
        ///<param name="type"> Type of the object. </param>
        ///<returns> A newly created GLSL program object. </returns>
        public override HighLevelGpuProgram CreateInstance(ResourceManager parent, string name, ResourceHandle handle,
                                                           string group, bool isManual, IManualResourceLoader loader)
        {
            return new GLSLProgram(parent, name, handle, group, isManual, loader);
        }

        ///<summary>
        ///  Returns the language code for this high level program manager.
        ///</summary>
        public override string Language
        {
            get { return languageName; }
        }

        #endregion HighLevelGpuProgramFactory Implementation

        #region IDisposable Implementation

        /// <summary>
        ///   Called when the engine is shutting down.
        /// </summary>
        protected override void dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                this.glslLinkProgramMgr.Dispose();
            }
            base.dispose(disposeManagedResources);
        }

        #endregion IDisposable Implementation
    }
}