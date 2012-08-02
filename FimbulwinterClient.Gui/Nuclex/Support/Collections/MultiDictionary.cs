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

namespace Nuclex.Support.Collections {

  /// <summary>Dictionary that can contain multiple values under the same key</summary>
  /// <typeparam name="TKey">Type of keys used within the dictionary</typeparam>
  /// <typeparam name="TValue">Type of values used within the dictionary</typeparam>
  public partial class MultiDictionary<TKey, TValue> : IMultiDictionary<TKey, TValue> {

    #region class Enumerator

    /// <summary>Enumerates the values stored in a multi dictionary</summary>
    private class Enumerator :
      IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator {

      /// <summary>Initializes a new multi dictionary enumerator</summary>
      /// <param name="dictionary">Dictionary that will be enumerated</param>
      public Enumerator(MultiDictionary<TKey, TValue> dictionary) {
        this.dictionary = dictionary;

        Reset();
      }

      /// <summary>The current entry the enumerator is pointing at</summary>
      public KeyValuePair<TKey, TValue> Current {
        get {
          if(this.currentValue == null) {
            throw new InvalidOperationException("Enumerator is not on a valid position");
          }

          return new KeyValuePair<TKey, TValue>(
            this.currentCollection.Current.Key, this.currentValue.Current
          );
        }
      }

      /// <summary>Immediately releases all resources owned by the instance</summary>
      public void Dispose() {
        if(this.currentValue != null) {
          this.currentValue.Dispose();
          this.currentValue = null;
        }
        if(this.currentCollection != null) {
          this.currentCollection.Dispose();
          this.currentCollection = null;
        }
      }

      /// <summary>Advances the enumerator to the entry</summary>
      /// <returns>
      ///   True if there was a next entry, false if the end of the set has been reached
      /// </returns>
      public bool MoveNext() {
        if(this.currentCollection == null) {
          return false;
        }

        for(; ; ) {

          // Try to move the enumerator in the current key's list to the next item
          if(this.currentValue != null) {
            if(this.currentValue.MoveNext()) {
              return true; // We found the next item
            } else {
              this.currentValue.Dispose();
            }
          }

          // Enumerator for the current key's list reached the end, go to the next key
          if(this.currentCollection.MoveNext()) {
            this.currentValue = this.currentCollection.Current.Value.GetEnumerator();
          } else {
            this.currentValue = null; // Guaranteed to be disposed already
            this.currentCollection.Dispose();
            this.currentCollection = null;
            return false;
          }

        }
      }

      /// <summary>Resets the enumerator to its initial position</summary>
      public void Reset() {
        if(this.currentValue != null) {
          this.currentValue.Dispose();
          this.currentValue = null;
        }
        if(this.currentCollection != null) {
          this.currentCollection.Dispose();
        }
        this.currentCollection = this.dictionary.GetEnumerator();
      }

      #region IEnumerator implementation

      /// <summary>The item the enumerator is currently pointing at</summary>
      object IEnumerator.Current {
        get { return Current; }
      }

      #endregion // IEnumerator implementation

      #region IDictionaryEnumerator implementation

      /// <summary>The current entry the enumerator is pointing to</summary>
      DictionaryEntry IDictionaryEnumerator.Entry {
        get {
          enforceEnumeratorOnValidPosition();

          return new DictionaryEntry(
            this.currentCollection.Current.Key, this.currentValue.Current
          );
        }
      }

      /// <summary>
      ///   Throws an exception if the enumerator is not on a valid position
      /// </summary>
      private void enforceEnumeratorOnValidPosition() {
        if(this.currentValue == null) {
          throw new InvalidOperationException("Enumerator is not on a valid position");
        }
      }

      /// <summary>The current dictionary key</summary>
      object IDictionaryEnumerator.Key {
        get {
          enforceEnumeratorOnValidPosition();
          return this.currentCollection.Current.Key;
        }
      }

      /// <summary>The current dictionary value</summary>
      object IDictionaryEnumerator.Value {
        get {
          enforceEnumeratorOnValidPosition();
          return this.currentValue.Current;
        }
      }

      #endregion // IDictionaryEnumerator implementation

      /// <summary>Dictionary over whose entries the enumerator is enumerating</summary>
      private IDictionary<TKey, ICollection<TValue>> dictionary;
      /// <summary>Current key the enumerator is at</summary>
      private IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> currentCollection;
      /// <summary>Current value in the current key the enumerator is at</summary>
      private IEnumerator<TValue> currentValue;

    }

