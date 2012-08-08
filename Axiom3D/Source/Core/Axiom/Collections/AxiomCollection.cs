#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: AxiomCollection.cs 3315 2012-05-31 20:37:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Linq;
#if !USE_CUSTOM_SORTEDLIST
using System.Collections;
using System.Collections.Generic;

#endif

#endregion Namespace Declarations

namespace Axiom.Collections
{
    ///<summary>
    ///  Serves as a basis for strongly typed collections in the engine.
    ///</summary>
#if NET_40 && !(WINDOWS_PHONE || XBOX || XBOX360)
    public class AxiomCollection<T> : System.Collections.Concurrent.ConcurrentDictionary<string, T>
#else
	public class AxiomCollection<T> : Dictionary<string, T>
#endif
    {
        #region Constants

        private const int InitialCapacity = 60;

        #endregion Constants

        #region Readonly & Static Fields

        protected static int nextUniqueKeyCounter;

        protected string typeName;

        #endregion Readonly & Static Fields

        #region Fields

        protected Object parent;

        #endregion Fields

        #region Constructors

        public AxiomCollection()
        {
            this.parent = null;
            this.typeName = typeof (T).Name;
        }

        protected AxiomCollection(Object parent)
#if NET_40 && !(WINDOWS_PHONE || XBOX || XBOX360)
            : base(Environment.ProcessorCount, InitialCapacity)
#else
			: base( InitialCapacity )
#endif
        {
            this.parent = parent;
            this.typeName = typeof (T).Name;
        }

        public AxiomCollection(AxiomCollection<int> copy)
            : base((IDictionary<string, T>) copy)
        {
        }

        #endregion Constructors

        #region Instance Methods

        public virtual void Add(string key, T item)
        {
            (this as IDictionary<string, T>).Add(key, item);
        }

        public virtual void Remove(string key)
        {
            (this as IDictionary<string, T>).Remove(key);
        }

        public new virtual bool TryGetValue(string key, out T val)
        {
            val = default(T);
            if (ContainsKey(key))
            {
                val = this[key];
                return true;
            }

            return false;
        }

        public virtual bool TryRemove(string key)
        {
#if NET_40 && !(WINDOWS_PHONE || XBOX || XBOX360)
            T val;
            return base.TryRemove(key, out val);
#else
			if ( base.ContainsKey( key ) )
			{
				base.Remove( key );
				return true;
			}

			return false;
#endif
        }

        ///<summary>
        ///  Adds an unnamed object to the <see cref="AxiomCollection{T}" /> and names it manually.
        ///</summary>
        ///<param name="item"> The object to add. </param>
        public virtual void Add(T item)
        {
            Add(this.typeName + (nextUniqueKeyCounter++), item);
        }

        /// <summary>
        ///   Adds multiple items from a specified source collection
        /// </summary>
        public virtual void AddRange(IDictionary<string, T> source)
        {
            foreach (KeyValuePair<string, T> entry in source)
            {
                Add(entry.Key, entry.Value);
            }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the <see cref="AxiomCollection{T}" />.
        /// </summary>
        /// <returns> An <see cref="IEnumerator{T}" /> for the <see cref="AxiomCollection{T}" /> values. </returns>
        public new virtual IEnumerator<T> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public T this[int index]
        {
            get { return Values.Skip(index).FirstOrDefault(); }
        }

        public new T this[string key]
        {
            get { return base[key]; }
            set
            {
                if (ContainsKey(key))
                {
                    base[key] = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        #endregion Instance Methods
    }


    ///<summary>
    ///  Serves as a basis for strongly typed collections in the engine.
    ///</summary>
    public class AxiomSortedCollection<TKey, TValue> : SortedList<TKey, TValue>
    {
        #region Constants

        private const int InitialCapacity = 60;

        #endregion Constants

        #region Fields

        protected Object parent;

        #endregion Fields

        #region Constructors

        ///<summary>
        ///</summary>
        public AxiomSortedCollection()
            : base(InitialCapacity)
        {
            this.parent = null;
        }

        ///<summary>
        ///</summary>
        ///<param name="parent"> </param>
        public AxiomSortedCollection(Object parent)
            : base(InitialCapacity)
        {
            this.parent = parent;
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
        public AxiomSortedCollection(int capacity, IComparer<TKey> comparer)
            : base(capacity, comparer)
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
        public AxiomSortedCollection(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
            : base(dictionary, comparer)
        {
        }

        #endregion Constructors
    }
}