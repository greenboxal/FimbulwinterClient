using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuclex.Input.Devices {

  /// <summary>Specialized queue which lets users access its raw data</summary>
  /// <typeparam name="ItemType">Type of items the queue will contain</typeparam>
  /// <remarks>
  ///   No, it doesn't leak memory. It leaks implementation details. :o)
  /// </remarks>
  internal class LeakyQueue<ItemType> {

    /// <summary>Default capacity a new leaky queue will have</summary>
    private const int DefaultCapacity = 4;
    /// <summary>Factor by which the queue grows if its capacity is exceeded</summary>
    private const int GrowFactor = 2;
    /// <summary>Minimum number of items the queue will grow by</summary>
    private const int MinimumGrow = 4;

    /// <summary>Initializes a new leaky queue</summary>
    public LeakyQueue() {
      this.items = LeakyQueue<ItemType>.emptyArray;
    }

    /// <summary>Initializes a new leaky queue with the specified capacity</summary>
    /// <param name="capacity">Initial capacity of the leaky queue</param>
    public LeakyQueue(int capacity) {
      if (capacity < 0) {
        throw new ArgumentOutOfRangeException("Capacity must be 0 or larger");
      }

      this.items = new ItemType[capacity];
      this.headIndex = 0;
      this.tailIndex = 0;
      this.count = 0;
    }

    /// <summary>Removes all items from the queue</summary>
    public void Clear() {
      // We do not clear the removed items, intentionally

      this.headIndex = 0;
      this.tailIndex = 0;
      this.count = 0;
    }

    /// <summary>Tests whether the queue contains the specified item</summary>
    /// <param name="searchedItem">Item the queue will be tested for</param>
    /// <returns>True if the queue contains the specified item</returns>
    public bool Contains(ItemType searchedItem) {
      EqualityComparer<ItemType> comparer = EqualityComparer<ItemType>.Default;

      int index = this.headIndex;
      int remaining = this.count;
      while (remaining-- > 0) {
        if (searchedItem == null) {
          if (this.items[index] == null) {
            return true;
          }
        } else if (
          (this.items[index] != null) &&
          comparer.Equals(this.items[index], searchedItem)
        ) {
          return true;
        }

        index = (index + 1) % this.items.Length;
      }

      return false;
    }

    /// <summary>Removes the oldest item from the queue and returns it</summary>
    /// <returns>The oldest item that was in the queue</returns>
    public ItemType Dequeue() {
      int dequeuedItemIndex = this.headIndex;

      // We do not clear the removed item, intentionally
      AdvanceHead();

      return this.items[dequeuedItemIndex];
    }

    /// <summary>Advances the head index of the queue</summary>
    public void AdvanceHead() {
      if (this.count == 0) {
        throw new InvalidOperationException("Queue is empty");
      }

      this.headIndex = (this.headIndex + 1) % this.items.Length;
      --this.count;
    }

    /// <summary>Appends an item to the queue</summary>
    /// <param name="item">Item that will be appended</param>
    public void Enqueue(ItemType item) {
      EnsureSlotAvailable();

      this.items[this.tailIndex] = item;

      AdvanceTail();
    }

    /// <summary>Advances the tail index of the queue</summary>
    public void AdvanceTail() {
      this.tailIndex = (this.tailIndex + 1) % this.items.Length;
      ++this.count;
    }

    /// <summary>
    ///   Ensures that the is space for at least one more item available
    /// </summary>
    /// <remarks>
    ///   Call this before manually inserting an item into the queue
    /// </remarks>
    public void EnsureSlotAvailable() {
      if (this.count == this.items.Length) {
        int capacity = this.items.Length * GrowFactor;
        if (capacity < (this.items.Length + MinimumGrow)) {
          capacity = this.items.Length + MinimumGrow;
        }

        setCapacity(capacity);
      }
    }

    /// <summary>Returns the oldest item in the queue without removing it</summary>
    /// <returns>The oldest item that is in the queue</returns>
    public ItemType Peek() {
      if (this.count == 0) {
        throw new InvalidOperationException("Queue is empty");
      }

      return this.items[this.headIndex];
    }

    /// <summary>Changes the capacity of the queue</summary>
    /// <param name="capacity">New capacity the queue will assume</param>
    private void setCapacity(int capacity) {
      ItemType[] newItemArray = new ItemType[capacity];
      if (this.count > 0) {
        if (this.headIndex < this.tailIndex) {
          Array.Copy(this.items, this.headIndex, newItemArray, 0, this.count);
        } else {
          Array.Copy(
            this.items, this.headIndex,
            newItemArray, 0, this.items.Length - this.headIndex
          );
          Array.Copy(
            this.items, 0,
            newItemArray, this.items.Length - this.headIndex, this.tailIndex
          );
        }
      }

      this.items = newItemArray;
      this.headIndex = 0;
      this.tailIndex = (this.count == capacity) ? 0 : this.count;
    }

    /// <summary>Returns the contents of the queue as an array</summary>
    /// <returns>A new array containing all items that are in the queue</returns>
    public ItemType[] ToArray() {
      ItemType[] array = new ItemType[this.count];
      if (this.count != 0) {
        if (this.headIndex < this.tailIndex) {
          Array.Copy(this.items, this.headIndex, array, 0, this.count);
        } else {
          Array.Copy(
            this.items, this.headIndex,
            array, 0, this.items.Length - this.headIndex
          );
          Array.Copy(
            this.items, 0,
            array, this.items.Length - this.headIndex, this.tailIndex
          );
        }
      }

      return array;
    }

    /// <summary>Number of items currently stored in the queue</summary>
    public int Count {
      get { return this.count; }
    }

    /// <summary>Index of the first item in the queue</summary>
    public int HeadIndex {
      get { return this.headIndex; }
    }

    /// <summary>Index one past the last item in the queue</summary>
    public int TailIndex {
      get { return this.tailIndex; }
    }

    /// <summary>Returns the internal item array of the queue</summary>
    public ItemType[] Items {
      get { return this.items; }
    }

    /// <summary>Empty item array to avoid garbage during initialization</summary>
    private static readonly ItemType[] emptyArray = new ItemType[0];

    /// <summary>Contains the ring-buffered items stored in the queue</summary>
    private ItemType[] items;

    /// <summary>Index of the queue's head element</summary>
    private int headIndex;
    /// <summary>Index of the queue's tail element</summary>
    private int tailIndex;
    /// <summary>Number of items currently contained in the queue</summary>
    private int count;

  }

} // namespace Nuclex.Input.Devices
