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

  /// <summary>Queue that dequeues items in order of their priority</summary>
  /// <remarks>
  ///   This variant of the priority queue uses an external priority value. If the
  ///   priority data type implements the IComparable interface, the user does not
  ///   even
  /// </remarks>
  public class PairPriorityQueue<PriorityType, ItemType>
    : ICollection, IEnumerable<PriorityItemPair<PriorityType, ItemType>> {
    
    #region class PairComparer

    /// <summary>Compares two priority queue entries based on their priority</summary>
    private class PairComparer : IComparer<PriorityItemPair<PriorityType, ItemType>> {

      /// <summary>Initializes a new entry comparer</summary>
      /// <param name="priorityComparer">Comparer used to compare entry priorities</param>
      public PairComparer(IComparer<PriorityType> priorityComparer) {
        this.priorityComparer = priorityComparer;
      }

      /// <summary>Compares the left entry to the right entry</summary>
      /// <param name="left">Entry on the left side</param>
      /// <param name="right">Entry on the right side</param>
      /// <returns>The relationship of the two entries</returns>
      public int Compare(
        PriorityItemPair<PriorityType, ItemType> left,
        PriorityItemPair<PriorityType, ItemType> right
      ) {
        return this.priorityComparer.Compare(left.Priority, right.Priority);
      }

      /// <summary>Comparer used to compare the priorities of the entries</summary>
      private IComparer<PriorityType> priorityComparer;

    }

    #endregion // class EntryComparer

    /// <summary>Initializes a new non-intrusive priority queue</summary>
    public PairPriorityQueue() : this(Comparer<PriorityType>.Default) { }

    /// <summary>Initializes a new non-intrusive priority queue</summary>
    /// <param name="priorityComparer">Comparer used to compare the item priorities</param>
    public PairPriorityQueue(IComparer<PriorityType> priorityComparer) {
      this.internalQueue = new PriorityQueue<PriorityItemPair<PriorityType, ItemType>>(
        new PairComparer(priorityComparer)
      );
    }

    /// <summary>Returns the topmost item in the queue without dequeueing it</summary>
    /// <returns>The topmost item in the queue</returns>
    public PriorityItemPair<PriorityType, ItemType> Peek() {
      return this.internalQueue.Peek();
    }

    /// <summary>Takes the item with the highest priority off from the queue</summary>
    /// <returns>The item with the highest priority in the list</returns>
    public PriorityItemPair<PriorityType, ItemType> Dequeue() {
      return this.internalQueue.Dequeue();
    }

    /// <summary>Puts an item into the priority queue</summary>
    /// <param name="priority">Priority of the item to be queued</param>
    /// <param name="item">Item to be queued</param>
    public void Enqueue(PriorityType priority, ItemType item) {
      this.internalQueue.Enqueue(
        new PriorityItemPair<PriorityType, ItemType>(priority, item)
      );
    }

    /// <summary>Removes all items from the priority queue</summary>
    public void Clear() {
      this.internalQueue.Clear();
    }

    /// <summary>Total number of items in the priority queue</summary>
    public int Count {
      get { return this.internalQueue.Count; }
    }

    /// <summary>Copies the contents of the priority queue into an array</summary>
    /// <param name="array">Array to copy the priority queue into</param>
    /// <param name="index">Starting index for the destination array</param>
    public void CopyTo(Array array, int index) {
      this.internalQueue.CopyTo(array, index);
    }

    /// <summary>
    ///   Obtains an object that can be used to synchronize accesses to the priority queue
    ///   from different threads
    /// </summary>
    public object SyncRoot {
      get { return this.internalQueue.SyncRoot; }
    }

    /// <summary>Whether operations performed on this priority queue are thread safe</summary>
    public bool IsSynchronized {
      get { return this.internalQueue.IsSynchronized; }
    }

    /// <summary>Returns a typesafe enumerator for the priority queue</summary>
    /// <returns>A new enumerator for the priority queue</returns>
    public IEnumerator<PriorityItemPair<PriorityType, ItemType>> GetEnumerator() {
      return this.internalQueue.GetEnumerator();
    }

    /// <summary>Returns an enumerator for the priority queue</summary>
    /// <returns>A new enumerator for the priority queue</returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return this.internalQueue.GetEnumerator();
    }

    /// <summary>Intrusive priority queue being wrapped by this class</summary>
    private PriorityQueue<PriorityItemPair<PriorityType, ItemType>> internalQueue;

  }

} // namespace Nuclex.Support.Collections
