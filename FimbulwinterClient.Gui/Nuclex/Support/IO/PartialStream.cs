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
using System.Diagnostics;
using System.IO;

namespace Nuclex.Support.IO {

  /// <summary>Wraps a stream and exposes only a limited region of its data</summary>
  public class PartialStream : Stream {

    /// <summary>Initializes a new partial stream</summary>
    /// <param name="stream">
    ///   Stream the wrapper will make a limited region accessible of
    /// </param>
    /// <param name="start">
    ///   Start index in the stream which becomes the beginning for the wrapper
    /// </param>
    /// <param name="length">
    ///   Length the wrapped stream should report and allow access to
    /// </param>
    public PartialStream(Stream stream, long start, long length) {
      if(start < 0) {
        throw new ArgumentException("Start index must not be less than 0", "start");
      }

      if(!stream.CanSeek) {
        if(start != 0) {
          throw new ArgumentException(
            "The only valid start for unseekable streams is 0", "start"
          );
        }
      } else {
        if(start + length > stream.Length) {
          throw new ArgumentException(
            "Partial stream exceeds end of full stream", "length"
          );
        }
      }

      this.stream = stream;
      this.start = start;
      this.length = length;
    }

    /// <summary>Whether data can be read from the stream</summary>
    public override bool CanRead {
      get { return this.stream.CanRead; }
    }

    /// <summary>Whether the stream supports seeking</summary>
    public override bool CanSeek {
      get { return this.stream.CanSeek; }
    }

    /// <summary>Whether data can be written into the stream</summary>
    public override bool CanWrite {
      get { return this.stream.CanWrite; }
    }

    /// <summary>
    ///   Clears all buffers for this stream and causes any buffered data to be written
    ///   to the underlying device.
    /// </summary>
    public override void Flush() {
      this.stream.Flush();
    }

    /// <summary>Length of the stream in bytes</summary>
    /// <exception cref="NotSupportedException">
    ///   The wrapped stream does not support seeking
    /// </exception>
    public override long Length {
      get { return this.length; }
    }

    /// <summary>Absolute position of the file pointer within the stream</summary>
    /// <exception cref="NotSupportedException">
    ///   The wrapped stream does not support seeking
    /// </exception>
    public override long Position {
      get {
        if(!this.stream.CanSeek) {
          throw makeSeekNotSupportedException("seek");
        }

        return this.position;
      }
      set { moveFilePointer(value); }
    }

    /// <summary>
    ///   Reads a sequence of bytes from the stream and advances the position of
    ///   the file pointer by the number of bytes read.
    /// </summary>
    /// <param name="buffer">Buffer that will receive the data read from the stream</param>
    /// <param name="offset">
    ///   Offset in the buffer at which the stream will place the data read
    /// </param>
    /// <param name="count">Maximum number of bytes that will be read</param>
    /// <returns>
    ///   The number of bytes that were actually read from the stream and written into
    ///   the provided buffer
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///   The wrapped stream does not support reading
    /// </exception>
    public override int Read(byte[] buffer, int offset, int count) {
      if(!this.stream.CanRead) {
        throw new NotSupportedException(
          "Can't read: the wrapped stream doesn't support reading"
        );
      }

      long remaining = this.length - this.position;
      int bytesToRead = (int)Math.Min(count, remaining);

      if(this.stream.CanSeek) {
        this.stream.Position = this.position + this.start;
      }
      int bytesRead = this.stream.Read(buffer, offset, bytesToRead);
      this.position += bytesRead;

      return bytesRead;
    }

    /// <summary>Changes the position of the file pointer</summary>
    /// <param name="offset">
    ///   Offset to move the file pointer by, relative to the position indicated by
    ///   the <paramref name="origin" /> parameter.
    /// </param>
    /// <param name="origin">
    ///   Reference point relative to which the file pointer is placed
    /// </param>
    /// <returns>The new absolute position within the stream</returns>
    public override long Seek(long offset, SeekOrigin origin) {
      switch(origin) {
        case SeekOrigin.Begin: {
          return Position = offset;
        }
        case SeekOrigin.Current: {
          return Position += offset;
        }
        case SeekOrigin.End: {
          return Position = (Length + offset);
        }
        default: {
          throw new ArgumentException("Invalid seek origin", "origin");
        }
      }
    }

    /// <summary>Changes the length of the stream</summary>
    /// <param name="value">New length the stream shall have</param>
    /// <exception cref="NotSupportedException">
    ///   Always, the stream chainer does not support the SetLength() operation
    /// </exception>
    public override void SetLength(long value) {
      throw new NotSupportedException("Resizing partial streams is not supported");
    }

    /// <summary>
    ///   Writes a sequence of bytes to the stream and advances the position of
    ///   the file pointer by the number of bytes written.
    /// </summary>
    /// <param name="buffer">
    ///   Buffer containing the data that will be written to the stream
    /// </param>
    /// <param name="offset">
    ///   Offset in the buffer at which the data to be written starts
    /// </param>
    /// <param name="count">Number of bytes that will be written into the stream</param>
    /// <remarks>
    ///   The behavior of this method is as follows: If one or more chained streams
    ///   do not support seeking, all data is appended to the final stream in the
    ///   chain. Otherwise, writing will begin with the stream the current file pointer
    ///   offset falls into. If the end of that stream is reached, writing continues
    ///   in the next stream. On the last stream, writing more data into the stream
    ///   that it current size allows will enlarge the stream.
    /// </remarks>
    public override void Write(byte[] buffer, int offset, int count) {
      long remaining = this.length - this.position;
      if(count > remaining) {
        throw new NotSupportedException(
          "Cannot extend the length of the partial stream"
        );
      }

      if(this.stream.CanSeek) {
        this.stream.Position = this.position + this.start;
      }
      this.stream.Write(buffer, offset, count);

      this.position += count;
    }

    /// <summary>Stream being wrapped by the partial stream wrapper</summary>
    public Stream CompleteStream {
      get { return this.stream; }
    }

    /// <summary>Moves the file pointer</summary>
    /// <param name="position">New position the file pointer will be moved to</param>
    private void moveFilePointer(long position) {
      if(!this.stream.CanSeek) {
        throw makeSeekNotSupportedException("seek");
      }

      // Seemingly, it is okay to move the file pointer beyond the end of
      // the stream until you try to Read() or Write()
      this.position = position;
    }

    /// <summary>
    ///   Constructs a NotSupportException for an error caused by the wrapped
    ///   stream having no seek support
    /// </summary>
    /// <param name="action">Action that was tried to perform</param>
    /// <returns>The newly constructed NotSupportedException</returns>
    private static NotSupportedException makeSeekNotSupportedException(string action) {
      return new NotSupportedException(
        string.Format(
          "Can't {0}: the wrapped stream does not support seeking",
          action
        )
      );
    }

    /// <summary>Streams that have been chained together</summary>
    private Stream stream;
    /// <summary>Start index of the partial stream in the wrapped stream</summary>
    private long start;
    /// <summary>Zero-based position of the partial stream's file pointer</summary>
    /// <remarks>
    ///   If the stream does not support seeking, the position will simply be counted
    ///   up until it reaches <see cref="PartialStream.length" />.
    /// </remarks>
    private long position;
    /// <summary>Length of the partial stream</summary>
    private long length;


  }

} // namespace Nuclex.Support.IO
