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

  /// <summary>
  ///   Associative collection that can store several values under one key and vice versa
  /// </summary>
  /// <typeparam name="TKey">Type of keys used within the dictionary</typeparam>
  /// <typeparam name="TValue">Type of values stored in the dictionary</typeparam>
  public interface IMultiDictionary<TKey, TValue> :
    IDictionary<TKey, ICollection<TValue>>,
    IDictionary,
    ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable {

    /// <summary>Adds a value into the dictionary under the provided key</summary>
    /// <param name="key">Key the value will be stored under</param>
    /// <param name="value">Value that will be stored under the specified key</param>
    void Add(TKey key, TValue value);

    /// <summary>Determines the number of values stored under the specified key</summary>
    /// <param name="key">Key whose values will be counted</param>
    /// <returns>The number of values stored under the specified key</returns>
    int CountValues(TKey key);

    /// <summary>
    ///   Removes the item with the specified key and value from the dictionary
    /// </summary>
    /// <param name="key">Key of the item that will be removed</param>
    /// <param name="value">Value of the item that will be removed</param>
    /// <returns>
    ///   True if the specified item was contained in the dictionary and was removed
    /// </returns>
    /// <exception cref="NotSupportedException">If the dictionary is read-only</exception>
    bool Remove(TKey key, TValue value);

    /// <summary>Removes all items with the specified key from the dictionary</summary>
    /// <param name="key">Key of the item that will be removed</param>
    /// <returns>The number of items that have been removed from the dictionary</returns>
    /// <exception cref="NotSupportedException">If the dictionary is read-only</exception>
    int RemoveKey(TKey key);

  }

} // namespace Nuclex.Support.Collections
