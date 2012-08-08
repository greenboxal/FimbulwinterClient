#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: GpuProgramParameters.NamedConstants.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System.Diagnostics;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    public partial class GpuProgramParameters
    {
        /// <summary>
        ///   Find a constant definition for a named parameter.
        ///   <remarks>
        ///     This method returns null if the named parameter did not exist, unlike
        ///     <see cref="GetConstantDefinition" /> which is more strict; unless you set the 
        ///     last parameter to true.
        ///   </remarks>
        /// </summary>
        /// <param name="name"> The name to look up </param>
        /// <param name="throwExceptionIfNotFound"> If set to true, failure to find an entry will throw an exception. </param>
        [OgreVersion(1, 7, 2790)]
#if NET_40
        public GpuConstantDefinition FindNamedConstantDefinition(string name, bool throwExceptionIfNotFound = false)
#else
		public GpuConstantDefinition FindNamedConstantDefinition( string name, bool throwExceptionIfNotFound )
#endif
        {
            if (this._namedConstants == null)
            {
                if (throwExceptionIfNotFound)
                {
                    throw new AxiomException("Named constants have not been initialized, perhaps a compile error.");
                }

                return null;
            }

            GpuConstantDefinition def;
            if (!this._namedConstants.Map.TryGetValue(name, out def))
            {
                if (throwExceptionIfNotFound)
                {
                    throw new AxiomException("Parameter called {0} does not exist. ", name);
                }

                return null;
            }

            return def;
        }

#if !NET_40
    /// <see cref="FindNamedConstantDefinition(string, bool)"/>
		public GpuConstantDefinition FindNamedConstantDefinition( string name )
		{
			return FindNamedConstantDefinition( name, false );
		}
#endif
    };
}