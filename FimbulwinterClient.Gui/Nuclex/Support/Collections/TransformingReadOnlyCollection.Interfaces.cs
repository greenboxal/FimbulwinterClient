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

  // More than 400 lines of code just to implement the .NET collection interfaces.
  // Shows the niceties of having to support languages without generics and using
  // an inferior design to make collections "more convenient" for the user :/

  partial class TransformingReadOnlyCollection<
    ContainedItemType, ExposedItemType
  > {

    #region IList<ExposedItemType> Members

    /// <summary>
    ///   Inserts an item to the TransformingReadOnlyCollection at the specified index.
    /// </summary>
    /// <param name="index">
    ///   The zero-based index at which item should be inserted.
    /// </param>
    /// <param name="item">
    ///   The object to insert into the TransformingReadOnlyCollection
    /// </param>
    /// <exception cref="System.NotSupportedException">
    ///   The TransformingReadOnlyCollection is read-only.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   index is not a valid index in the TransformingReadOnlyCollection.
    /// </exception>
    void IList<ExposedItemType>.Insert(int index, ExposedItemType item) {
      throw new NotSupportedException("The collection is ready-only");
    }

    /// <summary>
    ///   Removes the TransformingReadOnlyCollection item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    /// <exception cref="System.NotSupportedException">
    ///   The TransformingReadOnlyCollection is read-only.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Index is not a valid index in the TransformingReadOnlyCollection.
    /// </exception>
    void IList<ExposedItemType>.RemoveAt(int index) {
      throw new NotSupportedException("The collection is ready-only");
    }

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Index is not a valid index in the TransformingReadOnlyCollection.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    ///   The property is set and the TransformingReadOnlyCollection is read-only
    /// </exception>
    ExposedItemType IList<ExposedItemType>.this[int index] {
      get {
        return this[index];
      }
      set {
        throw new NotSupportedException("The collection is ready-only");
      }
    }

    #endregion

    #region ICollection<ExposedItemType> Members

    /// <summary>Adds an item to the TransformingReadOnlyCollection.</summary>
    /// <param name="item">The object to add to the TransformingReadOnlyCollection</param>
    /// <exception cref="System.NotSupportedException">
    ///   The TransformingReadOnlyCollection is read-only.
    /// </exception>
    void ICollection<ExposedItemType>.Add(ExposedItemType item) {
      throw new NotSupportedException("The collection is ready-only");
    }

    /// <summary>Removes all items from the TransformingReadOnlyCollection</summary>
    /// <exception cref="System.NotSupportedException">
    ///    The TransformingReadOnlyCollection is read-only.
    /// </exception>
    void ICollection<ExposedItemType>.Clear() {
      throw new NotSupportedException("The collection is ready-only");
    }

    /// <summary>
    ///   Removes the first occurrence of a specific object from the
    ///   TransformingReadOnlyCollection.
    /// </summary>
    /// <param name="item">
    ///   The object to remove from the TransformingReadOnlyCollection
    /// </param>
    /// <returns>
    ///   True if item was successfully removed from the TransformingReadOnlyCollection;
    ///   otherwise, false. This method also returns false if item is not found in the
    ///   original TransformingReadOnlyCollection.
    /// </returns>
    /// <exception cref="System.NotSupportedException">
    ///   The TransformingReadOnlyCollection is read-only.
    /// </exception>
    bool ICollection<ExposedItemType>.Remove(ExposedItemType item) {
      throw new NotSupportedException("The collection is ready-only");
    }

    #endregion

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

    /// <summary>Adds an item to the TransformingReadOnlyCollection.</summary>
    /// <param name="value">
    ///   The System.Object to add to the TransformingReadOnlyCollection.
    /// </param>
    /// <returns>The position into which the new element was inserted.</returns>
    /// <exception cref="System.NotSupportedException">
    ///   The System.Collections.IList is read-only or the TransformingReadOnlyCollection
    ///   has a fixed size.
    /// </exception>
    int IList.Add(object value) {
      throw new NotSupportedException("The collection is ready-only");
    }

    /// <summary>Removes all items from the TransformingReadOnlyCollection.</summary>
    /// <exception cref="System.NotSupportedException">
    ///   The TransformingReadOnlyCollection is read-only.
    /// </exception>
    void IList.Clear() {
      throw new NotSupportedException("The collection is ready-only");
    }

    /// <summary>
    ///   Determines whether the TransformingReadOnlyCollection contains a specific value.
    /// </summary>
    /// <param name="value">
    ///   The System.Object to locate in the TransformingReadOnlyCollection.
    /// </param>
    /// <returns>
    ///   True if the System.Object is found in the TransformingReadOnlyCollection;
    ///   otherwise, false.
    /// </returns>
    bool IList.Contains(object value) {
      return Contains((ExposedItemType)value);
    }

    /// <summary>
    ///   Determines the index of a specific item in the TransformingReadOnlyCollection.
    /// </summary>
    /// <param name="value">
    ///   The System.Object to locate in the TransformingReadOnlyCollection.
    /// </param>
    /// <returns>
    ///   The index of value if found in the list; otherwise, -1.
    /// </returns>
    int IList.IndexOf(object value) {
      return IndexOf((ExposedItemType)value);
    }

    /// <summary>
    ///   Inserts an item to the TransformingReadOnlyCollection at the specified index.
    /// </summary>
    /// <param name="index">
    ///   The zero-based index at which value should be inserted.
    /// </param>
    /// <param name="value">
    ///   The System.Object to insert into the TransformingReadOnlyCollection.
    /// </param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Index is not a valid index in the TransformingReadOnlyCollection.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    ///   The System.Collections.IList is read-only or the TransformingReadOnlyCollection
    ///   has a fixed size.
    /// </exception>
    /// <exception cref="System.NullReferenceException">
    ///   Value is null reference in the TransformingReadOnlyCollection.
    /// </exception>
    void IList.Insert(int index, object value) {
      throw new NotSupportedException("The collection is ready-only");
    }

    /// <summary>
    ///   A value indicating whether the TransformingReadOnlyCollection has a fixed
    ///   size.
    /// </summary>
    bool IList.IsFixedSize {
      get { return true; }
    }

    /// <summary>
    ///   Removes the first occurrence of a specific object from the
    ///   TransformingReadOnlyCollection.
    /// </summary>
    /// <param name="value">
    ///   The System.Object to remove from the TransformingReadOnlyCollection.
    /// </param>
    /// <exception cref="System.NotSupportedException">
    ///   The TransformingReadOnlyCollection is read-only or the
    ///   TransformingReadOnlyCollection has a fixed size.
    /// </exception>
    void IList.Remove(object value) {
      throw new NotSupportedException("The collection is ready-only");
    }

    /// <summary>
    ///   Removes the TransformingReadOnlyCollection item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Index is not a valid index in the TransformingReadOnlyCollection.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    ///   The TransformingReadOnlyCollection is read-only or the
    ///   TransformingReadOnlyCollection has a fixed size.
    /// </exception>
    void IList.RemoveAt(int index) {
      throw new NotSupportedException("The collection is ready-only");
    }

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Index is not a valid index in the TransformingReadOnlyCollection
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    ///   The property is set and the TransformingReadOnlyCollection is read-only.
    /// </exception>
    object IList.this[int index] {
      get {
        return this[index];
      }
      set {
        throw new NotSupportedException("The collection is ready-only");
      }
    }

    #endregion

    #region ICollection Members

    /// <summary>
    ///   Copies the elements of the TransformingReadOnlyCollection to an System.Array,
    ///   starting at a particular System.Array index.
    /// </summary>
    /// <param name="array">
    ///   The one-dimensional System.Array that is the destination of the elements
    ///   copied from TransformingReadOnlyCollection. The System.Array must have zero-based
    ///   indexing.
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
    ///   of array or the number of elements in the source TransformingReadOnlyCollection
    ///   is greater than the available space from index to the end of the destination
    ///   array.
    /// </exception>
    /// <exception cref="System.InvalidCastException">
    ///   The type of the source TransformingReadOnlyCollection cannot be cast
    ///   automatically to the type of the destination array.
    /// </exception>
    void ICollection.CopyTo(Array array, int index) {
      CopyTo((ExposedItemType[])array, index);
    }

    /// <summary>
    ///   The number of elements contained in the TransformingReadOnlyCollection.
    /// </summary>
    int ICollection.Count {
      get { return Count; }
    }

    /// <summary>
    ///   A value indicating whether access to the TransformingReadOnlyCollection
    ///   is synchronized (thread safe).
    /// </summary>
    bool ICollection.IsSynchronized {
      get { return false; }
    }

    /// <summary>
    ///   An object that can be used to synchronize access to the
    ///   TransformingReadOnlyCollection.
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

  }

} // namespace Nuclex.Support.Collections
