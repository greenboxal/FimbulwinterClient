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
using System.Threading;

namespace Nuclex.Support {

  /// <summary>A reverse counting semaphore</summary>
  /// <remarks>
  ///   <para>
  ///     This semaphore counts in reverse, which means you can Release() the semaphore
  ///     as often as you'd like a thread calling WaitOne() to be let through. You
  ///     can use it in the traditional sense and have any Thread calling WaitOne()
  ///     make sure to call Release() afterwards, or you can, for example, Release() it
  ///     whenever work becomes available and let threads take work from the Semaphore
  ///     by calling WaitOne() alone.
  ///   </para>
  ///   <para>
  ///     Implementation notes (ignore this if you just want to use the Semaphore)
  ///   </para>
  ///   <para>
  ///     We could design a semaphore that uses an auto reset event, where the thread
  ///     that gets to pass immediately sets the event again if the semaphore isn't full
  ///     yet to let another thread pass.
  ///   </para>
  ///   <para>
  ///     However, this would mean that when a semaphore receives a large number of
  ///     wait requests, assuming it would allow, for example, 25 users at once, the
  ///     thread scheduler would see only 1 thread become eligible for execution. Then
  ///     that thread would unlock the next and so on. In short, we wait 25 times
  ///     for the thread scheduler to wake up a thread until all users get through.
  ///   </para>
  ///   <para>
  ///     So we chose a ManualResetEvent, which will wake up more threads than
  ///     neccessary and possibly cause a period of intense competition for getting
  ///     a lock on the resource, but will make the thread scheduler see all threads
  ///     become eligible for execution.
  ///   </para>
  /// </remarks>
#if !(XBOX360 || WINDOWS_PHONE)
  [Obsolete("Prefer the normal semaphore on Windows builds.")]
#endif
  public class Semaphore : WaitHandle {

    /// <summary>Initializes a new semaphore</summary>
    public Semaphore() {
      createEvent();
    }

    /// <summary>Initializes a new semaphore</summary>
    /// <param name="count">
    ///   Number of users that can access the resource at the same time
    /// </param>
    public Semaphore(int count) {
      this.free = count;

      createEvent();
    }

    /// <summary>Initializes a new semaphore</summary>
    /// <param name="initialCount">
    ///   Initial number of users accessing the resource 
    /// </param>
    /// <param name="maximumCount">
    ///   Maximum numbr of users that can access the resource at the same time
    /// </param>
    public Semaphore(int initialCount, int maximumCount) {
      if(initialCount > maximumCount) {
        throw new ArgumentOutOfRangeException(
          "initialCount", "Initial count must not be larger than the maximum count"
        );
      }

      this.free = maximumCount - initialCount;
      createEvent();
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    /// <param name="explicitDisposing">
    ///   Whether Dispose() has been called explictly
    /// </param>
    protected override void Dispose(bool explicitDisposing) {
      if(this.manualResetEvent != null) {
        base.SafeWaitHandle = null;

        this.manualResetEvent.Close();
        this.manualResetEvent = null;
      }

      base.Dispose(explicitDisposing);
    }

    /// <summary>
    ///   Waits for the resource to become available and locks it
    /// </summary>
    /// <param name="millisecondsTimeout">
    ///   Number of milliseconds to wait at most before giving up
    /// </param>
    /// <param name="exitContext">
    ///   True to exit the synchronization domain for the context before the wait (if
    ///   in a synchronized context), and reacquire it afterward; otherwise, false.
    /// </param>
    /// <returns>
    ///   True if the resource was available and is now locked, false if
    ///   the timeout has been reached.
    /// </returns>
#if NO_EXITCONTEXT
    public override bool WaitOne(int millisecondsTimeout) {
#else
    public override bool WaitOne(int millisecondsTimeout, bool exitContext) {
#endif
      for (; ; ) {

        // Lock the resource - even if it is full. We will correct out mistake later
        // if we overcomitted the resource.
        int newFree = Interlocked.Decrement(ref this.free);

        // If we got the resource, let the thread pass without further processing.
        if(newFree >= 0) {
          if(newFree > 0) {
            this.manualResetEvent.Set();
          }

          return true;
        }

        // We overcomitted the resource, count it down again. We know that, at least
        // moments ago, the resource was busy, so block the event.
        this.manualResetEvent.Reset();
        Thread.MemoryBarrier();
        newFree = Interlocked.Increment(ref this.free);

        // Unless we have been preempted by a Release(), we now have to wait for the
        // resource to become available.
        if(newFree >= 0) {
#if NO_EXITCONTEXT
          if(!this.manualResetEvent.WaitOne(millisecondsTimeout)) {
#else
          if(!this.manualResetEvent.WaitOne(millisecondsTimeout, exitContext)) {
#endif
            return false;
          }
        }

      } // for(; ; )
    }

    /// <summary>
    ///   Waits for the resource to become available and locks it
    /// </summary>
    /// <returns>
    ///   True if the resource was available and is now locked, false if
    ///   the timeout has been reached.
    /// </returns>
    public override bool WaitOne() {
#if NO_EXITCONTEXT
      return WaitOne(-1);
#else
      return WaitOne(-1, false);
#endif
    }

    /// <summary>
    ///   Waits for the resource to become available and locks it
    /// </summary>
    /// <param name="timeout">
    ///   Time span to wait for the lock before giving up
    /// </param>
    /// <param name="exitContext">
    ///   True to exit the synchronization domain for the context before the wait (if
    ///   in a synchronized context), and reacquire it afterward; otherwise, false.
    /// </param>
    /// <returns>
    ///   True if the resource was available and is now locked, false if
    ///   the timeout has been reached.
    /// </returns>
#if NO_EXITCONTEXT
    public override bool WaitOne(TimeSpan timeout) {
#else
    public override bool WaitOne(TimeSpan timeout, bool exitContext) {
#endif
      long totalMilliseconds = (long)timeout.TotalMilliseconds;
      if((totalMilliseconds < -1) || (totalMilliseconds > int.MaxValue)) {
        throw new ArgumentOutOfRangeException(
          "timeout", "Timeout must be either -1 or positive and less than 2^31"
        );
      }

#if NO_EXITCONTEXT
      return WaitOne((int)totalMilliseconds);
#else
      return WaitOne((int)totalMilliseconds, exitContext);
#endif
    }

    /// <summary>
    ///   Releases a lock on the resource. Note that for a reverse counting semaphore,
    ///   it is legal to Release() the resource before locking it.
    /// </summary>
    public void Release() {

      // Release one lock on the resource
      Interlocked.Increment(ref this.free);

      // Wake up any threads waiting for the resource to become available
      this.manualResetEvent.Set();

    }

    /// <summary>Creates the event used to make threads wait for the resource</summary>
    private void createEvent() {
      this.manualResetEvent = new ManualResetEvent(false);
      base.SafeWaitHandle = this.manualResetEvent.SafeWaitHandle;
    }

    /// <summary>Event used to make threads wait if the semaphore is full</summary>
    private ManualResetEvent manualResetEvent;
    /// <summary>Number of users currently accessing the resource</summary>
    /// <remarks>
    ///   Since this is a reverse counting semaphore, it will be negative if
    ///   the resource is available and 0 if the semaphore is full.
    /// </remarks>
    private int free;

  }

} // namespace Nuclex.Support
