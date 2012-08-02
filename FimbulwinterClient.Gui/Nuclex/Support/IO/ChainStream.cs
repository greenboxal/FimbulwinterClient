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

  /// <summary>Chains a series of independent streams into a single stream</summary>
  /// <remarks>
  ///   <para>
  ///     This class can be used to chain multiple independent streams into a single
  ///     stream that acts as if its chained streams were only one combined stream.
  ///     It is useful to avoid creating huge memory streams or temporary files when
  ///     you just need to prepend or append some data to a stream or if you need to
  ///     read a file that was split into several parts as if it was a single file.
  ///   </para>
  ///   <para>
  ///     It is not recommended to change the size of any chained stream after it
  ///     has become part of a stream chainer, though the stream chainer will do its
  ///     best to cope with the changes as they occur. Increasing the length of a
  ///     chained stream is generally not an issue for streams that support seeking,
  ///     but reducing the length might invalidate the stream chainer's file pointer,
  ///     resulting in an IOException when Read() or Write() is next called.
  ///   </para>
  /// </remarks>
  public class ChainStream : Stream {

    /// <summary>Initializes a new stream chainer</summary>
    /// <param name="streams">Array of streams that will be chained together</param>
    public ChainStream(params Stream[] streams) {
      this.streams = (Stream[])streams.Clone();

      determineCapabilities();
    }

    /// <summary>Whether data can be read from the stream</summary>
    public override bool CanRead {
      get { return this.allStreamsCanRead; }
    }

    /// <summary>Whether the stream supports seeking</summary>
    public override bool CanSeek {
      get { return this.allStreamsCanSeek; }
    }

    /// <summary>Whether data can be written into the stream</summary>
    public override bool CanWrite {
      get { return this.allStreamsCanWrite; }
    }

    /// <summary>
    ///   Clears all buffers for this stream and causes any buffered data to be written
    ///   to the underlying device.
    /// </summary>
    public override void Flush() {
      for(int index = 0; index < this.streams.Length; ++index) {
        this.streams[index].Flush();
      }
    }

    /// <summary>Length of the stream in bytes</summary>
    /// <exception cref="NotSupportedException">
    ///   At least one of the chained streams does not support seeking
    /// </exception>
    public override long Length {
      get {
        if(!this.allStreamsCanSeek) {
          throw makeSeekNotSupportedException("determine length");
        }

        // Sum up the length of all chained streams
        long length = 0;
        for(int index = 0; index < this.streams.Length; ++index) {
          length += this.streams[index].Length;
        }

        return length;
      }
    }

    /// <summary>Absolute position of the file pointer within the stream</summary>
    /// <exception cref="NotSupportedException">
    ///   At least one of the chained streams does not support seeking
    /// </exception>
    public override long Position {
      get {
        if(!this.allStreamsCanSeek) {
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
    ///   The chained stream at the current position does not support reading
    /// </exception>
    public override int Read(byte[] buffer, int offset, int count) {
      if(!this.allStreamsCanRead) {
        throw new NotSupportedException(
          "Can't read: at least one of the chained streams doesn't support reading"
        );
      }

      int totalBytesRead = 0;
      int lastStreamIndex = this.streams.Length - 1;

      if(this.CanSeek) {

        // Find out from which stream and at which position we need to begin reading
        int streamIndex;
        long streamOffset;
        findStreamIndexAndOffset(this.position, out streamIndex, out streamOffset);

        // Try to read from the stream our current file pointer falls into. If more
        // data was requested than the stream contains, read each stream to its end
        // until we either have enough data or run out of streams.
        while(count > 0) {
          Stream currentStream = this.streams[streamIndex];

          // Read up to count bytes from the current stream. Count is decreased each
          // time we successfully get data and holds the number of bytes remaining
          // to be read
          long maximumBytes = Math.Min(count, currentStream.Length - streamOffset);
          currentStream.Position = streamOffset;
          int bytesRead = currentStream.Read(buffer, offset, (int)maximumBytes);

          // Accumulate the total number of bytes we read for the return value
          totalBytesRead += bytesRead;

          // If the stream returned partial data, stop here. Also, if this was the
          // last stream we queried, this is as far as we can go.
          if((bytesRead < maximumBytes) || (streamIndex == lastStreamIndex)) {
            break;
          }

          // Move on to the next stream in the chain
          ++streamIndex;
          streamOffset = 0;
          count -= bytesRead;
          offset += bytesRead;
        }

        this.position += totalBytesRead;

      } else {

        // Try to read from the active read stream. If the end of the active read
        // stream is reached, switch to the next stream in the chain until we have
        // no more streams left to read from
        while(this.activeReadStreamIndex <= lastStreamIndex) {

          // Try to read from the stream. The stream can either return any amount
          // of data > 0 if there's still data left ot be read or 0 if the end of
          // the stream was reached
          Stream activeStream = this.streams[this.activeReadStreamIndex];
          if(activeStream.CanSeek) {
            activeStream.Position = this.activeReadStreamPosition;
          }
          totalBytesRead = activeStream.Read(buffer, offset, count);

          // If we got any data, we're done, exit the loop
          if(totalBytesRead != 0) {
            break;
          } else { // Otherwise, go to the next stream in the chain
            this.activeReadStreamPosition = 0;
            ++this.activeReadStreamIndex;
          }
        }

        this.activeReadStreamPosition += totalBytesRead;

      }

      return totalBytesRead;
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
      throw new NotSupportedException("Resizing chained streams is not supported");
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
      if(!this.allStreamsCanWrite) {
        throw new NotSupportedException(
          "Can't write: at least one of the chained streams doesn't support writing"
        );
      }

      int remaining = count;

      // If seeking is supported, we can write into the mid of the stream,
      // if the user so desires
      if(this.allStreamsCanSeek) {

        // Find out in which stream and at which position we need to begin writing
        int streamIndex;
        long streamOffset;
        findStreamIndexAndOffset(this.position, out streamIndex, out streamOffset);

        // Write data into the streams, switching over to the next stream if data is
        // too large to fit into the current stream, until all data is spent.
        int lastStreamIndex = this.streams.Length - 1;
        while(remaining > 0) {
          Stream currentStream = this.streams[streamIndex];

          // If this is the last stream, just write. If the data is larger than the last
          // stream's remaining bytes, it will append to that stream, enlarging it.
          if(streamIndex == lastStreamIndex) {

            // Write all remaining data into the last stream
            currentStream.Position = streamOffset;
            currentStream.Write(buffer, offset, remaining);
            remaining = 0;

          } else { // We're writing into a stream that's followed by another stream

            // Find out how much data we can put into the current stream without
            // enlarging it (if seeking is supported, so is the Length property)
            long currentStreamRemaining = currentStream.Length - streamOffset;
            int bytesToWrite = (int)Math.Min((long)remaining, currentStreamRemaining);

            // Write all data that can fit into the current stream
            currentStream.Position = streamOffset;
            currentStream.Write(buffer, offset, bytesToWrite);

            // Adjust the offsets and count for the next stream
            offset += bytesToWrite;
            remaining -= bytesToWrite;
            streamOffset = 0;
            ++streamIndex;

          }
        }

      } else { // Seeking not supported, append everything to the last stream
        Stream lastStream = this.streams[this.streams.Length - 1];
        if(lastStream.CanSeek) {
          lastStream.Seek(0, SeekOrigin.End);
        }
        lastStream.Write(buffer, offset, remaining);
      }

      this.position += count;
    }

    /// <summary>Streams being combined by the stream chainer</summary>
    public Stream[] ChainedStreams {
      get { return this.streams; }
    }

    /// <summary>Moves the file pointer</summary>
    /// <param name="position">New position the file pointer will be moved to</param>
    private void moveFilePointer(long position) {
      if(!this.allStreamsCanSeek) {
        throw makeSeekNotSupportedException("seek");
      }

      // Seemingly, it is okay to move the file pointer beyond the end of
      // the stream until you try to Read() or Write()
      this.position = position;
    }

    /// <summary>
    ///   Finds the stream index and local offset for an absolute position within
    ///   the combined streams.
    /// </summary>
    /// <param name="overallPosition">Absolute position within the combined streams</param>
    /// <param name="streamIndex">
    ///   Index of the stream the overall position falls into
    /// </param>
    /// <param name="streamPosition">
    ///   Local position within the stream indicated by <paramref name="streamIndex" />
    /// </param>
    private void findStreamIndexAndOffset(
      long overallPosition, out int streamIndex, out long streamPosition
    ) {
      Debug.Assert(
        this.allStreamsCanSeek, "Call to findStreamIndexAndOffset() but no seek support"
      );

      // In case the position is beyond the stream's end, this is what we will
      // return to the caller
      streamIndex = (this.streams.Length - 1);

      // Search until we have found the stream the position must lie in
      for(int index = 0; index < this.streams.Length; ++index) {
        long streamLength = this.streams[index].Length;

        if(overallPosition < streamLength) {
          streamIndex = index;
          break;
        }

        overallPosition -= streamLength;
      }

      // The overall position will have been decreased by each skipped stream's length,
      // so it should now contain the local position for the final stream we checked.
      streamPosition = overallPosition;
    }

    /// <summary>Determines the capabilities of the chained streams</summary>
    /// <remarks>
    ///   <para>
    ///     Theoretically, it would be possible to create a stream chainer that supported
    ///     writing only when the file pointer was on a chained stream with write support,
    ///     that could seek within the beginning of the stream until the first chained
    ///     stream with no seek capability was encountered and so on.
    ///   </para>
    ///   <para>
    ///     However, the interface of the Stream class requires us to make a definitive
    ///     statement as to whether the Stream supports seeking, reading and writing.
    ///     We can't return "maybe" or "mostly" in CanSeek, so the only sane choice that
    ///     doesn't violate the Stream interface is to implement these capabilities as
    ///     all or nothing - either all streams support a feature, or the stream chainer
    ///     will report the feature as unsupported.
    ///   </para>
    /// </remarks>
    private void determineCapabilities() {
      this.allStreamsCanSeek = true;
      this.allStreamsCanRead = true;
      this.allStreamsCanWrite = true;

      for(int index = 0; index < this.streams.Length; ++index) {
        this.allStreamsCanSeek &= this.streams[index].CanSeek;
        this.allStreamsCanRead &= this.streams[index].CanRead;
        this.allStreamsCanWrite &= this.streams[index].CanWrite;
      }
    }

    /// <summary>
    ///   Constructs a NotSupportException for an error caused by one of the chained
    ///   streams having no seek support
    /// </summary>
    /// <param name="action">Action that was tried to perform</param>
    /// <returns>The newly constructed NotSupportedException</returns>
    private static NotSupportedException makeSeekNotSupportedException(string action) {
      return new NotSupportedException(
        string.Format(
          "Can't {0}: at least one of the chained streams does not support seeking",
          action
        )
      );
    }

    /// <summary>Streams that have been chained together</summary>
    private Stream[] streams;
    /// <summary>Current position of the overall file pointer</summary>
    private long position;
    /// <summary>Stream we're currently reading from if seeking is not supported</summary>
    /// <remarks>
    ///   If seeking is not supported, the stream chainer will read from each stream
    ///   until the end was reached
    ///   sequentially
    /// </remarks>
    private int activeReadStreamIndex;
    /// <summary>Position in the current read stream if seeking is not supported</summary>
    /// <remarks>
    ///   If there is a mix of streams supporting seeking and not supporting seeking, we
    ///   need to keep track of the read index for those streams that do. If, for example,
    ///   the last stream is written to and read from in succession, the file pointer
    ///   of that stream would have been moved to the end by the write attempt, skipping
    ///   data that should have been read in the following read attempt.
    /// </remarks>
    private long activeReadStreamPosition;

    /// <summary>Whether all of the chained streams support seeking</summary>
    private bool allStreamsCanSeek;
    /// <summary>Whether all of the chained streams support reading</summary>
    private bool allStreamsCanRead;
    /// <summary>Whether all of the chained streams support writing</summary>
    private bool allStreamsCanWrite;

  }

} // namespace Nuclex.Support.IO
