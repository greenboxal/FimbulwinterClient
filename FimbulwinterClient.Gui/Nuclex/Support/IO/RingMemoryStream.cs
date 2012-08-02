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
using System.IO;

namespace Nuclex.Support.IO {

  /// <summary>Specialized memory stream for ring buffers</summary>
  /// <remarks>
  ///   This ring buffer class is specialized for binary data and tries to achieve
  ///   optimal efficiency when storing and retrieving chunks of several bytes
  ///   at once. Typical use cases include audio and network buffers where one party
  ///   is responsible for refilling the buffer at regular intervals while the other
  ///   constantly streams data out of it.
  /// </remarks>
  public class RingMemoryStream : Stream {

    /// <summary>Initializes a new ring memory stream</summary>
    /// <param name="capacity">Maximum capacity of the stream</param>
    public RingMemoryStream(int capacity) {
      this.ringBuffer = new MemoryStream(capacity);
      this.ringBuffer.SetLength(capacity);
      this.empty = true;
    }

    /// <summary>Maximum amount of data that will fit into the ring memory stream</summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Thrown if the new capacity is too small for the data already contained
    ///   in the ring buffer.
    /// </exception>
    public long Capacity {
      get { return this.ringBuffer.Length; }
      set {
        int length = (int)Length;
        if(value < length) {
          throw new ArgumentOutOfRangeException(
            "New capacity is less than the stream's current length"
          );
        }

        // This could be done in a more efficient manner than just replacing
        // the entire buffer, but since this operation will probably be only called
        // once during the lifetime of the application, if at all, I don't see
        // the need to optimize it...
        MemoryStream newBuffer = new MemoryStream((int)value);

        newBuffer.SetLength(value);
        if(length > 0) {
          Read(newBuffer.GetBuffer(), 0, length);
        }

        this.ringBuffer.Close(); // Equals dispose of the old buffer
        this.ringBuffer = newBuffer;
        this.startIndex = 0;
        this.endIndex = length;
      }

    }

    /// <summary>Whether it's possible to read from this stream</summary>
    public override bool CanRead { get { return true; } }
    /// <summary>Whether this stream supports random access</summary>
    public override bool CanSeek { get { return false; } }
    /// <summary>Whether it's possible to write into this stream</summary>
    public override bool CanWrite { get { return true; } }
    /// <summary>Flushes the buffers and writes down unsaved data</summary>
    public override void Flush() { }

    /// <summary>Current length of the stream</summary>
    public override long Length {
      get {
        if((this.endIndex > this.startIndex) || this.empty) {
          return this.endIndex - this.startIndex;
        } else {
          return this.ringBuffer.Length - this.startIndex + this.endIndex;
        }
      }
    }

    /// <summary>Current cursor position within the stream</summary>
    /// <exception cref="NotSupportedException">Always</exception>
    public override long Position {
      get { throw new NotSupportedException("The ring buffer does not support seeking"); }
      set { throw new NotSupportedException("The ring buffer does not support seeking"); }
    }

    /// <summary>Reads data from the beginning of the stream</summary>
    /// <param name="buffer">Buffer in which to store the data</param>
    /// <param name="offset">Starting index at which to begin writing the buffer</param>
    /// <param name="count">Number of bytes to read from the stream</param>
    /// <returns>Die Number of bytes actually read</returns>
    public override int Read(byte[] buffer, int offset, int count) {

      // The end index lies behind the start index (usual case), so the
      // ring memory is not fragmented. Example: |-----<#######>-----|
      if((this.startIndex < this.endIndex) || this.empty) {

        // The Stream interface requires us to return less than the requested
        // number of bytes if we don't have enough data
        count = Math.Min(count, this.endIndex - this.startIndex);
        if(count > 0) {
          this.ringBuffer.Position = this.startIndex;
          this.ringBuffer.Read(buffer, offset, count);
          this.startIndex += count;

          if(this.startIndex == this.endIndex) {
            setEmpty();
          }
        }

      } else { // The end index lies in front of the start index

        // With the end before the start index, the data in the ring memory
        // stream is fragmented. Example: |#####>-------<#####|
        int linearAvailable = (int)this.ringBuffer.Length - this.startIndex;

        // Will this read process cross the end of the ring buffer, requiring us to
        // read the data in 2 steps?
        if(count > linearAvailable) {

          // The Stream interface requires us to return less than the requested
          // number of bytes if we don't have enough data
          count = Math.Min(count, linearAvailable + this.endIndex);

          this.ringBuffer.Position = this.startIndex;
          this.ringBuffer.Read(buffer, offset, linearAvailable);
          this.ringBuffer.Position = 0;
          this.startIndex = count - linearAvailable;
          this.ringBuffer.Read(buffer, offset + linearAvailable, this.startIndex);

        } else { // Nope, the amount of requested data can be read in one piece
          this.ringBuffer.Position = this.startIndex;
          this.ringBuffer.Read(buffer, offset, count);
          this.startIndex += count;

        }

        // If we consumed the entire ring buffer, set the empty flag and move
        // the indexes back to zero for better performance
        if(this.startIndex == this.endIndex) {
          setEmpty();
        }

      }

      return count;
    }

