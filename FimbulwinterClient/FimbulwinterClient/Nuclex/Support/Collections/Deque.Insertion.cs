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

  partial class Deque<ItemType> {

    /// <summary>Inserts an item at the beginning of the double-ended queue</summary>
    /// <param name="item">Item that will be inserted into the queue</param>
    public void AddFirst(ItemType item) {
      if(this.firstBlockStartIndex > 0) {
        --this.firstBlockStartIndex;
      } else { // Need to allocate a new block
        this.blocks.Insert(0, new ItemType[this.blockSize]);
        this.firstBlockStartIndex = this.blockSize - 1;
      }

      this.blocks[0][this.firstBlockStartIndex] = item;
      ++this.count;
#if DEBUG
      ++this.version;
#endif      
    }

    /// <summary>Appends an item to the end of the double-ended queue</summary>
    /// <param name="item">Item that will be appended to the queue</param>
    public void AddLast(ItemType item) {
      if(this.lastBlockEndIndex < this.blockSize) {
        ++this.lastBlockEndIndex;
      } else { // Need to allocate a new block
        this.blocks.Add(new ItemType[this.blockSize]);
        this.lastBlockEndIndex = 1;
      }

      this.blocks[this.blocks.Count - 1][this.lastBlockEndIndex - 1] = item;
      ++this.count;
#if DEBUG
      ++this.version;
#endif
    }

    /// <summary>Inserts the item at the specified index</summary>
    /// <param name="index">Index the item will be inserted at</param>
    /// <param name="item">Item that will be inserted</param>
    public void Insert(int index, ItemType item) {
      int distanceToRightEnd = this.count - index;
      if(index < distanceToRightEnd) { // Are we closer to the left end?
        shiftLeftAndInsert(index, item);
      } else { // Nope, we're closer to the right end
        shiftRightAndInsert(index, item);
      }
#if DEBUG
      ++this.version;
#endif
    }

    /// <summary>
    ///   Shifts all items before the insertion point to the left and inserts
    ///   the item at the specified index
    /// </summary>
    /// <param name="index">Index the item will be inserted at</param>
    /// <param name="item">Item that will be inserted</param>
    private void shiftLeftAndInsert(int index, ItemType item) {
      if(index == 0) {
        AddFirst(item);
      } else {
        int blockIndex, subIndex;
        findIndex(index, out blockIndex, out subIndex);

        int firstBlock = 0;
        int blockStart;

        // If the first block is full, we need to add another block
        if(this.firstBlockStartIndex == 0) {
          this.blocks.Insert(0, new ItemType[this.blockSize]);
          this.blocks[0][this.blockSize - 1] = this.blocks[1][0];
          this.firstBlockStartIndex = this.blockSize - 1;

          blockStart = 1;
          --subIndex;
          if(subIndex < 0) {
            subIndex = this.blockSize - 1;
          } else {
            ++blockIndex;
          }
          ++firstBlock;
        } else {
          blockStart = this.firstBlockStartIndex;
          --this.firstBlockStartIndex;

          --subIndex;
          if(subIndex < 0) {
            subIndex = this.blockSize - 1;
            --blockIndex;
          }
        }

        // If the insertion point is not in the first block
        if(blockIndex != firstBlock) {
          Array.Copy(
            this.blocks[firstBlock], blockStart,
            this.blocks[firstBlock], blockStart - 1,
            this.blockSize - blockStart
          );
          this.blocks[firstBlock][this.blockSize - 1] = this.blocks[firstBlock + 1][0];

          // Move all the blocks following the insertion point to the right by one item.
          // If there are no blocks inbetween, this for loop will not run.
          for(int tempIndex = firstBlock + 1; tempIndex < blockIndex; ++tempIndex) {
            Array.Copy(
              this.blocks[tempIndex], 1, this.blocks[tempIndex], 0, this.blockSize - 1
            );
            this.blocks[tempIndex][this.blockSize - 1] = this.blocks[tempIndex + 1][0];
          }

          blockStart = 1;
        }

        // Finally, move the items in the block the insertion takes place in
        Array.Copy(
          this.blocks[blockIndex], blockStart,
          this.blocks[blockIndex], blockStart - 1,
          subIndex - blockStart + 1
        );

        this.blocks[blockIndex][subIndex] = item;
        ++this.count;
      }
    }

    /// <summary>
    ///   Shifts all items after the insertion point to the right and inserts
    ///   the item at the specified index
    /// </summary>
    /// <param name="index">Index the item will be inserted at</param>
    /// <param name="item">Item that will be inserted</param>
    private void shiftRightAndInsert(int index, ItemType item) {
      if(index == this.count) {
        AddLast(item);
      } else {
        int blockIndex, subIndex;
        findIndex(index, out blockIndex, out subIndex);

        int lastBlock = this.blocks.Count - 1;
        int blockLength;

        // If the lastmost block is full, we need to add another block
        if(this.lastBlockEndIndex == this.blockSize) {
          this.blocks.Add(new ItemType[this.blockSize]);
          this.blocks[lastBlock + 1][0] = this.blocks[lastBlock][this.blockSize - 1];
          this.lastBlockEndIndex = 1;

          blockLength = this.blockSize - 1;
        } else {
          blockLength = this.lastBlockEndIndex;
          ++this.lastBlockEndIndex;
        }

        // If the insertion point is not in the lastmost block
        if(blockIndex != lastBlock) {
          Array.Copy(
            this.blocks[lastBlock], 0, this.blocks[lastBlock], 1, blockLength
          );
          this.blocks[lastBlock][0] = this.blocks[lastBlock - 1][this.blockSize - 1];

          // Move all the blocks following the insertion point to the right by one item.
          // If there are no blocks inbetween, this for loop will not run.
          for(int tempIndex = lastBlock - 1; tempIndex > blockIndex; --tempIndex) {
            Array.Copy(
              this.blocks[tempIndex], 0, this.blocks[tempIndex], 1, this.blockSize - 1
            );
            this.blocks[tempIndex][0] = this.blocks[tempIndex - 1][this.blockSize - 1];
          }

          blockLength = this.blockSize - 1;
        }

        // Finally, move the items in the block the insertion takes place in
        Array.Copy(
          this.blocks[blockIndex], subIndex,
          this.blocks[blockIndex], subIndex + 1,
          blockLength - subIndex
        );

        this.blocks[blockIndex][subIndex] = item;
        ++this.count;
      }
    }

  }

} // namespace Nuclex.Support.Collections
