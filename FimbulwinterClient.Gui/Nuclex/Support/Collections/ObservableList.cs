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

#if !NO_SPECIALIZED_COLLECTIONS
using System.Collections.Specialized;
#endif

namespace Nuclex.Support.Collections {

  /// <summary>List which fires events when items are added or removed</summary>
  /// <typeparam name="TItem">Type of items the collection manages</typeparam>
  public class ObservableList<TItem> : IList<TItem>, IList, ICollection,
#if !NO_SPECIALIZED_COLLECTIONS
    INotifyCollectionChanged,
#endif
    IObservableCollection<TItem> {

    /// <summary>Raised when an item has been added to the collection</summary>
    public event EventHandler<ItemEventArgs<TItem>> ItemAdded;
    /// <summary>Raised when an item is removed from the collection</summary>
    public event EventHandler<ItemEventArgs<TItem>> ItemRemoved;
    /// <summary>Raised when the collection is about to be cleared</summary>
    /// <remarks>
    ///   This could be covered by calling ItemRemoved for each item currently
    ///   contained in the collection, but it is often simpler and more efficient
    ///   to process the clearing of the entire collection as a special operation.
    /// </remarks>
    public event EventHandler Clearing;
    /// <summary>Raised when the collection has been cleared</summary>
    public event EventHandler Cleared;

#if !NO_SPECIALIZED_COLLECTIONS
    /// <summary>Called when the collection has changed</summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;
#endif

    /// <summary>
    ///   Initializes a new instance of the ObservableList class that is empty.
    /// </summary>
    public ObservableList() : this(new List<TItem>()) { }

    /// <summary>
    ///   Initializes a new instance of the ObservableList class as a wrapper
    ///   for the specified list.
    /// </summary>
    /// <param name="list">The list that is wrapped by the new collection.</param>
    /// <exception cref="System.ArgumentNullException">List is null</exception>
    public ObservableList(IList<TItem> list) {
      this.typedList = list;
      this.objectList = list as IList; // Gah!
    }

    /// <summary>Determines the index of the specified item in the list</summary>
    /// <param name="item">Item whose index will be determined</param>
    /// <returns>The index of the item in the list or -1 if not found</returns>
    public int IndexOf(TItem item) {
      return this.typedList.IndexOf(item);
    }

    /// <summary>Inserts an item into the list at the specified index</summary>
    /// <param name="index">Index the item will be insertted at</param>
    /// <param name="item">Item that will be inserted into the list</param>
    public void Insert(int index, TItem item) {
      this.typedList.Insert(index, item);
      OnAdded(item);
#if !NO_SPECIALIZED_COLLECTIONS
      OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
#endif
    }

    /// <summary>Removes the item at the specified index from the list</summary>
    /// <param name="index">Index at which the item will be removed</param>
    public void RemoveAt(int index) {
      TItem item = this.typedList[index];
      this.typedList.RemoveAt(index);
      OnRemoved(item);
#if !NO_SPECIALIZED_COLLECTIONS
      OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
#endif
    }

    /// <summary>Accesses the item at the specified index in the list</summary>
    /// <param name="index">Index of the item that will be accessed</param>
    /// <returns>The item at the specified index</returns>
    public TItem this[int index] {
      get { return this.typedList[index]; }
      set {
        TItem oldItem = this.typedList[index];
        this.typedList[index] = value;
        OnRemoved(oldItem);
        OnAdded(value);
#if !NO_SPECIALIZED_COLLECTIONS
        if(CollectionChanged != null) {
          CollectionChanged(
            this, new NotifyCollectionChangedEventArgs(
              NotifyCollectionChangedAction.Replace, value, oldItem, index
            )
          );
        }
#endif

      }
    }

    /// <summary>Adds an item to the end of the list</summary>
    /// <param name="item">Item that will be added to the list</param>
    public void Add(TItem item) {
      this.typedList.Add(item);
      OnAdded(item);
    }

    /// <summary>Removes all items from the list</summary>
    public void Clear() {
      OnClearing();
      this.typedList.Clear();
      OnCleared();
#if !NO_SPECIALIZED_COLLECTIONS
      if(CollectionChanged != null) {
        CollectionChanged(this, CollectionResetEventArgs);
      }
#endif
    }

    /// <summary>Checks whether the list contains the specified item</summary>
    /// <param name="item">Item the list will be checked for</param>
    /// <returns>True if the list contains the specified items</returns>
    public bool Contains(TItem item) {
      return this.typedList.Contains(item);
    }

    /// <summary>Copies the contents of the list into an array</summary>
    /// <param name="array">Array the list will be copied into</param>
    /// <param name="arrayIndex">
    ///   Index in the target array where the first item will be copied to
    /// </param>
    public void CopyTo(TItem[] array, int arrayIndex) {
      this.typedList.CopyTo(array, arrayIndex);
    }

    /// <summary>Total number of items in the list</summary>
    public int Count {
      get { return this.typedList.Count; }
    }

    /// <summary>Whether the list is a read-only list</summary>
    public bool IsReadOnly {
      get { return this.typedList.IsReadOnly; }
    }

    /// <summary>Removes the specified item from the list</summary>
    /// <param name="item">Item that will be removed from the list</param>
    /// <returns>
    ///   True if the item was found and removed from the list, false otherwise
    /// </returns>
    public bool Remove(TItem item) {
      int index = this.typedList.IndexOf(item);
      if(index == -1) {
        return false;
      }

      TItem removedItem = this.typedList[index];
      this.typedList.RemoveAt(index);
      OnRemoved(removedItem);
#if !NO_SPECIALIZED_COLLECTIONS
      OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
#endif
      return true;
    }

    /// <summary>Returns an enumerator for the items in the list</summary>
    /// <returns>An enumerator for the list's items</returns>
    public IEnumerator<TItem> GetEnumerator() {
      return this.typedList.GetEnumerator();
    }

    #region IEnumerable implementation

    /// <summary>Returns an enumerator for the items in the list</summary>
    /// <returns>An enumerator for the list's items</returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return this.objectList.GetEnumerator();
    }

