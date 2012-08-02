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
using System.Collections.ObjectModel;
#if !NO_SPECIALIZED_COLLECTIONS
using System.Collections.Specialized;
#endif

namespace Nuclex.Support.Collections {

  /// <summary>Collection which fires events when items are added or removed</summary>
  /// <typeparam name="TItem">Type of items the collection manages</typeparam>
  public class ObservableCollection<TItem> :
    ICollection<TItem>,
    ICollection,
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

    /// <summary>Initializes a new ObservableCollection with no items</summary>
    public ObservableCollection() : this(new Collection<TItem>()) {}

    /// <summary>
    ///   Initializes a new ObservableCollection as a wrapper for an existing collection
    /// </summary>
    /// <param name="collection">Collection that will be wrapped</param>
    /// <exception cref="System.ArgumentNullException">List is null</exception>
    public ObservableCollection(ICollection<TItem> collection) {
      this.typedCollection = collection;
      this.objectCollection = (collection as ICollection);
    }

    /// <summary>Removes all elements from the Collection</summary>
    public void Clear() {
      OnClearing();
      this.typedCollection.Clear();
      OnCleared();
#if !NO_SPECIALIZED_COLLECTIONS
      if(CollectionChanged != null) {
        CollectionChanged(this, CollectionResetEventArgs);
      }
#endif
    }

    /// <summary>Adds an item to the collection</summary>
    /// <param name="item">Collection an item will be added to</param>
    public void Add(TItem item) {
      this.typedCollection.Add(item);
      OnAdded(item);
#if !NO_SPECIALIZED_COLLECTIONS
      if(CollectionChanged != null) {
        CollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item)
        );
      }
#endif
    }

    /// <summary>Determines whether the collection contains the specified item</summary>
    /// <param name="item">Item the collection will be searched for</param>
    /// <returns>
    ///   True if the collection contains the specified item, false otherwise
    /// </returns>
    public bool Contains(TItem item) {
      return this.typedCollection.Contains(item);
    }

    /// <summary>Copies the contents of the collection into an array</summary>
    /// <param name="array">Array the collection's contents will be copied into</param>
    /// <param name="arrayIndex">
    ///   Index in the array where the collection's first item will be placed
    /// </param>
    public void CopyTo(TItem[] array, int arrayIndex) {
      this.typedCollection.CopyTo(array, arrayIndex);
    }

    /// <summary>The total number of items currently in the collection</summary>
    public int Count {
      get { return this.typedCollection.Count; }
    }

    /// <summary>Whether the collection is read-only</summary>
    public bool IsReadOnly {
      get { return this.typedCollection.IsReadOnly; }
    }

    /// <summary>Removes an item from the collection</summary>
    /// <param name="item">Item that will be removed from the collection</param>
    /// <returns>True if the item was found and removed, false otherwise</returns>
    public bool Remove(TItem item) {
      bool wasRemoved = this.typedCollection.Remove(item);
      if(wasRemoved) {
        OnRemoved(item);
#if !NO_SPECIALIZED_COLLECTIONS
        if(CollectionChanged != null) {
          CollectionChanged(
            this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item)
          );
        }
#endif
      }

      return wasRemoved;
    }

    /// <summary>Returns an enumerator for the items in the collection</summary>
    /// <returns>An enumeration for the items in the collection</returns>
    public IEnumerator<TItem> GetEnumerator() {
      return this.typedCollection.GetEnumerator();
    }

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

    #region IEnumerable implementation

    /// <summary>Returns an enumerator for the items in the collection</summary>
    /// <returns>An enumeration for the items in the collection</returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return this.objectCollection.GetEnumerator();
    }

    #endregion // IEnumerable implementation

    #region ICollection implementation

    /// <summary>Copies the contents of the collection into an array</summary>
    /// <param name="array">Array the collection's contents will be copied into</param>
    /// <param name="arrayIndex">
    ///   Index in the array where the collection's first item will be placed
    /// </param>
    void ICollection.CopyTo(Array array, int arrayIndex) {
      this.objectCollection.CopyTo(array, arrayIndex);
    }

    /// <summary>Whether the collection synchronizes accesses from multiple threads</summary>
    bool ICollection.IsSynchronized {
      get { return this.objectCollection.IsSynchronized; }
    }

    /// <summary>
    ///   Synchronization root used to synchronize threads accessing the collection
    /// </summary>
    object ICollection.SyncRoot {
      get { return this.objectCollection.SyncRoot; }
    }

    #endregion // IEnumerable implementation

#if !NO_SPECIALIZED_COLLECTIONS
    /// <summary>Fixed event args used to notify that the collection has reset</summary>
    private static readonly NotifyCollectionChangedEventArgs CollectionResetEventArgs =
      new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
#endif

    /// <summary>The wrapped collection under its type-safe interface</summary>
    private ICollection<TItem> typedCollection;
    /// <summary>The wrapped collection under its object interface</summary>
    private ICollection objectCollection;

  }

} // namespace Nuclex.Support.Collections
