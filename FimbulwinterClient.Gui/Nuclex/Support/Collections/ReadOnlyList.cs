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

  /// <summary>Wraps a List and prevents users from modifying it</summary>
  /// <typeparam name="ItemType">Type of items to manage in the List</typeparam>
  public class ReadOnlyList<ItemType> :
    IList<ItemType>,
    IList {

    /// <summary>Initializes a new read-only List wrapper</summary>
    /// <param name="list">List that will be wrapped</param>
    public ReadOnlyList(IList<ItemType> list) {
      this.typedList = list;
      this.objectList = (list as IList);
    }

    /// <summary>Retrieves the index of an item within the List</summary>
    /// <param name="item">Item whose index will be returned</param>
    /// <returns>The zero-based index of the specified item in the List</returns>
    public int IndexOf(ItemType item) {
      return this.typedList.IndexOf(item);
    }

    /// <summary>Accesses the List item with the specified index</summary>
    /// <param name="index">Zero-based index of the List item that will be accessed</param>
    public ItemType this[int index] {
      get { return this.typedList[index]; }
    }

    /// <summary>Determines whether the List contains the specified item</summary>
    /// <param name="item">Item that will be checked for</param>
    /// <returns>True if the specified item is contained in the List</returns>
    public bool Contains(ItemType item) {
      return this.typedList.Contains(item);
    }

    /// <summary>Copies the contents of the List into an array</summary>
    /// <param name="array">Array the List will be copied into</param>
    /// <param name="arrayIndex">
    ///   Starting index at which to begin filling the destination array
    /// </param>
    public void CopyTo(ItemType[] array, int arrayIndex) {
      this.typedList.CopyTo(array, arrayIndex);
    }

    /// <summary>The number of items current contained in the List</summary>
    public int Count {
      get { return this.typedList.Count; }
    }

    /// <summary>Whether the List is write-protected</summary>
    public bool IsReadOnly {
      get { return true; }
    }

    /// <summary>Returns a new enumerator over the contents of the List</summary>
    /// <returns>The new List contents enumerator</returns>
    public IEnumerator<ItemType> GetEnumerator() {
      return this.typedList.GetEnumerator();
    }

    #region IList<> implementation

    /// <summary>Inserts an item into the List</summary>
    /// <param name="index">Zero-based index before which the item will be inserted</param>
    /// <param name="item">Item that will be inserted into the List</param>
    void IList<ItemType>.Insert(int index, ItemType item) {
      throw new NotSupportedException(
        "Inserting items is not supported by the read-only List"
      );
    }

    /// <summary>Removes an item from the list</summary>
    /// <param name="index">Zero-based index of the item that will be removed</param>
    void IList<ItemType>.RemoveAt(int index) {
      throw new NotSupportedException(
        "Removing items is not supported by the read-only List"
      );
    }

    /// <summary>Accesses the List item with the specified index</summary>
    /// <param name="index">Zero-based index of the List item that will be accessed</param>
    ItemType IList<ItemType>.this[int index] {
      get { return this.typedList[index]; }
      set {
        throw new NotSupportedException(
          "Assigning items is not supported by the read-only List"
        );
      }
    }

    #endregion

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
      return this.objectList.GetEnumerator();
    }

    #endregion

    #region IList implementation

    /// <summary>Removes all items from the List</summary>
    void IList.Clear() {
      throw new NotSupportedException(
        "Clearing is not supported by the read-only List"
      );
    }

    /// <summary>Adds an item to the end of the List</summary>
    /// <param name="value">Item that will be added to the List</param>
    int IList.Add(object value) {
      throw new NotSupportedException(
        "Adding items is not supported by the read-only List"
      );
    }

    /// <summary>Determines whether the List contains the specified item</summary>
    /// <param name="value">Item that will be checked for</param>
    /// <returns>True if the specified item is contained in the List</returns>
    bool IList.Contains(object value) {
      return this.objectList.Contains(value);
    }

    /// <summary>Retrieves the index of an item within the List</summary>
    /// <param name="value">Item whose index will be returned</param>
    /// <returns>The zero-based index of the specified item in the List</returns>
    int IList.IndexOf(object value) {
      return this.objectList.IndexOf(value);
    }

    /// <summary>Inserts an item into the List</summary>
    /// <param name="index">Zero-based index before which the item will be inserted</param>
    /// <param name="value">Item that will be inserted into the List</param>
    void IList.Insert(int index, object value) {
      throw new NotSupportedException(
        "Inserting items is not supported by the read-only List"
      );
    }

    /// <summary>Whether the size of the List is fixed</summary>
    bool IList.IsFixedSize {
      get { return this.objectList.IsFixedSize; }
    }

    /// <summary>Removes the specified item from the List</summary>
    /// <param name="value">Item that will be removed from the List</param>
    /// <returns>True of the specified item was found in the List and removed</returns>
    void IList.Remove(object value) {
      throw new NotSupportedException(
        "Removing items is not supported by the read-only List"
      );
    }

    /// <summary>Removes an item from the list</summary>
    /// <param name="index">Zero-based index of the item that will be removed</param>
    void IList.RemoveAt(int index) {
      throw new NotSupportedException(
        "Removing items is not supported by the read-only List"
      );
    }

    /// <summary>Accesses the List item with the specified index</summary>
    /// <param name="index">Zero-based index of the List item that will be accessed</param>
    object IList.this[int index] {
      get { return this.objectList[index]; }
      set {
        throw new NotSupportedException(
          "Assigning items is not supported by the read-only List"
        );
      }
    }

    #endregion

    #region ICollection implementation

    /// <summary>Copies the contents of the List into an array</summary>
    /// <param name="array">Array the List will be copied into</param>
    /// <param name="index">
    ///   Starting index at which to begin filling the destination array
    /// </param>
    void ICollection.CopyTo(Array array, int index) {
      this.objectList.CopyTo(array, index);
    }

    /// <summary>Whether the List is synchronized for multi-threaded usage</summary>
    bool ICollection.IsSynchronized {
      get { return this.objectList.IsSynchronized; }
    }

    /// <summary>Synchronization root on which the List locks</summary>
    object ICollection.SyncRoot {
      get { return this.objectList.SyncRoot; }
    }

    #endregion

    /// <summary>The wrapped List under its type-safe interface</summary>
    private IList<ItemType> typedList;
    /// <summary>The wrapped List under its object interface</summary>
    private IList objectList;

  }

} // namespace Nuclex.Support.Collections
