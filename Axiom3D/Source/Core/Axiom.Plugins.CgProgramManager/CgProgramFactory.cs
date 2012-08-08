#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: CgProgramFactory.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using Axiom.Graphics;
using Tao.Cg;

#endregion Namespace Declarations

namespace Axiom.CgPrograms
{
    /// <summary>
    ///   Summary description for CgProgramFactory.
    /// </summary>
    public class CgProgramFactory : HighLevelGpuProgramFactory
    {
        #region Fields

        /// <summary>
        ///   ID of the active Cg context.
        /// </summary>
        private readonly IntPtr cgContext;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///   Internal constructor.
        /// </summary>
        internal CgProgramFactory()
        {
            // create the Cg context
            this.cgContext = Cg.cgCreateContext();

            CgHelper.CheckCgError("Error creating Cg context.", this.cgContext);
        }

        #endregion Constructors

        #region HighLevelGpuProgramFactory Members

        public override string Language
        {
            get { return "cg"; }
        }

        /// <summary>
        ///   Creates and returns a specialized CgProgram instance.
        /// </summary>
        /// <param name="name"> Name of the program to create. </param>
        /// <param name="type"> Type of program to create, vertex or fragment. </param>
        /// <returns> A new CgProgram instance within the current Cg Context. </returns>
        public override HighLevelGpuProgram CreateInstance(ResourceManager parent, string name, ulong handle,
                                                           string group,
                                                           bool isManual, IManualResourceLoader loader)
        {
            return new CgProgram(parent, name, handle, group, isManual, loader, this.cgContext);
        }

        #endregion HighLevelGpuProgramFactory Members

        #region IDisposable Members

        /// <summary>
        ///   Destroys the Cg context upon being disposed.
        /// </summary>
        protected override void dispose(bool disposeManagedResources)
        {
            // destroy the Cg context
            if (disposeManagedResources)
            {
                Cg.cgDestroyContext(this.cgContext);
            }
            base.dispose(disposeManagedResources);
        }

        #endregion IDisposable Members
    }
}