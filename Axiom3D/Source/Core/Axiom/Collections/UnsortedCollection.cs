#region SVN Version Information

// <file>
//     <license see="http://axiomengine.sf.net/wiki/index.php/license.txt"/>
//     <id value="$Id: UnsortedCollection.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace Axiom.Collections
{
    /// <summary>
    ///   Represents a strongly typed list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    public class UnsortedCollection<T> : List<T>
    {
        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1" /> class that is empty and has the default initial capacity.
        /// </summary>
        public UnsortedCollection()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1" /> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity"> The number of elements that the new list can initially store. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="capacity" />
        ///   is less than 0.</exception>
        public UnsortedCollection(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1" /> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection"> The collection whose elements are copied to the new list. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="collection" />
        ///   is null.</exception>
        public UnsortedCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        #endregion
    }
}