    /// <summary>Appends data to the end of the stream</summary>
    /// <param name="buffer">Buffer containing the data to append</param>
    /// <param name="offset">Starting index of the data in the buffer</param>
    /// <param name="count">Number of bytes to write to the stream</param>
    /// <exception cref="OverflowException">When the ring buffer is full</exception>
    public override void Write(byte[] buffer, int offset, int count) {

      // The end index lies behind the start index (usual case), so the
      // unused buffer space is fragmented. Example: |-----<#######>-----|
      if((this.startIndex < this.endIndex) || this.empty) {
        int linearAvailable = (int)(this.ringBuffer.Length - this.endIndex);

        // If the data to be written would cross the ring memory stream's end,
        // we have to check that there's enough space at the beginning of the
        // stream to contain the remainder of the data.
        if(count > linearAvailable) {
          if(count > (linearAvailable + this.startIndex))
            throw new OverflowException("Data does not fit in buffer");

          this.ringBuffer.Position = this.endIndex;
          this.ringBuffer.Write(buffer, offset, linearAvailable);
          this.ringBuffer.Position = 0;
          this.endIndex = count - linearAvailable;
          this.ringBuffer.Write(buffer, offset + linearAvailable, this.endIndex);

        } else { // All data can be appended at the current stream position
          this.ringBuffer.Position = this.endIndex;
          this.ringBuffer.Write(buffer, offset, count);
          this.endIndex += count;
        }

        this.empty = false;

      } else { // The end index lies before the start index

        // The ring memory stream has been fragmented. This means the gap into which
        // we are about to write is not fragmented. Example: |#####>-------<#####|
        if(count > (this.startIndex - this.endIndex))
          throw new OverflowException("Data does not fit in buffer");

        // Because the gap isn't fragmented, we can be sure that a single
        // write call will suffice.
        this.ringBuffer.Position = this.endIndex;
        this.ringBuffer.Write(buffer, offset, count);
        this.endIndex += count;

      }

    }

    /// <summary>Jumps to the specified location within the stream</summary>
    /// <param name="offset">Position to jump to</param>
    /// <param name="origin">Origin towards which to interpret the offset</param>
    /// <returns>The new offset within the stream</returns>
    /// <exception cref="NotSupportedException">Always</exception>
    public override long Seek(long offset, SeekOrigin origin) {
      throw new NotSupportedException("The ring buffer does not support seeking");
    }

    /// <summary>Changes the length of the stream</summary>
    /// <param name="value">New length to resize the stream to</param>
    /// <exception cref="NotSupportedException">Always</exception>
    public override void SetLength(long value) {
      throw new NotSupportedException("This operation is not supported");
    }

    /// <summary>Resets the stream to its empty state</summary>
    private void setEmpty() {
      this.empty = true;
      this.startIndex = 0;
      this.endIndex = 0;
    }

    /// <summary>Internal stream containing the ring buffer data</summary>
    private MemoryStream ringBuffer;
    /// <summary>Start index of the data within the ring buffer</summary>
    private int startIndex;
    /// <summary>End index of the data within the ring buffer</summary>
    private int endIndex;
    /// <summary>Whether the ring buffer is empty</summary>
    /// <remarks>
    ///   This field is required to differentiate between the ring buffer being
    ///   filled to the limit and being totally empty, because in both cases,
    ///   the start index and the end index will be the same. 
    /// </remarks>
    private bool empty;

  }

} // namespace Nuclex.Support.IO
