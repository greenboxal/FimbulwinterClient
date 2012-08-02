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

  partial class MultiDictionary<TKey, TValue> {

    #region IEnumerable implementation

    /// <summary>Returns a new object enumerator for the Dictionary</summary>
    /// <returns>The new object enumerator</returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    #endregion

    #region IDictionary implementation

    /// <summary>Adds an item into the dictionary</summary>
    /// <param name="key">Key under which the item will be added</param>
    /// <param name="value">Item that will be added</param>
    void IDictionary.Add(object key, object value) {
      Add((TKey)key, (TValue)value);
    }

    /// <summary>Determines whether the specified key exists in the dictionary</summary>
    /// <param name="key">Key that will be checked for</param>
    /// <returns>True if an item with the specified key exists in the dictionary</returns>
    bool IDictionary.Contains(object key) {
      return this.objectDictionary.Contains(key);
    }

    /// <summary>Returns a new entry enumerator for the dictionary</summary>
    /// <returns>The new entry enumerator</returns>
    IDictionaryEnumerator IDictionary.GetEnumerator() {
      return new Enumerator(this);
    }

    /// <summary>Whether the size of the dictionary is fixed</summary>
    bool IDictionary.IsFixedSize {
      get { return this.objectDictionary.IsFixedSize; }
    }

    /// <summary>Returns a collection of all keys in the dictionary</summary>
    ICollection IDictionary.Keys {
      get { return this.objectDictionary.Keys; }
    }

    /// <summary>Returns a collection of all values stored in the dictionary</summary>
    ICollection IDictionary.Values {
      get {
        if(this.valueCollection == null) {
          this.valueCollection = new ValueCollection(this);
        }

        return this.valueCollection;
      }
    }

    /// <summary>Removes an item from the dictionary</summary>
    /// <param name="key">Key of the item that will be removed</param>
    void IDictionary.Remove(object key) {
      RemoveKey((TKey)key);
    }

    /// <summary>Accesses an item in the dictionary by its key</summary>
    /// <param name="key">Key of the item that will be accessed</param>
    /// <returns>The item with the specified key</returns>
    object IDictionary.this[object key] {
      get { return this.objectDictionary[key]; }
      set { this[(TKey)key] = (ICollection<TValue>)value; }
    }

    #endregion

    #region ICollection<KeyValuePair<TKey, TValue>> implementation

    /// <summary>Inserts an already prepared element into the dictionary</summary>
    /// <param name="item">Prepared element that will be added to the dictionary</param>
    void ICollection<KeyValuePair<TKey, TValue>>.Add(
      KeyValuePair<TKey, TValue> item
    ) {
      Add(item.Key, item.Value);
    }

    /// <summary>Removes all items from the dictionary</summary>
    /// <param name="itemToRemove">Item that will be removed from the dictionary</param>
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(
      KeyValuePair<TKey, TValue> itemToRemove
    ) {
      ICollection<TValue> values;
      if(!this.typedDictionary.TryGetValue(itemToRemove.Key, out values)) {
        return false;
      }

      if(values.Remove(itemToRemove.Value)) {
        if(values.Count == 0) {
          this.typedDictionary.Remove(itemToRemove.Key);
        }

        OnRemoved(itemToRemove);
        return true;
      } else {
        return false;
      }
    }

    #endregion

    #region ICollection implementation

    /// <summary>Copies the contents of the Dictionary into an array</summary>
    /// <param name="array">Array the Dictionary contents will be copied into</param>
    /// <param name="arrayIndex">
    ///   Starting index at which to begin filling the destination array
    /// </param>
    void ICollection.CopyTo(Array array, int arrayIndex) {
      foreach(KeyValuePair<TKey, ICollection<TValue>> item in this.typedDictionary) {
        foreach(TValue value in item.Value) {
          array.SetValue(new KeyValuePair<TKey, TValue>(item.Key, value), arrayIndex);
          ++arrayIndex;
        }
      }
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

    #region IDictionary<TKey, ICollection<TValue>> implementation

    /// <summary>Adds a series of values to a dictionary</summary>
    /// <param name="key">Key under which the values will be added</param>
    /// <param name="values">Values that will be added to the dictionary</param>
    void IDictionary<TKey, ICollection<TValue>>.Add(TKey key, ICollection<TValue> values) {
      ICollection<TValue> currentValues;
      if(!this.typedDictionary.TryGetValue(key, out currentValues)) {
        currentValues = new ValueList(this);
      }

      foreach(TValue value in values) {
        currentValues.Add(value);
      }
    }

    /// <summary>Removes all values with the specified key</summary>
    /// <param name="key">Key whose associated entries will be removed</param>
    /// <returns>True if at least one entry has been removed from the dictionary</returns>
    bool IDictionary<TKey, ICollection<TValue>>.Remove(TKey key) {
      return (RemoveKey(key) > 0);
    }

    /// <summary>Returns a collection of value collections</summary>
    ICollection<ICollection<TValue>> IDictionary<TKey, ICollection<TValue>>.Values {
      get { return this.typedDictionary.Values; }
    }

    #endregion // IDictionary<TKey, ICollection<TValue>> implementation

    #region ICollection<KeyValuePair<TKey, ICollection<TValue>>> implementation

    /// <summary>Adds a series of values to a dictionary</summary>
    /// <param name="item">Entry containing the values that will be added</param>
    void ICollection<KeyValuePair<TKey, ICollection<TValue>>>.Add(
      KeyValuePair<TKey, ICollection<TValue>> item
    ) {
      ICollection<TValue> currentValues;
      if(!this.typedDictionary.TryGetValue(item.Key, out currentValues)) {
        currentValues = new ValueList(this);
      }

      foreach(TValue value in item.Value) {
        currentValues.Add(value);
      }
    }

    /// <summary>
    ///   Checks whether the dictionary contains the specified key/value pair
    /// </summary>
    /// <param name="item">Key/value pair for which the dictionary will be checked</param>
    /// <returns>True if the dictionary contains the specified key/value pair</returns>
    bool ICollection<KeyValuePair<TKey, ICollection<TValue>>>.Contains(
      KeyValuePair<TKey, ICollection<TValue>> item
    ) {
      return this.typedDictionary.Contains(item);
    }

    /// <summary>Copies the contents of the dictionary into an array</summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    void ICollection<KeyValuePair<TKey, ICollection<TValue>>>.CopyTo(
      KeyValuePair<TKey, ICollection<TValue>>[] array, int arrayIndex
    ) {
      this.typedDictionary.CopyTo(array, arrayIndex);
    }

    /// <summary>Removes the specified key/value pair from the dictionary</summary>
    /// <param name="item">Key/value pair that will be removed</param>
    /// <returns>True if the key/value pair was contained in the dictionary</returns>
    bool ICollection<KeyValuePair<TKey, ICollection<TValue>>>.Remove(
      KeyValuePair<TKey, ICollection<TValue>> item
    ) {
      return this.typedDictionary.Remove(item);
    }

    /// <summary>Returns an enumerator for the dictionary</summary>
    /// <returns>An enumerator for the key/value pairs in the dictionary</returns>
    IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> IEnumerable<
      KeyValuePair<TKey, ICollection<TValue>>
    >.GetEnumerator() {
      return this.typedDictionary.GetEnumerator();
    }

    #endregion // ICollection<KeyValuePair<TKey, ICollection<TValue>>> implementation

  }

} // namespace Nuclex.Support.Collections
