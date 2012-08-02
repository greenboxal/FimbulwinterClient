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
using System.Runtime.Serialization;

namespace Nuclex.Support.Collections {

  /// <summary>A dictionary that sneds out change notifications</summary>
  /// <typeparam name="TKey">Type of the keys used in the dictionary</typeparam>
  /// <typeparam name="TValue">Type of the values used in the dictionary</typeparam>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class ObservableDictionary<TKey, TValue> :
#if !NO_SERIALIZATION
    ISerializable,
    IDeserializationCallback,
#endif
    IDictionary<TKey, TValue>,
    IDictionary,
#if !NO_SPECIALIZED_COLLECTIONS
    INotifyCollectionChanged,
#endif
    IObservableCollection<KeyValuePair<TKey, TValue>> {

#if !NO_SERIALIZATION
    #region class SerializedDictionary

    /// <summary>
    ///   Dictionary wrapped used to reconstruct a serialized read only dictionary
    /// </summary>
    private class SerializedDictionary : Dictionary<TKey, TValue> {

      /// <summary>
      ///   Initializes a new instance of the System.WeakReference class, using deserialized
      ///   data from the specified serialization and stream objects.
      /// </summary>
      /// <param name="info">
      ///   An object that holds all the data needed to serialize or deserialize the
      ///   current System.WeakReference object.
      /// </param>
      /// <param name="context">
      ///   (Reserved) Describes the source and destination of the serialized stream
      ///   specified by info.
      /// </param>
      /// <exception cref="System.ArgumentNullException">
      ///   The info parameter is null.
      /// </exception>
      public SerializedDictionary(SerializationInfo info, StreamingContext context) :
        base(info, context) { }

    }

    #endregion // class SerializedDictionary
#endif // !NO_SERIALIZATION

    /// <summary>Raised when an item has been added to the dictionary</summary>
    public event EventHandler<ItemEventArgs<KeyValuePair<TKey, TValue>>> ItemAdded;
    /// <summary>Raised when an item is removed from the dictionary</summary>
    public event EventHandler<ItemEventArgs<KeyValuePair<TKey, TValue>>> ItemRemoved;
    /// <summary>Raised when the dictionary is about to be cleared</summary>
    public event EventHandler Clearing;
    /// <summary>Raised when the dictionary has been cleared</summary>
    public event EventHandler Cleared;

#if !NO_SPECIALIZED_COLLECTIONS
    /// <summary>Called when the collection has changed</summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;
#endif

    /// <summary>Initializes a new observable dictionary</summary>
    public ObservableDictionary() : this(new Dictionary<TKey, TValue>()) { }

    /// <summary>Initializes a new observable Dictionary wrapper</summary>
    /// <param name="dictionary">Dictionary that will be wrapped</param>
    public ObservableDictionary(IDictionary<TKey, TValue> dictionary) {
      this.typedDictionary = dictionary;
      this.objectDictionary = (this.typedDictionary as IDictionary);
    }

#if !NO_SERIALIZATION

    /// <summary>
    ///   Initializes a new instance of the System.WeakReference class, using deserialized
    ///   data from the specified serialization and stream objects.
    /// </summary>
    /// <param name="info">
    ///   An object that holds all the data needed to serialize or deserialize the
    ///   current System.WeakReference object.
    /// </param>
    /// <param name="context">
    ///   (Reserved) Describes the source and destination of the serialized stream
    ///   specified by info.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    ///   The info parameter is null.
    /// </exception>
    protected ObservableDictionary(SerializationInfo info, StreamingContext context) :
      this(new SerializedDictionary(info, context)) { }

#endif // !NO_SERIALIZATION

    /// <summary>Whether the directory is write-protected</summary>
    public bool IsReadOnly {
      get { return this.typedDictionary.IsReadOnly; }
    }

    /// <summary>
    ///   Determines whether the specified KeyValuePair is contained in the Dictionary
    /// </summary>
    /// <param name="item">KeyValuePair that will be checked for</param>
    /// <returns>True if the provided KeyValuePair was contained in the Dictionary</returns>
    public bool Contains(KeyValuePair<TKey, TValue> item) {
      return this.typedDictionary.Contains(item);
    }

    /// <summary>Determines whether the Dictionary contains the specified key</summary>
    /// <param name="key">Key that will be checked for</param>
    /// <returns>
    ///   True if an entry with the specified key was contained in the Dictionary
    /// </returns>
    public bool ContainsKey(TKey key) {
      return this.typedDictionary.ContainsKey(key);
    }

    /// <summary>Copies the contents of the Dictionary into an array</summary>
    /// <param name="array">Array the Dictionary will be copied into</param>
    /// <param name="arrayIndex">
    ///   Starting index at which to begin filling the destination array
    /// </param>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
      this.typedDictionary.CopyTo(array, arrayIndex);
    }

    /// <summary>Number of elements contained in the Dictionary</summary>
    public int Count {
      get { return this.typedDictionary.Count; }
    }

