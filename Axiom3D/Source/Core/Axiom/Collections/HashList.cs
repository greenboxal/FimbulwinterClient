#region SVN Version Information

// <file>
//     <license see="http://axiomengine.sf.net/wiki/index.php/license.txt"/>
//     <id value="$Id: HashList.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

#endregion Namespace Declarations

namespace Axiom.Collections
{
    /// <summary>
    ///   Represents a collection of key/value pairs that are sorted by the keys and are accessible by key and by index.
    /// </summary>
    public class HashList<TKey, TValue> : AxiomSortedCollection<TKey, TValue>
    {
        #region Instance Indexers

        /// <summary>
        ///   Gets the <see name="TValue" /> at the specified index.
        /// </summary>
        /// <value> A <see name="TValue" /> . </value>
        public TValue this[int index]
        {
            get { return Values[index]; }
        }

        #endregion

        #region Instance Methods

        /// <summary>
        ///   Gets a <see name="TValue" /> by key.
        /// </summary>
        /// <param name="key"> The key. </param>
        /// <returns> The <see name="TValue" /> that corresponds to the specified key. </returns>
        public TValue GetByKey(TKey key)
        {
            return base[key];
        }

        /// <summary>
        ///   Gets the key at the specified index.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> The key at the specified index. </returns>
        public TKey GetKeyAt(int index)
        {
            return Keys[index];
        }

        #endregion
    }
}