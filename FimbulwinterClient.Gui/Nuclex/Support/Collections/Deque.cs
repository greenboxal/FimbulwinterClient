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

namespace Nuclex.Support.Collections {

  /// <summary>A double-ended queue that allocates memory in blocks</summary>
  /// <typeparam name="ItemType">Type of the items being stored in the queue</typeparam>
  /// <remarks>
  ///   <para>
  ///     The double-ended queue allows items to be appended to either side of the queue
  ///     without a hefty toll on performance. Like its namesake in C++, it is implemented
  ///     using multiple arrays.
  ///   </para>
  ///   <para>
  ///     Therefore, it's not only good at coping with lists that are modified at their
  ///     beginning, but also at handling huge data sets since enlarging the deque doesn't
  ///     require items to be copied around and it still can be accessed by index.
  ///   </para>
  /// </remarks>
  public partial class Deque<ItemType> : IList<ItemType>, IList {

    #region class Enumerator

    /// <summary>Enumerates over the items in a deque</summary>
    private class Enumerator : IEnumerator<ItemType>, IEnumerator {

      /// <summary>Initializes a new deque enumerator</summary>
      /// <param name="deque">Deque whose items will be enumerated</param>
      public Enumerator(Deque<ItemType> deque) {
        this.deque = deque;
        this.blockSize = this.deque.blockSize;
        this.lastBlock = this.deque.blocks.Count - 1;
        this.lastBlockEndIndex = this.deque.lastBlockEndIndex - 1;

        Reset();
      }

      /// <summary>Immediately releases all resources owned by the instance</summary>
      public void Dispose() {
        this.deque = null;
        this.currentBlock = null;
      }

      /// <summary>The item at the enumerator's current position</summary>
      public ItemType Current {
        get {
#if DEBUG
          checkVersion();
#endif
          if (this.currentBlock == null) {
            throw new InvalidOperationException("Enumerator is not on a valid position");
          }

          return this.currentBlock[this.subIndex];
        }
      }

      /// <summary>Advances the enumerator to the next item</summary>
      /// <returns>True if there was a next item</returns>
      public bool MoveNext() {

#if DEBUG
        checkVersion();
#endif

        // If we haven't reached the last block yet      
        if (this.currentBlockIndex < this.lastBlock) {

          // Advance to the next item. If the end of the current block is reached,
          // go to the next block's first item
          ++this.subIndex;
          if (this.subIndex >= this.blockSize) {
            ++this.currentBlockIndex;
            this.currentBlock = this.deque.blocks[this.currentBlockIndex];
            if (this.currentBlockIndex == 0) {
              this.subIndex = this.deque.firstBlockStartIndex;
            } else {
              this.subIndex = 0;
            }
          }

          // Item found. If the current block wasn't the last block, an item *has*
          // to follow since otherwise, no further blocks would exist!
          return true;

        } else { // We in or beyond the last block

          // Are there any items left to advance to?
          if (this.subIndex < this.lastBlockEndIndex) {
            ++this.subIndex;
            return true;
          } else { // Nope, we've reached the end of the deque
            this.currentBlock = null;
            return false;
          }

        }

      }

      /// <summary>Resets the enumerator to its initial position</summary>
      public void Reset() {
        this.currentBlock = null;
        this.currentBlockIndex = -1;
        this.subIndex = this.deque.blockSize - 1;
#if DEBUG
        this.expectedVersion = this.deque.version;
#endif
      }

      /// <summary>The item at the enumerator's current position</summary>
      object IEnumerator.Current {
        get { return Current; }
      }

#if DEBUG
      /// <summary>Ensures that the deque has not changed</summary>
      private void checkVersion() {
        if(this.expectedVersion != this.deque.version)
          throw new InvalidOperationException("Deque has been modified");
      }
#endif

      /// <summary>Deque the enumerator belongs to</summary>
      private Deque<ItemType> deque;
      /// <summary>Size of the blocks in the deque</summary>
      private int blockSize;
      /// <summary>Index of the last block in the deque</summary>
      private int lastBlock;
      /// <summary>End index of the items in the deque's last block</summary>
      private int lastBlockEndIndex;

      /// <summary>Index of the block the enumerator currently is in</summary>
      private int currentBlockIndex;
      /// <summary>Reference to the block being enumerated</summary>
      private ItemType[] currentBlock;
      /// <summary>Index in the current block</summary>
      private int subIndex;
#if DEBUG
      /// <summary>Version the deque is expected to have</summary>
      private int expectedVersion;
#endif
    }

    #endregion // class Enumerator

    /// <summary>Initializes a new deque</summary>
    public Deque() : this(512) { }

    /// <summary>Initializes a new deque using the specified block size</summary>
    /// <param name="blockSize">Size of the individual memory blocks used</param>
    public Deque(int blockSize) {
      this.blockSize = blockSize;

      this.blocks = new List<ItemType[]>();
      this.blocks.Add(new ItemType[this.blockSize]);
    }

    /// <summary>Number of items contained in the double ended queue</summary>
    public int Count {
      get { return this.count; }
    }

