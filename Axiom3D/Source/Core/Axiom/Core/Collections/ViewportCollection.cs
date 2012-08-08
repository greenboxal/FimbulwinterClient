#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ViewportCollection.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System.Collections.Generic;
using System.Diagnostics;
using Axiom.Core;
using Axiom.Collections;

#endregion

namespace Axiom.Collections
{
    /// <summary>
    ///   Represents a collection of Viewports that are sorted by zOrder key based on the associated <see
    ///    cref="IComparer&lt;T&gt;" /> implementation.
    /// </summary>
    public class ViewportCollection : AxiomSortedCollection<int, Viewport>
    {
        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2" /> class that is empty, has the default initial capacity, and uses the default <see
        ///    cref="T:System.Collections.Generic.IComparer`1" />.
        /// </summary>
        public ViewportCollection()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2" /> class that is empty, has the specified initial capacity, and uses the default <see
        ///    cref="T:System.Collections.Generic.IComparer`1" />.
        /// </summary>
        /// <param name="capacity"> The initial number of elements that the <see cref="T:System.Collections.Generic.SortedList`2" /> can contain. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="capacity" />
        ///   is less than zero.</exception>
        public ViewportCollection(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2" /> class that is empty, has the default initial capacity, and uses the specified <see
        ///    cref="T:System.Collections.Generic.IComparer`1" />.
        /// </summary>
        /// <param name="comparer"> The <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to use when comparing keys. -or- null to use the default <see
        ///    cref="T:System.Collections.Generic.Comparer`1" /> for the type of the key. </param>
        public ViewportCollection(IComparer<int> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2" /> class that is empty, has the specified initial capacity, and uses the specified <see
        ///    cref="T:System.Collections.Generic.IComparer`1" />.
        /// </summary>
        /// <param name="capacity"> The initial number of elements that the <see cref="T:System.Collections.Generic.SortedList`2" /> can contain. </param>
        /// <param name="comparer"> The <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to use when comparing keys. -or- null to use the default <see
        ///    cref="T:System.Collections.Generic.Comparer`1" /> for the type of the key. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="capacity" />
        ///   is less than zero.</exception>
        public ViewportCollection(int capacity, IComparer<int> comparer)
            : base(capacity, comparer)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2" /> class that contains elements copied from the specified <see
        ///    cref="T:System.Collections.Generic.IDictionary`2" />, has sufficient capacity to accommodate the number of elements copied, and uses the default <see
        ///    cref="T:System.Collections.Generic.IComparer`1" />.
        /// </summary>
        /// <param name="dictionary"> The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the new <see
        ///    cref="T:System.Collections.Generic.SortedList`2" /> . </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="dictionary" />
        ///   is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="dictionary" />
        ///   contains one or more duplicate keys.</exception>
        public ViewportCollection(IDictionary<int, Viewport> dictionary)
            : base(dictionary)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2" /> class that contains elements copied from the specified <see
        ///    cref="T:System.Collections.Generic.IDictionary`2" />, has sufficient capacity to accommodate the number of elements copied, and uses the specified <see
        ///    cref="T:System.Collections.Generic.IComparer`1" />.
        /// </summary>
        /// <param name="dictionary"> The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the new <see
        ///    cref="T:System.Collections.Generic.SortedList`2" /> . </param>
        /// <param name="comparer"> The <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to use when comparing keys. -or- null to use the default <see
        ///    cref="T:System.Collections.Generic.Comparer`1" /> for the type of the key. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="dictionary" />
        ///   is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="dictionary" />
        ///   contains one or more duplicate keys.</exception>
        public ViewportCollection(IDictionary<int, Viewport> dictionary, IComparer<int> comparer)
            : base(dictionary, comparer)
        {
        }

        #endregion

        #region Instance Methods

        ///<summary>
        ///  Adds a Viewport into the SortedList, automatically using its zOrder as key.
        ///</summary>
        ///<param name="item"> A Viewport </param>
        public void Add(Viewport item)
        {
            Debug.Assert(!ContainsKey(item.ZOrder),
                         "A viewport with the specified ZOrder " + item.ZOrder + " already exists.");

            // Add the viewport
            Add(item.ZOrder, item);
        }

        #endregion
    }
}