    /// <summary>Creates a new enumerator for the Dictionary</summary>
    /// <returns>The new Dictionary enumerator</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
      return this.typedDictionary.GetEnumerator();
    }

    /// <summary>Collection of all keys contained in the Dictionary</summary>
    public ICollection<TKey> Keys {
      get { return this.typedDictionary.Keys; }
    }

    /// <summary>Collection of all values contained in the Dictionary</summary>
    public ICollection<TValue> Values {
      get { return this.typedDictionary.Values; }
    }

    /// <summary>
    ///   Attempts to retrieve the item with the specified key from the Dictionary
    /// </summary>
    /// <param name="key">Key of the item to attempt to retrieve</param>
    /// <param name="value">
    ///   Output parameter that will receive the key upon successful completion
    /// </param>
    /// <returns>
    ///   True if the item was found and has been placed in the output parameter
    /// </returns>
    public bool TryGetValue(TKey key, out TValue value) {
      return this.typedDictionary.TryGetValue(key, out value);
    }

    /// <summary>Accesses an item in the Dictionary by its key</summary>
    /// <param name="key">Key of the item that will be accessed</param>
    public TValue this[TKey key] {
      get { return this.typedDictionary[key]; }
      set {
        bool removed;
        TValue oldValue;
        removed = this.typedDictionary.TryGetValue(key, out oldValue);

        this.typedDictionary[key] = value;

        if(removed) {
          OnRemoved(new KeyValuePair<TKey, TValue>(key, oldValue));
        }
        OnAdded(new KeyValuePair<TKey, TValue>(key, value));
      }
    }

    /// <summary>Inserts an item into the Dictionary</summary>
    /// <param name="key">Key under which to add the new item</param>
    /// <param name="value">Item that will be added to the Dictionary</param>
    public void Add(TKey key, TValue value) {
      this.typedDictionary.Add(key, value);
      OnAdded(new KeyValuePair<TKey, TValue>(key, value));
    }

    /// <summary>Removes the item with the specified key from the Dictionary</summary>
    /// <param name="key">Key of the elementes that will be removed</param>
    /// <returns>True if an item with the specified key was found and removed</returns>
    public bool Remove(TKey key) {
      TValue oldValue;
      this.typedDictionary.TryGetValue(key, out oldValue);

      bool removed = this.typedDictionary.Remove(key);
      if(removed) {
        OnRemoved(new KeyValuePair<TKey, TValue>(key, oldValue));
      }
      return removed;
    }

    /// <summary>Removes all items from the Dictionary</summary>
    public void Clear() {
      OnClearing();
      this.typedDictionary.Clear();
      OnCleared();
    }

    /// <summary>Fires the 'ItemAdded' event</summary>
    /// <param name="item">Item that has been added to the collection</param>
    protected virtual void OnAdded(KeyValuePair<TKey, TValue> item) {
      if(ItemAdded != null)
        ItemAdded(this, new ItemEventArgs<KeyValuePair<TKey, TValue>>(item));

#if !NO_SPECIALIZED_COLLECTIONS
      if(CollectionChanged != null)
        CollectionChanged(
          this, new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Add, item
          )
        );
#endif
    }

    /// <summary>Fires the 'ItemRemoved' event</summary>
    /// <param name="item">Item that has been removed from the collection</param>
    protected virtual void OnRemoved(KeyValuePair<TKey, TValue> item) {
      if(ItemRemoved != null)
        ItemRemoved(this, new ItemEventArgs<KeyValuePair<TKey, TValue>>(item));

#if !NO_SPECIALIZED_COLLECTIONS
      if(CollectionChanged != null)
        CollectionChanged(
          this, new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Remove, item
          )
        );
#endif
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

#if !NO_SPECIALIZED_COLLECTIONS
      if(CollectionChanged != null)
        CollectionChanged(this, CollectionResetEventArgs);