    #endregion // class Enumerator

    #region class ValueList

    /// <summary>Stores the list of values for a dictionary key</summary>
    private class ValueList : Collection<TValue> {

      /// <summary>Initializes a new value list</summary>
      /// <param name="dictionary">Dictionary the value list belongs to</param>
      public ValueList(MultiDictionary<TKey, TValue> dictionary) {
        this.dictionary = dictionary;
      }

      /// <summary>Called when the value list is being cleared</summary>
      protected override void ClearItems() {
        this.dictionary.count -= Count;
        base.ClearItems();
      }

      /// <summary>Called when an item is inserted into the value list</summary>
      /// <param name="index">Index at which the item is being inserted</param>
      /// <param name="item">Item that is being inserted</param>
      protected override void InsertItem(int index, TValue item) {
        base.InsertItem(index, item);
        ++this.dictionary.count;
      }

      /// <summary>Called when an item is removed from the value list</summary>
      /// <param name="index">Index at which the item is being removed</param>
      protected override void RemoveItem(int index) {
        base.RemoveItem(index);
        --this.dictionary.count;
      }

      /// <summary>The dictionary the value list belongs to</summary>
      private MultiDictionary<TKey, TValue> dictionary;

    }

    #endregion // class ValueList

    /// <summary>Initializes a new multi dictionary</summary>
    public MultiDictionary() : this(new Dictionary<TKey, ICollection<TValue>>()) { }

    /// <summary>Initializes a new multi dictionary</summary>
    /// <param name="dictionary">Dictionary the multi dictionary will be based on</param>
    internal MultiDictionary(IDictionary<TKey, ICollection<TValue>> dictionary) {
      this.typedDictionary = dictionary;
      this.objectDictionary = (this.typedDictionary as IDictionary);

      foreach(ICollection<TValue> values in dictionary.Values) {
        this.count += values.Count;
      }
    }

    /// <summary>Whether the dictionary is write-protected</summary>
    public bool IsReadOnly {
      get { return this.typedDictionary.IsReadOnly; }
    }

    /// <summary>Determines the number of values stored under the specified key</summary>
    /// <param name="key">Key whose values will be counted</param>
    /// <returns>The number of values stored under the specified key</returns>
    public int CountValues(TKey key) {
      ICollection<TValue> values;
      if(this.typedDictionary.TryGetValue(key, out values)) {
        return values.Count;
      } else {
        return 0;
      }
    }

    /// <summary>
    ///   Determines whether the specified KeyValuePair is contained in the dictionary
    /// </summary>
    /// <param name="item">KeyValuePair that will be checked for</param>
    /// <returns>True if the provided KeyValuePair was contained in the dictionary</returns>
    public bool Contains(KeyValuePair<TKey, TValue> item) {
      ICollection<TValue> values;
      if(this.typedDictionary.TryGetValue(item.Key, out values)) {
        return values.Contains(item.Value);
      } else {
        return false;
      }
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
      foreach(KeyValuePair<TKey, ICollection<TValue>> item in this.typedDictionary) {
        foreach(TValue value in item.Value) {
          array[arrayIndex] = new KeyValuePair<TKey, TValue>(item.Key, value);
          ++arrayIndex;
        }
      }
    }

    /// <summary>Number of elements contained in the Dictionary</summary>
    public int Count {
      get { return this.count; }
    }

    /// <summary>Creates a new enumerator for the dictionary</summary>
    /// <returns>The new dictionary enumerator</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
      return new Enumerator(this);
    }

    /// <summary>Collection of all keys contained in the dictionary</summary>
    public ICollection<TKey> Keys {
      get { return this.typedDictionary.Keys; }
    }

    /// <summary>Collection of all values contained in the dictionary</summary>
    public ICollection<TValue> Values {
      get {
        if(this.valueCollection == null) {
          this.valueCollection = new ValueCollection(this);
        }

        return this.valueCollection;
      }
    }

