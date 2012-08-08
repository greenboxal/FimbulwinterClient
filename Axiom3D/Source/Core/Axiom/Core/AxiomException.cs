#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: AxiomException.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Core
{
    /// <summary>
    ///   Summary description for AxiomException.
    /// </summary>
    public class AxiomException : Exception
    {
        public AxiomException(string message, params object[] args)
            : base(string.Format(message, args))
        {
        }

        public AxiomException(string message, Exception innerException, params object[] args)
            : base(string.Format(message, args), innerException)
        {
        }
    }
}