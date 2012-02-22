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

  /// <summary>Collection that transforms the contents of another collection.</summary>
  /// <typeparam name="ContainedItemType">
  ///   Type of the items contained in the wrapped collection.
  /// </typeparam>
  /// <typeparam name="ExposedItemType">
  ///   Type this collection exposes its items as.
  /// </typeparam>
  /// <remarks>
  ///   <para>
  ///     This collection is useful if you want to expose the objects of an arbitrary
  ///     collection under a different type. It can be used, for example, to construct
  ///     wrappers for the items in a collection on-the-fly, eliminating the need to
  ///     manage the wrappers in parallel to the real items and improving performance
  ///     by only constructing a wrapper when an item is actually requested.
  ///   </para>
  ///   <para>
  ///     Another common use would be if you have a private collection of a non-public
  ///     type that's derived from some publicly visible type. By using this collection,
  ///     you can return the items under the publicly visible type while still having
  ///     your private collection under the non-public type, eliminating the need to
  ///     downcast each time you need to access elements of the non-public type.
  ///   </para>
  /// </remarks>
  public abstract partial class TransformingReadOnlyCollection<
    ContainedItemType, ExposedItemType
  > : IList<ExposedItemType>, IList {

    #region class TransformingEnumerator

    /// <summary>
    ///   An enumerator that transforms the items returned by an enumerator of the
    ///   wrapped collection into the exposed type on-the-fly.
    /// </summary>
    private class TransformingEnumerator : IEnumerator<ExposedItemType> {

      /// <summary>Initializes a new transforming enumerator</summary>
      /// <param name="transformer">Owner; used to invoke the Transform() method</param>
      /// <param name="containedTypeEnumerator">Enumerator of the wrapped collection</param>
      public TransformingEnumerator(
        TransformingReadOnlyCollection<ContainedItemType, ExposedItemType> transformer,
        IEnumerator<ContainedItemType> containedTypeEnumerator
      ) {
        this.transformer = transformer;
        this.containedTypeEnumerator = containedTypeEnumerator;
      }

      /// <summary>Immediately releases all resources used by the instance</summary>
      public void Dispose() {
        this.containedTypeEnumerator.Dispose();
      }

      /// <summary>
      ///   The element in the collection at the current position of the enumerator.
      /// </summary>
      public ExposedItemType Current {
        get {
          return this.transformer.Transform(this.containedTypeEnumerator.Current);
        }
      }

      /// <summary>Gets the current element in the collection.</summary>
      /// <returns>The current element in the collection.</returns>
      /// <exception cref="System.InvalidOperationException">
      ///   The enumerator is positioned before the first element of the collection
      ///   or after the last element.
      /// </exception>
      public bool MoveNext() {
        return this.containedTypeEnumerator.MoveNext();
      }

      /// <summary>
      ///   Sets the enumerator to its initial position, which is before the first element
      ///   in the collection.
      /// </summary>
      /// <exception cref="System.InvalidOperationException">
      ///   The collection was modified after the enumerator was created.
      /// </exception>
      public void Reset() {
        this.containedTypeEnumerator.Reset();
      }

      /// <summary>The current element in the collection.</summary>
      /// <exception cref="System.InvalidOperationException">
      ///   The enumerator is positioned before the first element of the collection
      ///   or after the last element.
      /// </exception>
      object IEnumerator.Current {
        get { return Current; }
      }

      /// <summary>
      ///   Collection that owns this enumerator; required to invoke the item
      ///   transformation method.
      /// </summary>
      private TransformingReadOnlyCollection<ContainedItemType, ExposedItemType> transformer;
      /// <summary>An enumerator from the wrapped collection</summary>
      private IEnumerator<ContainedItemType> containedTypeEnumerator;

    }

    #endregion // class TransformingEnumerator

    /// <summary>Initializes a new transforming collection wrapper</summary>
    /// <param name="items">
    ///   Internal list of items that are transformed into the exposed type when
    ///   accessed through the TransformingReadOnlyCollection.
    /// </param>
    public TransformingReadOnlyCollection(IList<ContainedItemType> items) {
      this.items = items;
    }

    /// <summary>
    ///   Determines whether an element is in the TransformingReadOnlyCollection
    /// </summary>
    /// <param name="item">
    ///   The object to locate in the TransformingReadOnlyCollection.
    ///   The value can be null for reference types.
    /// </param>
    /// <returns>
    ///   True if value is found in the TransformingReadOnlyCollection; otherwise, false.
    /// </returns>
    /// <remarks>
    ///   The default implementation of this method is very unoptimized and will
    ///   enumerate all the items in the collection, transforming one after another
    ///   to check whether the transformed item matches the item the user was
    ///   looking for. It is recommended to provide a custom implementation of
    ///   this method, if possible.
    /// </remarks>
    public virtual bool Contains(ExposedItemType item) {
      return (IndexOf(item) != -1);
    }

    /// <summary>
    ///   Copies the entire TransformingReadOnlyCollection to a compatible one-dimensional
    ///   System.Array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array">
    ///   The one-dimensional System.Array that is the destination of the elements copied
    ///   from the TransformingReadOnlyCollection. The System.Array must have
    ///   zero-based indexing.
    /// </param>
    /// <param name="index">
    ///   The zero-based index in array at which copying begins.
    /// </param>
    /// <exception cref="System.ArgumentException">
    ///   Index is equal to or greater than the length of array or the number of elements
    ///   in the source TransformingReadOnlyCollection is greater than the available space
    ///   from index to the end of the destination array.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Index is less than zero.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///   Array is null.
    /// </exception>
    public void CopyTo(ExposedItemType[] array, int index) {
      if(this.items.Count > (array.Length - index)) {
        throw new ArgumentException(
          "Array too small to fit the collection items starting at the specified index"
        );
      }

      for(int itemIndex = 0; itemIndex < this.items.Count; ++itemIndex) {
        array[itemIndex + index] = Transform(this.items[itemIndex]);
      }
    }

    /// <summary>
    ///   Returns an enumerator that iterates through the TransformingReadOnlyCollection.
    /// </summary>
    /// <returns>
    ///   An enumerator or the TransformingReadOnlyCollection.
    /// </returns>
    public IEnumerator<ExposedItemType> GetEnumerator() {
      return new TransformingEnumerator(this, this.items.GetEnumerator());
    }

    /// <summary>
    ///   Searches for the specified object and returns the zero-based index of the
    ///   first occurrence within the entire TransformingReadOnlyCollection.
    /// </summary>
    /// <param name="item">
    ///   The object to locate in the TransformingReadOnlyCollection. The value can
    ///   be null for reference types.
    /// </param>
    /// <returns>
    ///   The zero-based index of the first occurrence of item within the entire
    ///   TransformingReadOnlyCollection, if found; otherwise, -1.
    /// </returns>
    /// <remarks>
    ///   The default implementation of this method is very unoptimized and will
    ///   enumerate all the items in the collection, transforming one after another
    ///   to check whether the transformed item matches the item the user was
    ///   looking for. It is recommended to provide a custom implementation of
    ///   this method, if possible.
    /// </remarks>
    public int IndexOf(ExposedItemType item) {

      if(item == null) {

        for(int index = 0; index < this.items.Count; ++index) {
          if(Transform(this.items[index]) == null) {
            return index;
          }
        }

      } else {

        EqualityComparer<ExposedItemType> comparer =
          EqualityComparer<ExposedItemType>.Default;

        for(int index = 0; index < this.items.Count; ++index) {
          if(comparer.Equals(Transform(this.items[index]), item)) {
            return index;
          }
        }

      }

      return -1;

    }

    /// <summary>
    ///   The number of elements contained in the TransformingReadOnlyCollection instance
    /// </summary>
    public int Count {
      get { return this.items.Count; }
    }

    /// <summary>Gets the element at the specified index.</summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///    Index is less than zero or index is equal to or greater than
    ///    TransformingReadOnlyCollection.Count.
    /// </exception>
    public ExposedItemType this[int index] {
      get { return Transform(this.items[index]); }
    }

    /// <summary>Whether the List is write-protected</summary>
    public bool IsReadOnly {
      get { return true; }
    }

    /// <summary>Transforms an item into the exposed type</summary>
    /// <param name="item">Item to be transformed</param>
    /// <returns>The transformed item</returns>
    /// <remarks>
    ///   This method is used to transform an item in the wrapped collection into
    ///   the exposed item type whenever the user accesses an item. Expect it to
    ///   be called frequently, because the TransformingReadOnlyCollection does
    ///   not cache or otherwise store the transformed items.
    /// </remarks>
    protected abstract ExposedItemType Transform(ContainedItemType item);

    /// <summary>Items being transformed upon exposure by this collection</summary>
    private IList<ContainedItemType> items;
    /// <summary>Synchronization root for threaded accesses to this collection</summary>
    private object syncRoot;

  }

} // namespace Nuclex.Support.Collections