    #endregion // IEnumerable implementation

    #region ICollection implementation

    /// <summary>Copies the contents of the list into an array</summary>
    /// <param name="array">Array the list will be copied into</param>
    /// <param name="arrayIndex">
    ///   Index in the target array where the first item will be copied to
    /// </param>
    void ICollection.CopyTo(Array array, int arrayIndex) {
      this.objectList.CopyTo(array, arrayIndex);
    }

    /// <summary>Whether this list performs thread synchronization</summary>
    bool ICollection.IsSynchronized {
      get { return this.objectList.IsSynchronized; }
    }

    /// <summary>Synchronization root used by the list to synchronize threads</summary>
    object ICollection.SyncRoot {
      get { return this.objectList.SyncRoot; }
    }

    #endregion // ICollection implementation

    #region IList implementation

    /// <summary>Adds an item to the list</summary>
    /// <param name="value">Item that will be added to the list</param>
    /// <returns>
    ///   The position at which the item has been inserted or -1 if the item was not inserted
    /// </returns>
    int IList.Add(object value) {
      int index = this.objectList.Add(value);
      TItem addedItem = this.typedList[index];
      OnAdded(addedItem);
#if !NO_SPECIALIZED_COLLECTIONS
      OnCollectionChanged(NotifyCollectionChangedAction.Add, addedItem, index);
#endif
      return index;
    }

    /// <summary>Checks whether the list contains the specified item</summary>
    /// <param name="item">Item the list will be checked for</param>
    /// <returns>True if the list contains the specified items</returns>
    bool IList.Contains(object item) {
      return this.objectList.Contains(item);
    }

    /// <summary>Determines the index of the specified item in the list</summary>
    /// <param name="item">Item whose index will be determined</param>
    /// <returns>The index of the item in the list or -1 if not found</returns>
    int IList.IndexOf(object item) {
      return this.objectList.IndexOf(item);
    }

