#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2011 Nuclex Development Labs

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

#if UNITTEST

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

using NUnit.Framework;
using NMock;

namespace Nuclex.Input.Devices {

  /// <summary>Unit tests for the queue with leaky encapsulation</summary>
  [TestFixture]
  internal class LeakyQueueTest {

    /// <summary>
    ///   Verifies that the constructor of the leaky queue is working
    /// </summary>
    [Test]
    public void DefaultConstructorCanBeUsed() {
      var queue = new LeakyQueue<int>();
      Assert.IsNotNull(queue); // nonsense, avoids compiler warning
    }

    /// <summary>Verifies that the constructor accepting a capacity is working</summary>
    [Test]
    public void CapacityCanBePassedToConstructor() {
      var queue = new LeakyQueue<int>(123);
      int[] oldArray = queue.Items;
      for (int index = 0; index < 123; ++index) {
        queue.Enqueue(index);
      }
      Assert.AreSame(oldArray, queue.Items);
    }

    /// <summary>
    ///   Ensures that an exception is thrown if a leaky queue is constructed with
    ///   an invalid capacity
    /// </summary>
    [Test]
    public void InvalidCapacityInConstructorThrowsException() {
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { new LeakyQueue<int>(-1); }
      );
    }

    /// <summary>Verifies that the leaky queue can be cleared</summary>
    [Test]
    public void QueueBecomesEmptyAfterCallingClear() {
      var queue = new LeakyQueue<int>();
      for (int index = 0; index < 14; ++index) {
        queue.Enqueue(index);
      }

      Assert.AreEqual(14, queue.Count);
      queue.Clear();
      Assert.AreEqual(0, queue.Count);
    }

    /// <summary>Verifies that the count property is working as expected</summary>
    [Test]
    public void CountPropertyMatchesNumberOfContainedItems() {
      var queue = new LeakyQueue<int>();
      for (int index = 0; index < 42; ++index) {
        queue.Enqueue(index);

        Assert.AreEqual(index + 1, queue.Count);
      }
    }

    /// <summary>Verifies that the queue performs normal FIFO operations</summary>
    [Test]
    public void QueueReturnsItemsInFifoOrder() {
      var queue = new LeakyQueue<int>();

      for (int index = 0; index < 15; ++index) {
        queue.Enqueue(index);
      }
      for (int index = 0; index < 15; ++index) {
        Assert.AreEqual(index, queue.Dequeue());
      }
    }

    /// <summary>
    ///   Verifies that the Contains() method can detect if an item is in the queue
    /// </summary>
    [Test]
    public void ContainsMethodFindsSearchedItem() {
      var queue = new LeakyQueue<int>();

      Assert.IsFalse(queue.Contains(109));

      for (int index = 0; index < 12; ++index) {
        queue.Enqueue(index);
      }

      Assert.IsFalse(queue.Contains(109));

      queue.Enqueue(109);

      Assert.IsTrue(queue.Contains(109));

      for (int index = 0; index < 13; ++index) {
        queue.Enqueue(index);
      }

      Assert.IsTrue(queue.Contains(109));
    }

    /// <summary>
    ///   Verifies that the Contains() method also works on null items
    /// </summary>
    [Test]
    public void ContainsMethodFindsNullItems() {
      var queue = new LeakyQueue<object>();

      Assert.IsFalse(queue.Contains(109));

      for (int index = 0; index < 12; ++index) {
        queue.Enqueue(new object());
      }

      Assert.IsFalse(queue.Contains(null));

      queue.Enqueue(null);

      Assert.IsTrue(queue.Contains(null));

      for (int index = 0; index < 13; ++index) {
        queue.Enqueue(new object());
      }

      Assert.IsTrue(queue.Contains(null));
    }

    /// <summary>
    ///   Verifies that an exception is thrown if you attempt to dequeue from an empty queue
    /// </summary>
    [Test]
    public void DequeuingFromEmptyQueueCausesException() {
      var queue = new LeakyQueue<int>();

      Assert.Throws<InvalidOperationException>(
        delegate() { queue.Dequeue(); }
      );
    }

    /// <summary>
    ///   Verifies that an exception is thrown if you attempt to advance the head of
    ///   an empty queue
    /// </summary>
    [Test]
    public void AdvancingHeadOfEmptyQueueCausesException() {
      var queue = new LeakyQueue<int>();

      Assert.Throws<InvalidOperationException>(
        delegate() { queue.AdvanceHead(); }
      );
    }

