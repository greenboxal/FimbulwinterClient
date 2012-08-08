#region SVN Version Information

// <file>
//     <license see="http://axiomengine.sf.net/wiki/index.php/license.txt"/>
//     <id value="$Id: BaseCollection.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections;

#endregion Namespace Declarations

namespace Axiom.Math.Collections
{
    ///<summary>
    ///  Serves as a basis for strongly typed collections in the math lib.
    ///</summary>
    ///<remarks>
    ///  Can't wait for Generics in .Net Framework 2.0!
    ///</remarks>
    public abstract class BaseCollection : ICollection, IEnumerable, IEnumerator
    {
        /// <summary>
        /// </summary>
        protected ArrayList objectList;

        //		protected int nextUniqueKeyCounter;

        private const int INITIAL_CAPACITY = 50;

        #region Constructors

        ///<summary>
        ///</summary>
        public BaseCollection()
        {
            this.objectList = new ArrayList(INITIAL_CAPACITY);
        }

        #endregion

        ///<summary>
        ///</summary>
        public object this[int index]
        {
            get { return this.objectList[index]; }
            set { this.objectList[index] = value; }
        }

        ///<summary>
        ///  Adds an item to the collection.
        ///</summary>
        ///<param name="item"> </param>
        protected void Add(object item)
        {
            this.objectList.Add(item);
        }

        ///<summary>
        ///  Clears all objects from the collection.
        ///</summary>
        public void Clear()
        {
            this.objectList.Clear();
        }

        ///<summary>
        ///  Removes the item from the collection.
        ///</summary>
        ///<param name="item"> </param>
        public void Remove(object item)
        {
            int index = this.objectList.IndexOf(item);

            if (index != -1)
            {
                this.objectList.RemoveAt(index);
            }
        }

        #region Implementation of ICollection

        /// <summary>
        /// </summary>
        /// <param name="array"> </param>
        /// <param name="index"> </param>
        public void CopyTo(System.Array array, int index)
        {
            this.objectList.CopyTo(array, index);
        }

        /// <summary>
        /// </summary>
        public bool IsSynchronized
        {
            get { return this.objectList.IsSynchronized; }
        }

        /// <summary>
        /// </summary>
        public int Count
        {
            get { return this.objectList.Count; }
        }

        /// <summary>
        /// </summary>
        public object SyncRoot
        {
            get { return this.objectList.SyncRoot; }
        }

        #endregion

        #region Implementation of IEnumerable

        public System.Collections.IEnumerator GetEnumerator()
        {
            return this;
        }

        #endregion

        #region Implementation of IEnumerator

        private int position = -1;

        ///<summary>
        ///  Resets the in progress enumerator.
        ///</summary>
        public void Reset()
        {
            // reset the enumerator position
            this.position = -1;
        }

        ///<summary>
        ///  Moves to the next item in the enumeration if there is one.
        ///</summary>
        ///<returns> </returns>
        public bool MoveNext()
        {
            this.position += 1;

            if (this.position >= this.objectList.Count)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        ///<summary>
        ///  Returns the current object in the enumeration.
        ///</summary>
        public object Current
        {
            get { return this.objectList[this.position]; }
        }

        #endregion
    }
}