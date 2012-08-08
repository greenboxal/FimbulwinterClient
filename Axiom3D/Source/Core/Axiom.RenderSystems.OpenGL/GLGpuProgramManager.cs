#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: GLGpuProgramManager.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Collections;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    /// <summary>
    ///   Summary description for GLGpuProgramManager.
    /// </summary>
    public class GLGpuProgramManager : GpuProgramManager
    {
        protected Hashtable factories = new Hashtable();

        /// <summary>
        ///   Create the specified type of GpuProgram.
        /// </summary>
        /// <param name="name"> </param>
        /// <param name="type"> </param>
        /// <returns> </returns>
        protected override Resource _create(string name, ulong handle, string group, bool isManual,
                                            IManualResourceLoader loader, NameValuePairList createParams)
        {
            if (createParams == null ||
                (createParams.ContainsKey("syntax") == false || createParams.ContainsKey("type") == false))
            {
                throw new Exception("You must supply 'syntax' and 'type' parameters");
            }

            string syntaxCode = createParams["syntax"];
            string type = createParams["type"];

            // if there is none, this syntax code must not be supported
            // just return the base GL program since it won't be doing anything anyway
            if (this.factories[syntaxCode] == null)
            {
                return new GLGpuProgram(this, name, handle, group, isManual, loader);
            }

            GpuProgramType gpt;
            if (type == "vertex_program")
            {
                gpt = GpuProgramType.Vertex;
            }
            else
            {
                gpt = GpuProgramType.Fragment;
            }

            return ((IOpenGLGpuProgramFactory) this.factories[syntaxCode]).Create(this, name, handle, group, isManual,
                                                                                  loader,
                                                                                  gpt,
                                                                                  syntaxCode);
        }

        protected override Resource _create(string name, ulong handle, string group, bool isManual,
                                            IManualResourceLoader loader, GpuProgramType type, string syntaxCode)
        {
            // if there is none, this syntax code must not be supported
            // just return the base GL program since it won't be doing anything anyway
            if (this.factories[syntaxCode] == null)
            {
                return new GLGpuProgram(this, name, handle, group, isManual, loader);
            }

            // get a reference to the factory for this syntax code
            IOpenGLGpuProgramFactory factory = (IOpenGLGpuProgramFactory) this.factories[syntaxCode];

            // create the gpu program
            return factory.Create(this, name, handle, group, isManual, loader, type, syntaxCode);
        }

        /// <summary>
        ///   Returns a specialized version of GpuProgramParameters.
        /// </summary>
        /// <returns> </returns>
        public override GpuProgramParameters CreateParameters()
        {
            return new GpuProgramParameters();
        }

        /// <summary>
        ///   Registers a factory to handles requests for the creation of low level
        ///   gpu porgrams based on the syntax code.
        /// </summary>
        /// <param name="factory"> </param>
        public void RegisterProgramFactory(string syntaxCode, IOpenGLGpuProgramFactory factory)
        {
            // store this factory for the specified syntax code
            this.factories[syntaxCode] = factory;
        }
    }
}