    /// <summary>
    ///   Verifies that an queue is able to ensure that there's space for one more
    ///   item in it
    /// </summary>
    [Test]
    public void QueueCanEnsureCapacityForOneMoreSlot() {
      var queue = new LeakyQueue<int>();

      // Make sure the queue is filled to capacity. It is a valid implementation
      // to increase capacity in advance, so we just take the initial capacity
      // and make sure to fill up to that. The queue may have increased capacity already.
      int capacity = queue.Items.Length;
      while (queue.Count < capacity) {
        queue.Enqueue(123);
      }

      queue.EnsureSlotAvailable();

      Assert.Greater(queue.Items.Length, queue.Count);
    }

    /// <summary>
    ///   Verifies that an exception is thrown if the user attempts to Peek() into
    ///   an empty queue
    /// </summary>
    [Test]
    public void PeekInEmptyQueueThrowsException() {
      var queue = new LeakyQueue<int>();

      Assert.Throws<InvalidOperationException>(
        delegate() { queue.Peek(); }
      );
    }

    /// <summary>
    ///   Verifies that Peek() only looks at the oldest item in the queue but doesn't
    ///   dequeue it
    /// </summary>
    [Test]
    public void PeekDoesNotDequeueItem() {
      var queue = new LeakyQueue<int>();
      queue.Enqueue(12);
      queue.Enqueue(34);

      Assert.AreEqual(12, queue.Peek());
      Assert.AreEqual(12, queue.Peek());
    }

    /// <summary>
    ///   Verifies that the queue's head can be advanced
    /// </summary>
    [Test]
    public void QueueHeadCanBeAdvanced() {
      var queue = new LeakyQueue<int>();
      queue.Enqueue(12);
      queue.Enqueue(34);

      Assert.AreEqual(12, queue.Peek());
      queue.AdvanceHead();
      Assert.AreEqual(34, queue.Peek());
    }

    /// <summary>
    ///   Verifies that an exception is thrown if the user attempts to advance
    ///   the head of an empty queue
    /// </summary>
    [Test]
    public void AdvancingHeadOnEmptyQueueCausesException() {
      var queue = new LeakyQueue<int>();

      Assert.Throws<InvalidOperationException>(
        delegate() { queue.AdvanceHead(); }
      );
    }

    /// <summary>
    ///   Verifies that an queue's tail can be advanced without actually putting
    ///   items in the queue (required if enqueuing entails merely changing the state
    ///   of an item slot in the queue.
    /// </summary>
    [Test]
    public void TailCanBeAdvancedWithoutQueueing() {
      var queue = new LeakyQueue<int>();

      for (int index = 0; index < 13; ++index) {
        queue.EnsureSlotAvailable();
        queue.AdvanceTail();

        Assert.AreEqual(index + 1, queue.Count);
      }
    }

    /// <summary>
    ///   Tests whether the head and tail indices in the queue can be retrieved
    /// </summary>
    [Test]
    public void HeadAndTailIndexCanBeQueried() {
      var queue = new LeakyQueue<int>();

      for (int run = 0; run < 4; ++run) {
        for (int index = 0; index < 16; ++index) {
          queue.Enqueue(index);
        }
        for (int index = 0; index < 8; ++index) {
          queue.Dequeue();
        }
      }

      // We can't make any assumptions about how the queue works, thus:
      Assert.GreaterOrEqual(queue.HeadIndex, 0);
      Assert.Less(queue.HeadIndex, queue.Items.Length);
      Assert.GreaterOrEqual(queue.TailIndex, 0);
      Assert.Less(queue.TailIndex, queue.Items.Length);
    }

    /// <summary>
    ///   Verifies that a queue can be turned into an array
    /// </summary>
    [Test]
    public void QueueCanBeConvertedIntoArray() {
      var queue = new LeakyQueue<int>();

      for (int index = 0; index < 4; ++index) {
        queue.Enqueue(index);
      }

      Assert.AreEqual(
        new int[] { 0, 1, 2, 3 },
        queue.ToArray()
      );
    }

    /// <summary>
    ///   Verifies that a queue that is wrapped can be turned into an array
    /// </summary>
    [Test]
    public void InternallyWrappedQueueCanBeConvertedIntoArray() {
      var queue = new LeakyQueue<int>();

      for (int index = 0; index < 4; ++index) {
        queue.Enqueue(index);
      }
      queue.Dequeue();
      queue.Dequeue();
      for (int index = 4; index < 6; ++index) {
        queue.Enqueue(index);
      }

      Assert.AreEqual(
        new int[] { 2, 3, 4, 5 },
        queue.ToArray()
      );
    }

  }

} // namespace Nuclex.Input.Devices

#endif // UNITTEST
