#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Proclaim.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Diagnostics;
using System.Collections.Generic;

#endregion Namespace Declarations

namespace Axiom.Utilities
{
    /// <summary>
    /// </summary>
    public static class Proclaim
    {
        /// <summary>
        ///   Asserts if this statement is reached.
        /// </summary>
        /// <exception cref="InvalidOperationException">Code is supposed to be unreachable.</exception>
        public static Exception Unreachable
        {
            get
            {
                Debug.Assert(false, "Unreachable");
                return new InvalidOperationException("Code is supposed to be unreachable.");
            }
        }

        /// <summary>
        ///   Asserts if any argument is <c>null</c>.
        /// </summary>
        /// <param name="vars"> </param>
        public static void NotNull(params object[] vars)
        {
            bool result = true;
            foreach (object obj in vars)
            {
                result &= (obj != null);
            }
            Debug.Assert(result);
        }

        /// <summary>
        ///   Asserts if the string is <c>null</c> or zero length.
        /// </summary>
        /// <param name="str"> </param>
        public static void NotEmpty(string str)
        {
            Debug.Assert(!String.IsNullOrEmpty(str));
        }

        /// <summary>
        ///   Asserts if the collection is <c>null</c> or the <c>Count</c> is zero.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="items"> </param>
        public static void NotEmpty<T>(ICollection<T> items)
        {
            Debug.Assert(items != null && items.Count > 0);
        }

        /// <summary>
        ///   Asserts if any item in the collection is <c>null</c>.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="items"> </param>
        public static void NotNullItems<T>(IEnumerable<T> items) where T : class
        {
            Debug.Assert(items != null);
            foreach (object item in items)
            {
                Debug.Assert(item != null);
            }
        }
    }
}