    /// <summary>
    ///   Attempts to retrieve the item with the specified key from the dictionary
    /// </summary>
    /// <param name="key">Key of the item to attempt to retrieve</param>
    /// <param name="values">
    ///   Output parameter that will receive the values upon successful completion
    /// </param>
    /// <returns>
    ///   True if the item was found and has been placed in the output parameter
    /// </returns>
    public bool TryGetValue(TKey key, out ICollection<TValue> values) {
      return this.typedDictionary.TryGetValue(key, out values);
    }

    /// <summary>Accesses an item in the dictionary by its key</summary>
    /// <param name="key">Key of the item that will be accessed</param>
    public ICollection<TValue> this[TKey key] {
      get { return this.typedDictionary[key]; }
      set {
        if(value == null) {
          this.typedDictionary.Remove(key);
        }

        ICollection<TValue> currentValues;
        if(this.typedDictionary.TryGetValue(key, out currentValues)) {
          currentValues.Clear();
        } else {
          currentValues = new ValueList(this);
          this.typedDictionary.Add(key, currentValues);
        }

        foreach(TValue addedValue in value) {
          currentValues.Add(addedValue);
          OnAdded(new KeyValuePair<TKey, TValue>(key, addedValue));
        }
      }
    }

    /// <summary>Inserts an item into the dictionary</summary>
    /// <param name="key">Key under which to add the new item</param>
    /// <param name="value">Item that will be added to the dictionary</param>
    public void Add(TKey key, TValue value) {
      ICollection<TValue> values;
      if(!this.typedDictionary.TryGetValue(key, out values)) {
        values = new ValueList(this);
        this.typedDictionary.Add(key, values);
      }

      values.Add(value);
      OnAdded(new KeyValuePair<TKey, TValue>(key, value));
    }

    /// <summary>
    ///   Removes the item with the specified key and value from the dictionary
    /// </summary>
    /// <param name="key">Key of the item that will be removed</param>
    /// <param name="value">Value of the item that will be removed</param>
    /// <returns>
    ///   True if the specified item was contained in the dictionary and was removed
    /// </returns>
    /// <exception cref="NotSupportedException">If the dictionary is read-only</exception>
    public bool Remove(TKey key, TValue value) {
      ICollection<TValue> values;
      if(this.typedDictionary.TryGetValue(key, out values)) {
        values.Remove(value);

        if(values.Count == 0) {
          this.typedDictionary.Remove(key);
        }

        OnRemoved(new KeyValuePair<TKey, TValue>(key, value));
        return true;
      } else {
        return false;
      }
    }

    /// <summary>Removes all items with the specified key from the dictionary</summary>
    /// <param name="key">Key of the item that will be removed</param>
    /// <returns>The number of items that have been removed from the dictionary</returns>
    /// <exception cref="NotSupportedException">If the dictionary is read-only</exception>
    public int RemoveKey(TKey key) {
      ICollection<TValue> values;
      if(this.typedDictionary.TryGetValue(key, out values)) {
        this.count -= values.Count;
        this.typedDictionary.Remove(key);

        foreach(TValue value in values) {
          OnRemoved(new KeyValuePair<TKey, TValue>(key, value));
        }
        return values.Count;
      } else {
        return 0;
      }
    }

    /// <summary>Removes all items from the Dictionary</summary>
    public void Clear() {
      OnClearing();
      this.typedDictionary.Clear();
      this.count = 0;
      OnCleared();
    }

    /// <summary>Fires the 'ItemAdded' event</summary>
    /// <param name="item">Item that has been added to the collection</param>
    protected virtual void OnAdded(KeyValuePair<TKey, TValue> item) { }

    /// <summary>Fires the 'ItemRemoved' event</summary>
    /// <param name="item">Item that has been removed from the collection</param>
    protected virtual void OnRemoved(KeyValuePair<TKey, TValue> item) { }

    /// <summary>Fires the 'Clearing' event</summary>
    protected virtual void OnClearing() { }

    /// <summary>Fires the 'Cleared' event</summary>
    protected virtual void OnCleared() { }

    /// <summary>The wrapped Dictionary under its type-safe interface</summary>
    private IDictionary<TKey, ICollection<TValue>> typedDictionary;
    /// <summary>The wrapped Dictionary under its object interface</summary>
    private IDictionary objectDictionary;
    /// <summary>The number of items currently in the multi dictionary</summary>
    private int count;
    /// <summary>Provides the values stores in the dictionary in sequence</summary>
    private ValueCollection valueCollection;

  }

} // namespace Nuclex.Support.Collections
