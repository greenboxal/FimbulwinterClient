#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Exceptions.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Utilities
{
    /// <summary>
    ///   Factory class for some exception classes that have variable constructors based on the 
    ///   framework that is targeted. Rather than use <c>#if</c> around the different constructors
    ///   use the least common denominator, but wrap it in an easier to use method.
    /// </summary>
    internal static class ExceptionFactory
    {
        /// <summary>
        ///   Factory for the <c>ArgumentOutOfRangeException</c>
        /// </summary>
        /// <param name="name"> </param>
        /// <param name="value"> </param>
        /// <param name="message"> </param>
        /// <returns> </returns>
        public static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(string name, object value,
                                                                                    string message)
        {
            return new ArgumentOutOfRangeException(name, string.Format("{0} (actual value is '{1}')", message, value));
        }

        /// <summary>
        ///   Factory for the <c>ArgumentOutOfRangeException</c>
        /// </summary>
        /// <returns> </returns>
        public static ArgumentNullException CreateArgumentItemNullException(int index, string arrayName)
        {
            return new ArgumentNullException(String.Format("{0}[{1}]", arrayName, index));
        }
    }
}