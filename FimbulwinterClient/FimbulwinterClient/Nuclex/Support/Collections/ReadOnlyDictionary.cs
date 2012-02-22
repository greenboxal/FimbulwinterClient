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
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Nuclex.Support.Collections {

  /// <summary>Wraps a dictionary and prevents users from modifying it</summary>
  /// <typeparam name="KeyType">Type of the keys used in the dictionary</typeparam>
  /// <typeparam name="ValueType">Type of the values used in the dictionary</typeparam>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class ReadOnlyDictionary<KeyType, ValueType> :
#if !NO_SERIALIZATION
    ISerializable,
    IDeserializationCallback,
#endif
    IDictionary<KeyType, ValueType>,
    IDictionary {

#if !NO_SERIALIZATION

    #region class SerializedDictionary

    /// <summary>
    ///   Dictionary wrapped used to reconstruct a serialized read only dictionary
    /// </summary>
    private class SerializedDictionary : Dictionary<KeyType, ValueType> {

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
    protected ReadOnlyDictionary(SerializationInfo info, StreamingContext context) :
      this(new SerializedDictionary(info, context)) { }

#endif // !NO_SERIALIZATION

    /// <summary>Initializes a new read-only dictionary wrapper</summary>
    /// <param name="dictionary">Dictionary that will be wrapped</param>
    public ReadOnlyDictionary(IDictionary<KeyType, ValueType> dictionary) {
      this.typedDictionary = dictionary;
      this.objectDictionary = (this.typedDictionary as IDictionary);
    }


    /// <summary>Whether the directory is write-protected</summary>
    public bool IsReadOnly {
      get { return true; }
    }

    /// <summary>
    ///   Determines whether the specified KeyValuePair is contained in the Dictionary
    /// </summary>
    /// <param name="item">KeyValuePair that will be checked for</param>
    /// <returns>True if the provided KeyValuePair was contained in the Dictionary</returns>
    public bool Contains(KeyValuePair<KeyType, ValueType> item) {
      return this.typedDictionary.Contains(item);
    }

    /// <summary>Determines whether the Dictionary contains the specified key</summary>
    /// <param name="key">Key that will be checked for</param>
    /// <returns>
    ///   True if an entry with the specified key was contained in the Dictionary
    /// </returns>
    public bool ContainsKey(KeyType key) {
      return this.typedDictionary.ContainsKey(key);
    }

    /// <summary>Copies the contents of the Dictionary into an array</summary>
    /// <param name="array">Array the Dictionary will be copied into</param>
    /// <param name="arrayIndex">
    ///   Starting index at which to begin filling the destination array
    /// </param>
    public void CopyTo(KeyValuePair<KeyType, ValueType>[] array, int arrayIndex) {
      this.typedDictionary.CopyTo(array, arrayIndex);
    }

    /// <summary>Number of elements contained in the Dictionary</summary>
    public int Count {
      get { return this.typedDictionary.Count; }
    }

    /// <summary>Creates a new enumerator for the Dictionary</summary>
    /// <returns>The new Dictionary enumerator</returns>
    public IEnumerator<KeyValuePair<KeyType, ValueType>> GetEnumerator() {
      return this.typedDictionary.GetEnumerator();
    }

    /// <summary>Collection of all keys contained in the Dictionary</summary>
    public ICollection<KeyType> Keys {
      get {
        if(this.readonlyKeyCollection == null) {
          this.readonlyKeyCollection = new ReadOnlyCollection<KeyType>(
            this.typedDictionary.Keys
          );
        }

        return this.readonlyKeyCollection;
      }
    }

    /// <summary>Collection of all values contained in the Dictionary</summary>
    public ICollection<ValueType> Values {
      get {
        if(this.readonlyValueCollection == null) {
          this.readonlyValueCollection = new ReadOnlyCollection<ValueType>(
            this.typedDictionary.Values
          );
        }

        return this.readonlyValueCollection;
      }
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
    public bool TryGetValue(KeyType key, out ValueType value) {
      return this.typedDictionary.TryGetValue(key, out value);
    }

    /// <summary>Accesses an item in the Dictionary by its key</summary>
    /// <param name="key">Key of the item that will be accessed</param>
    public ValueType this[KeyType key] {
      get { return this.typedDictionary[key]; }
    }

    #region IDictionary<,> implementation

    /// <summary>Inserts an item into the Dictionary</summary>
    /// <param name="key">Key under which to add the new item</param>
    /// <param name="value">Item that will be added to the Dictionary</param>
    void IDictionary<KeyType, ValueType>.Add(KeyType key, ValueType value) {
      throw new NotSupportedException(
        "Adding items is not supported by the read-only Dictionary"
      );
    }

    /// <summary>Removes the item with the specified key from the Dictionary</summary>
    /// <param name="key">Key of the elementes that will be removed</param>
    /// <returns>True if an item with the specified key was found and removed</returns>
    bool IDictionary<KeyType, ValueType>.Remove(KeyType key) {
      throw new NotSupportedException(
        "Removing items is not supported by the read-only Dictionary"
      );
    }

    /// <summary>Accesses an item in the Dictionary by its key</summary>
    /// <param name="key">Key of the item that will be accessed</param>
    ValueType IDictionary<KeyType, ValueType>.this[KeyType key] {
      get { return this.typedDictionary[key]; }
      set {
        throw new NotSupportedException(
          "Assigning items is not supported in a read-only Dictionary"
        );
      }
    }

    #endregion

    #region IEnumerable implementation

    /// <summary>Returns a new object enumerator for the Dictionary</summary>
    /// <returns>The new object enumerator</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return (this.typedDictionary as IEnumerable).GetEnumerator();
    }

    #endregion

    #region IDictionary implementation

    /// <summary>Removes all items from the Dictionary</summary>
    void IDictionary.Clear() {
      throw new NotSupportedException(
        "Clearing is not supported in a read-only Dictionary"
      );
    }

    /// <summary>Adds an item into the Dictionary</summary>
    /// <param name="key">Key under which the item will be added</param>
    /// <param name="value">Item that will be added</param>
    void IDictionary.Add(object key, object value) {
      throw new NotSupportedException(
        "Adding items is not supported in a read-only Dictionary"
      );
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
      get {
        if(this.readonlyKeyCollection == null) {
          this.readonlyKeyCollection = new ReadOnlyCollection<KeyType>(
            this.typedDictionary.Keys
          );
        }

        return this.readonlyKeyCollection;
      }
    }

    /// <summary>Returns a collection of all values stored in the Dictionary</summary>
    ICollection IDictionary.Values {
      get {
        if(this.readonlyValueCollection == null) {
          this.readonlyValueCollection = new ReadOnlyCollection<ValueType>(
            this.typedDictionary.Values
          );
        }

        return this.readonlyValueCollection;
      }
    }

    /// <summary>Removes an item from the Dictionary</summary>
    /// <param name="key">Key of the item that will be removed</param>
    void IDictionary.Remove(object key) {
      throw new NotSupportedException(
        "Removing is not supported by the read-only Dictionary"
      );
    }

    /// <summary>Accesses an item in the Dictionary by its key</summary>
    /// <param name="key">Key of the item that will be accessed</param>
    /// <returns>The item with the specified key</returns>
    object IDictionary.this[object key] {
      get { return this.objectDictionary[key]; }
      set {
        throw new NotSupportedException(
          "Assigning items is not supported by the read-only Dictionary"
        );
      }
    }

    #endregion

    #region ICollection<> implementation

    /// <summary>Inserts an already prepared element into the Dictionary</summary>
    /// <param name="item">Prepared element that will be added to the Dictionary</param>
    void ICollection<KeyValuePair<KeyType, ValueType>>.Add(
      KeyValuePair<KeyType, ValueType> item
    ) {
      throw new NotSupportedException(
        "Adding items is not supported by the read-only Dictionary"
      );
    }

    /// <summary>Removes all items from the Dictionary</summary>
    void ICollection<KeyValuePair<KeyType, ValueType>>.Clear() {
      throw new NotSupportedException(
        "Clearing is not supported in a read-only Dictionary"
      );
    }

    /// <summary>Removes all items from the Dictionary</summary>
    /// <param name="itemToRemove">Item that will be removed from the Dictionary</param>
    bool ICollection<KeyValuePair<KeyType, ValueType>>.Remove(
      KeyValuePair<KeyType, ValueType> itemToRemove
    ) {
      throw new NotSupportedException(
        "Removing items is not supported in a read-only Dictionary"
      );
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
    private IDictionary<KeyType, ValueType> typedDictionary;
    /// <summary>The wrapped Dictionary under its object interface</summary>
    private IDictionary objectDictionary;
    /// <summary>ReadOnly wrapper for the keys collection of the Dictionary</summary>
    private ReadOnlyCollection<KeyType> readonlyKeyCollection;
    /// <summary>ReadOnly wrapper for the values collection of the Dictionary</summary>
    private ReadOnlyCollection<ValueType> readonlyValueCollection;
  }

} // namespace Nuclex.Support.Collections