    /// <summary>Accesses an item by its index</summary>
    /// <param name="index">Index of the item that will be accessed</param>
    /// <returns>The item at the specified index</returns>
    public ItemType this[int index] {
      get {
        int blockIndex, subIndex;
        findIndex(index, out blockIndex, out subIndex);

        return this.blocks[blockIndex][subIndex];
      }
      set {
        int blockIndex, subIndex;
        findIndex(index, out blockIndex, out subIndex);

        this.blocks[blockIndex][subIndex] = value;
      }
    }

    /// <summary>The first item in the double-ended queue</summary>
    public ItemType First {
      get {
        if (this.count == 0) {
          throw new InvalidOperationException("The deque is empty");
        }
        return this.blocks[0][this.firstBlockStartIndex];
      }
    }

    /// <summary>The last item in the double-ended queue</summary>
    public ItemType Last {
      get {
        if (this.count == 0) {
          throw new InvalidOperationException("The deque is empty");
        }
        return this.blocks[this.blocks.Count - 1][this.lastBlockEndIndex - 1];
      }
    }

    /// <summary>Determines whether the deque contains the specified item</summary>
    /// <param name="item">Item the deque will be scanned for</param>
    /// <returns>True if the deque contains the item, false otherwise</returns>
    public bool Contains(ItemType item) {
      return (IndexOf(item) != -1);
    }

    /// <summary>Copies the contents of the deque into an array</summary>
    /// <param name="array">Array the contents of the deque will be copied into</param>
    /// <param name="arrayIndex">Array index the deque contents will begin at</param>
    public void CopyTo(ItemType[] array, int arrayIndex) {
      if (this.count > (array.Length - arrayIndex)) {
        throw new ArgumentException(
          "Array too small to hold the collection items starting at the specified index"
        );
      }

      if (this.blocks.Count == 1) { // Does only one block exist?

        // Copy the one and only block there is
        Array.Copy(
          this.blocks[0], this.firstBlockStartIndex,
          array, arrayIndex,
          this.lastBlockEndIndex - this.firstBlockStartIndex
        );

      } else { // Multiple blocks exist

        // Copy the first block which is filled from the start index to its end
        int length = this.blockSize - this.firstBlockStartIndex;
        Array.Copy(
          this.blocks[0], this.firstBlockStartIndex,
          array, arrayIndex,
          length
        );
        arrayIndex += length;

        // Copy all intermediate blocks (if there are any). These are completely filled
        int lastBlock = this.blocks.Count - 1;
        for (int index = 1; index < lastBlock; ++index) {
          Array.Copy(
            this.blocks[index], 0,
            array, arrayIndex,
            this.blockSize
          );
          arrayIndex += this.blockSize;
        }

        // Copy the final block which is filled from the beginning to the end index
        Array.Copy(
          this.blocks[lastBlock], 0,
          array, arrayIndex,
          this.lastBlockEndIndex
        );

      }
    }

    /// <summary>Obtains a new enumerator for the contents of the deque</summary>
    /// <returns>The new enumerator</returns>
    public IEnumerator<ItemType> GetEnumerator() {
      return new Enumerator(this);
    }

    /// <summary>Calculates the block index and local sub index of an entry</summary>
    /// <param name="index">Index of the entry that will be located</param>
    /// <param name="blockIndex">Index of the block the entry is contained in</param>
    /// <param name="subIndex">Local sub index of the entry within the block</param>
    private void findIndex(int index, out int blockIndex, out int subIndex) {
      if ((index < 0) || (index >= this.count)) {
        throw new ArgumentOutOfRangeException("Index out of range", "index");
      }

      index += this.firstBlockStartIndex;
#if WINDOWS
      blockIndex = Math.DivRem(index, this.blockSize, out subIndex);
#else
      blockIndex = index / this.blockSize;
      subIndex = index % this.blockSize;
#endif
    }

    /// <summary>
    ///   Determines whether the provided object can be placed in the deque
    /// </summary>
    /// <param name="value">Value that will be checked for compatibility</param>
    /// <returns>True if the value can be placed in the deque</returns>
    private static bool isCompatibleObject(object value) {
      return ((value is ItemType) || ((value == null) && !typeof(ItemType).IsValueType));
    }

    /// <summary>Verifies that the provided object matches the deque's type</summary>
    /// <param name="value">Value that will be checked for compatibility</param>
    private static void verifyCompatibleObject(object value) {
      if (!isCompatibleObject(value)) {
        throw new ArgumentException("Value does not match the deque's type", "value");
      }
    }

    /// <summary>Number if items currently stored in the deque</summary>
    private int count;
    /// <summary>Size of a single deque block</summary>
    private int blockSize;
    /// <summary>Memory blocks being used to store the deque's data</summary>
    private List<ItemType[]> blocks;
    /// <summary>Starting index of data in the first block</summary>
    private int firstBlockStartIndex;
    /// <summary>End index of data in the last block</summary>
    private int lastBlockEndIndex;
#if DEBUG
    /// <summary>Used to detect when enumerators go out of sync</summary>
    private int version;
#endif

  }

} // namespace Nuclex.Support.Collections
