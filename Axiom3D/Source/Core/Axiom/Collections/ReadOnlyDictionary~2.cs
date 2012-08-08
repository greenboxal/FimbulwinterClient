#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ReadOnlyDictionary~2.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#endregion Namespace Declarations

namespace Axiom.Collections
{
    public class ReadOnlyDictionary<TKey, TValue>
        /*: ICollection, ICollection<KeyValuePair<TKey, TValue>>,
													IDictionary, IDictionary<TKey, TValue>,
													IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>> */
    {
        private readonly IDictionary<TKey, TValue> dictionary;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary;
        }

        public TValue this[TKey index]
        {
            get { return this.dictionary[index]; }
        }
    }
}