#endif
    }

    #region IEnumerable implementation

    /// <summary>Returns a new object enumerator for the Dictionary</summary>
    /// <returns>The new object enumerator</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return (this.typedDictionary as IEnumerable).GetEnumerator();
    }

    #endregion

    #region IDictionary implementation

    /// <summary>Adds an item into the Dictionary</summary>
    /// <param name="key">Key under which the item will be added</param>
    /// <param name="value">Item that will be added</param>
    void IDictionary.Add(object key, object value) {
      this.objectDictionary.Add(key, value);
      OnAdded(new KeyValuePair<TKey, TValue>((TKey)key, (TValue)value));
    }

    /// <summary>Determines whether the specified key exists in the Dictionary</summary>
    /// <param name="key">Key that will be checked for</param>
    /// <returns>True if an item with the specified key exists in the Dictionary</returns>
    bool IDictionary.Contains(object key) {
      return this.objectDictionary.Contains(key);
    }

    /// <summary>Returns a new entry enumerator for the dictionary</summary>
    /// <returns>The new entry enumerator</returns>
    IDictionaryEnumerator IDictionary.GetEnumerator() {
      return this.objectDictionary.GetEnumerator();
    }

    /// <summary>Whether the size of the Dictionary is fixed</summary>
    bool IDictionary.IsFixedSize {
      get { return this.objectDictionary.IsFixedSize; }
    }

    /// <summary>Returns a collection of all keys in the Dictionary</summary>
    ICollection IDictionary.Keys {
      get { return this.objectDictionary.Keys; }
    }

    /// <summary>Returns a collection of all values stored in the Dictionary</summary>
    ICollection IDictionary.Values {
      get { return this.objectDictionary.Values; }
    }

    /// <summary>Removes an item from the Dictionary</summary>
    /// <param name="key">Key of the item that will be removed</param>
    void IDictionary.Remove(object key) {
      TValue value;
      bool removed = this.typedDictionary.TryGetValue((TKey)key, out value);
      this.objectDictionary.Remove(key);
      if(removed) {
        OnRemoved(new KeyValuePair<TKey, TValue>((TKey)key, (TValue)value));
      }
    }

    /// <summary>Accesses an item in the Dictionary by its key</summary>
    /// <param name="key">Key of the item that will be accessed</param>
    /// <returns>The item with the specified key</returns>
    object IDictionary.this[object key] {
      get { return this.objectDictionary[key]; }
      set {
        bool removed;
        TValue oldValue;
        removed = this.typedDictionary.TryGetValue((TKey)key, out oldValue);

        this.objectDictionary[key] = value;

        if(removed) {
          OnRemoved(new KeyValuePair<TKey, TValue>((TKey)key, oldValue));
        }
        OnAdded(new KeyValuePair<TKey, TValue>((TKey)key, (TValue)value));
      }
    }

    #endregion

    #region ICollection<> implementation

    /// <summary>Inserts an already prepared element into the Dictionary</summary>
    /// <param name="item">Prepared element that will be added to the Dictionary</param>
    void ICollection<KeyValuePair<TKey, TValue>>.Add(
      KeyValuePair<TKey, TValue> item
    ) {
      this.typedDictionary.Add(item);
      OnAdded(item);
    }

    /// <summary>Removes all items from the Dictionary</summary>
    void ICollection<KeyValuePair<TKey, TValue>>.Clear() {
      OnClearing();
      this.typedDictionary.Clear();
      OnCleared();
    }

    /// <summary>Removes all items from the Dictionary</summary>
    /// <param name="itemToRemove">Item that will be removed from the Dictionary</param>
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(
      KeyValuePair<TKey, TValue> itemToRemove
    ) {
      bool removed = this.typedDictionary.Remove(itemToRemove);
      if(removed) {
        OnRemoved(itemToRemove);
      }
      return removed;
    }

    #endregion

    #region ICollection implementation

    /// <summary>Copies the contents of the Dictionary into an array</summary>
    /// <param name="array">Array the Dictionary contents will be copied into</param>
    /// <param name="index">
    ///   Starting index at which to begin filling the destination array
    /// </param>
    void ICollection.CopyTo(Array array, int index) {
      this.objectDictionary.CopyTo(array, index);
    }

    /// <summary>Whether the Dictionary is synchronized for multi-threaded usage</summary>
    bool ICollection.IsSynchronized {
      get { return this.objectDictionary.IsSynchronized; }
    }

    /// <summary>Synchronization root on which the Dictionary locks</summary>
    object ICollection.SyncRoot {
      get { return this.objectDictionary.SyncRoot; }
    }

    #endregion

#if !NO_SERIALIZATION
    #region ISerializable implementation

    /// <summary>Serializes the Dictionary</summary>
    /// <param name="info">
    ///   Provides the container into which the Dictionary will serialize itself
    /// </param>
    /// <param name="context">
    ///   Contextual informations about the serialization environment
    /// </param>
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
      (this.typedDictionary as ISerializable).GetObjectData(info, context);
    }

    /// <summary>Called after all objects have been successfully deserialized</summary>
    /// <param name="sender">Nicht unterstützt</param>
    void IDeserializationCallback.OnDeserialization(object sender) {
      (this.typedDictionary as IDeserializationCallback).OnDeserialization(sender);
    }

    #endregion
#endif //!NO_SERIALIZATION

    /// <summary>The wrapped Dictionary under its type-safe interface</summary>
    private IDictionary<TKey, TValue> typedDictionary;
    /// <summary>The wrapped Dictionary under its object interface</summary>
    private IDictionary objectDictionary;

#if !NO_SPECIALIZED_COLLECTIONS
    /// <summary>Fixed event args used to notify that the collection has reset</summary>
    private static readonly NotifyCollectionChangedEventArgs CollectionResetEventArgs =
      new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
#endif

  }

} // namespace Nuclex.Support.Collections
