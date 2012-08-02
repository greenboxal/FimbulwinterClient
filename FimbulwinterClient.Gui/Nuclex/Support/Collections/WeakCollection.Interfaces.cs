#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace Nuclex.Support.Collections {

  partial class WeakCollection<ItemType> {

    #region IEnumerable Members

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>
    ///   A System.Collections.IEnumerator object that can be used to iterate through
    ///   the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    #endregion

    #region IList Members

    /// <summary>Adds an item to the WeakCollection.</summary>
    /// <param name="value">The System.Object to add to the WeakCollection.</param>
    /// <returns>The position into which the new element was inserted.</returns>
    /// <exception cref="System.NotSupportedException">
    ///   The System.Collections.IList is read-only or the WeakCollection has a fixed size.
    /// </exception>
    int IList.Add(object value) {
      ItemType valueAsItemType = downcastToItemType(value);
      return (this.items as IList).Add(new WeakReference<ItemType>(valueAsItemType));
    }

    /// <summary>
    ///   Determines whether the WeakCollection contains a specific value.
    /// </summary>
    /// <param name="value">The System.Object to locate in the WeakCollection.</param>
    /// <returns>
    ///   True if the System.Object is found in the WeakCollection; otherwise, false.
    /// </returns>
    bool IList.Contains(object value) {
      ItemType valueAsItemType = downcastToItemType(value);
      return Contains(valueAsItemType);
    }

    /// <summary>Determines the index of a specific item in the WeakCollection.</summary>
    /// <param name="value">The System.Object to locate in the WeakCollection.</param>
    /// <returns>
    ///   The index of value if found in the list; otherwise, -1.
    /// </returns>
    int IList.IndexOf(object value) {
      ItemType valueAsItemType = downcastToItemType(value);
      return IndexOf(valueAsItemType);
    }

    /// <summary>
    ///   Inserts an item to the WeakCollection at the specified index.
    /// </summary>
    /// <param name="index">
    ///   The zero-based index at which value should be inserted.
    /// </param>
    /// <param name="value">The System.Object to insert into the WeakCollection.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Index is not a valid index in the TransformingReadOnlyCollection.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    ///   The System.Collections.IList is read-only or the WeakCollection has a fixed size.
    /// </exception>
    /// <exception cref="System.NullReferenceException">
    ///   Value is null reference in the WeakCollection.
    /// </exception>
    void IList.Insert(int index, object value) {
      ItemType valueAsItemType = downcastToItemType(value);
      Insert(index, valueAsItemType);
    }

    /// <summary>
    ///   A value indicating whether the WeakCollection has a fixed size.
    /// </summary>
    bool IList.IsFixedSize {
      get { return (this.items as IList).IsFixedSize; }
    }

    /// <summary>
    ///   Removes the first occurrence of a specific object from the WeakCollection.
    /// </summary>
    /// <param name="value">The System.Object to remove from the WeakCollection.</param>
    /// <exception cref="System.NotSupportedException">
    ///   The WeakCollection is read-only or the WeakCollection has a fixed size.
    /// </exception>
    void IList.Remove(object value) {
      ItemType valueAsItemType = downcastToItemType(value);
      Remove(valueAsItemType);
    }

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Index is not a valid index in the WeakCollection
    /// </exception>
    object IList.this[int index] {
      get { return this[index]; }
      set {
        ItemType valueAsItemType = downcastToItemType(value);
        this[index] = valueAsItemType;
      }
    }

    #endregion

    #region ICollection Members

    /// <summary>
    ///   Copies the elements of the WeakCollection to an System.Array, starting at
    ///   a particular System.Array index.
    /// </summary>
    /// <param name="array">
    ///   The one-dimensional System.Array that is the destination of the elements
    ///   copied from WeakCollection. The System.Array must have zero-based indexing.
    /// </param>
    /// <param name="index">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">
    ///   Array is null.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Index is less than zero.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///   Array is multidimensional or index is equal to or greater than the length
    ///   of array or the number of elements in the source WeakCollection is greater than
    ///   the available space from index to the end of the destination array.
    /// </exception>
    /// <exception cref="System.InvalidCastException">
    ///   The type of the source WeakCollection cannot be cast automatically to the type of
    ///   the destination array.
    /// </exception>
    void ICollection.CopyTo(Array array, int index) {
      CopyTo((ItemType[])array, index);
    }

    /// <summary>
    ///   A value indicating whether access to the WeakCollection is
    ///   synchronized (thread safe).
    /// </summary>
    bool ICollection.IsSynchronized {
      get { return false; }
    }

    /// <summary>
    ///   An object that can be used to synchronize access to the WeakCollection.
    /// </summary>
    object ICollection.SyncRoot {
      get {
        if(this.syncRoot == null) {
          ICollection is2 = this.items as ICollection;
          if(is2 != null) {
            this.syncRoot = is2.SyncRoot;
          } else {
            Interlocked.CompareExchange(ref this.syncRoot, new object(), null);
          }
        }

        return this.syncRoot;
      }
    }

    #endregion

    /// <summary>
    ///   Downcasts an object reference to a reference to the collection's item type
    /// </summary>
    /// <param name="value">Object reference that will be downcast</param>
    /// <returns>
    ///   The specified object referecne as a reference to the collection's item type
    /// </returns>
    private static ItemType downcastToItemType(object value) {
      ItemType valueAsItemType = value as ItemType;
      if(!ReferenceEquals(value, null)) {
        if(valueAsItemType == null) {
          throw new ArgumentException("Object is not of a compatible type", "value");
        }
      }
      return valueAsItemType;
    }

  }

} // namespace Nuclex.Support.Collections
