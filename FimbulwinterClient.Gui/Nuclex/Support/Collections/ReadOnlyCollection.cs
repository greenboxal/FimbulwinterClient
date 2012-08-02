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
using System.Collections;
using System.Collections.Generic;

namespace Nuclex.Support.Collections {

  /// <summary>Wraps a Collection and prevents users from modifying it</summary>
  /// <typeparam name="ItemType">Type of items to manage in the Collection</typeparam>
  public class ReadOnlyCollection<ItemType> :
    ICollection<ItemType>,
    ICollection {

    /// <summary>Initializes a new read-only Collection wrapper</summary>
    /// <param name="collection">Collection that will be wrapped</param>
    public ReadOnlyCollection(ICollection<ItemType> collection) {
      this.typedCollection = collection;
      this.objectCollection = (collection as ICollection);
    }

    /// <summary>Determines whether the List contains the specified item</summary>
    /// <param name="item">Item that will be checked for</param>
    /// <returns>True if the specified item is contained in the List</returns>
    public bool Contains(ItemType item) {
      return this.typedCollection.Contains(item);
    }

    /// <summary>Copies the contents of the List into an array</summary>
    /// <param name="array">Array the List will be copied into</param>
    /// <param name="arrayIndex">
    ///   Starting index at which to begin filling the destination array
    /// </param>
    public void CopyTo(ItemType[] array, int arrayIndex) {
      this.typedCollection.CopyTo(array, arrayIndex);
    }

    /// <summary>The number of items current contained in the List</summary>
    public int Count {
      get { return this.typedCollection.Count; }
    }

    /// <summary>Whether the List is write-protected</summary>
    public bool IsReadOnly {
      get { return true; }
    }

    /// <summary>Returns a new enumerator over the contents of the List</summary>
    /// <returns>The new List contents enumerator</returns>
    public IEnumerator<ItemType> GetEnumerator() {
      return this.typedCollection.GetEnumerator();
    }

    #region ICollection<> implementation

    /// <summary>Adds an item to the end of the List</summary>
    /// <param name="item">Item that will be added to the List</param>
    void ICollection<ItemType>.Add(ItemType item) {
      throw new NotSupportedException(
        "Adding items is not supported by the read-only List"
      );
    }

    /// <summary>Removes all items from the List</summary>
    void ICollection<ItemType>.Clear() {
      throw new NotSupportedException(
        "Clearing is not supported by the read-only List"
      );
    }

    /// <summary>Removes the specified item from the List</summary>
    /// <param name="item">Item that will be removed from the List</param>
    /// <returns>True of the specified item was found in the List and removed</returns>
    bool ICollection<ItemType>.Remove(ItemType item) {
      throw new NotSupportedException(
        "Removing items is not supported by the read-only List"
      );
    }

    #endregion

    #region IEnumerable implementation

    /// <summary>Returns a new enumerator over the contents of the List</summary>
    /// <returns>The new List contents enumerator</returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return this.objectCollection.GetEnumerator();
    }

    #endregion

    #region ICollection implementation

    /// <summary>Copies the contents of the List into an array</summary>
    /// <param name="array">Array the List will be copied into</param>
    /// <param name="index">
    ///   Starting index at which to begin filling the destination array
    /// </param>
    void ICollection.CopyTo(Array array, int index) {
      this.objectCollection.CopyTo(array, index);
    }

    /// <summary>Whether the List is synchronized for multi-threaded usage</summary>
    bool ICollection.IsSynchronized {
      get { return this.objectCollection.IsSynchronized; }
    }

    /// <summary>Synchronization root on which the List locks</summary>
    object ICollection.SyncRoot {
      get { return this.objectCollection.SyncRoot; }
    }

    #endregion

    /// <summary>The wrapped Collection under its type-safe interface</summary>
    private ICollection<ItemType> typedCollection;
    /// <summary>The wrapped Collection under its object interface</summary>
    private ICollection objectCollection;

  }

} // namespace Nuclex.Support.Collections