    /// <summary>Inserts an item into the list at the specified index</summary>
    /// <param name="index">Index the item will be insertted at</param>
    /// <param name="item">Item that will be inserted into the list</param>
    void IList.Insert(int index, object item) {
      this.objectList.Insert(index, item);
      TItem addedItem = this.typedList[index];
      OnAdded(addedItem);
#if !NO_SPECIALIZED_COLLECTIONS
      OnCollectionChanged(NotifyCollectionChangedAction.Add, addedItem, index);
#endif
    }

    /// <summary>Whether the list is of a fixed size</summary>
    bool IList.IsFixedSize {
      get { return this.objectList.IsFixedSize; }
    }

    /// <summary>Removes the specified item from the list</summary>
    /// <param name="item">Item that will be removed from the list</param>
    void IList.Remove(object item) {
      int index = this.objectList.IndexOf(item);
      if(index == -1) {
        return;
      }

      TItem removedItem = this.typedList[index];
      this.objectList.RemoveAt(index);
      OnRemoved(removedItem);
#if !NO_SPECIALIZED_COLLECTIONS
      OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
#endif
    }

    /// <summary>Accesses the item at the specified index in the list</summary>
    /// <param name="index">Index of the item that will be accessed</param>
    /// <returns>The item at the specified index</returns>
    object IList.this[int index] {
      get { return this.objectList[index]; }
      set {
        TItem oldItem = this.typedList[index];
        this.objectList[index] = value;
        TItem newItem = this.typedList[index];
        OnRemoved(oldItem);
        OnAdded(newItem);
#if !NO_SPECIALIZED_COLLECTIONS
        if(CollectionChanged != null) {
          CollectionChanged(
            this, new NotifyCollectionChangedEventArgs(
              NotifyCollectionChangedAction.Replace, newItem, oldItem, index
            )
          );
        }
#endif
      }
    }

    #endregion // IList implementation

#if !NO_SPECIALIZED_COLLECTIONS
    /// <summary>Fires the CollectionChanged event</summary>
    /// <param name="action">Type of change that has occured</param>
    /// <param name="item">The item that has been added, removed or replaced</param>
    /// <param name="index">Index of the changed item</param>
    protected virtual void OnCollectionChanged(
      NotifyCollectionChangedAction action, TItem item, int index
    ) {
      if(CollectionChanged != null) {
        CollectionChanged(
          this, new NotifyCollectionChangedEventArgs(action, item, index)
        );
      }
    }
#endif

    /// <summary>Fires the 'ItemAdded' event</summary>
    /// <param name="item">Item that has been added to the collection</param>
    protected virtual void OnAdded(TItem item) {
      if(ItemAdded != null)
        ItemAdded(this, new ItemEventArgs<TItem>(item));
    }

    /// <summary>Fires the 'ItemRemoved' event</summary>
    /// <param name="item">Item that has been removed from the collection</param>
    protected virtual void OnRemoved(TItem item) {
      if(ItemRemoved != null)
        ItemRemoved(this, new ItemEventArgs<TItem>(item));
    }

    /// <summary>Fires the 'Clearing' event</summary>
    protected virtual void OnClearing() {
      if(Clearing != null)
        Clearing(this, EventArgs.Empty);
    }

    /// <summary>Fires the 'Cleared' event</summary>
    protected virtual void OnCleared() {
      if(Cleared != null)
        Cleared(this, EventArgs.Empty);
    }

#if !NO_SPECIALIZED_COLLECTIONS
    /// <summary>Fixed event args used to notify that the collection has reset</summary>
    private static readonly NotifyCollectionChangedEventArgs CollectionResetEventArgs =
      new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
#endif

    /// <summary>The wrapped list under its type-safe interface</summary>
    private IList<TItem> typedList;
    /// <summary>The wrapped list under its object interface</summary>
    private IList objectList;

  }

} // namespace Nuclex.Support.Collections
