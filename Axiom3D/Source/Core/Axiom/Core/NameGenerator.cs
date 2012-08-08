#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: NameGenerator~1.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;

#endregion Namespace Declarations

namespace Axiom.Core
{
    /// <summary>
    ///   Generates a unique name for a given type T
    /// </summary>
    /// <typeparam name="T"> the type to generate a name for. </typeparam>
    public class NameGenerator<T>
    {
        private static long _nextId;
        private static string _baseName;

        /// <summary>
        ///   Gets/sets the next identifier used to generate a name
        /// </summary>
        public long NextIdentifier
        {
            get { return _nextId; }
            set { _nextId = value; }
        }

        /// <summary>
        ///   Constructor
        /// </summary>
        /// <remarks>
        ///   use the name of the type as a base for generating unique names.
        /// </remarks>
        public NameGenerator()
            : this(typeof (T).Name)
        {
        }

        /// <summary>
        ///   Constructor
        /// </summary>
        /// <param name="baseName"> the base of the name for the type </param>
        public NameGenerator(string baseName)
        {
            if (string.IsNullOrEmpty(_baseName))
            {
                _baseName = baseName;
            }
        }

        /// <summary>
        ///   Generates the next name
        /// </summary>
        /// <returns> the generated name </returns>
        public string GetNextUniqueName()
        {
            return GetNextUniqueName(String.Empty);
        }

        /// <summary>
        ///   Generates the next name using a given prefix
        /// </summary>
        /// <param name="prefix"> a prefix for the name </param>
        /// <returns> the generated name </returns>
        public string GetNextUniqueName(string prefix)
        {
            return String.Format("{0}{1}{2}", prefix, _baseName, _nextId++);
        }
    }
}