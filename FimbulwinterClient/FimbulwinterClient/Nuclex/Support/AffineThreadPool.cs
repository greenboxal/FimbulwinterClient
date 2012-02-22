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
using System.Runtime.InteropServices;
using System.Threading;

namespace Nuclex.Support {

  /// <summary>Alternative Thread pool providing one thread for each core</summary>
  /// <remarks>
  ///   <para>
  ///     Unlike the normal thread pool, the affine thread pool provides only as many
  ///     threads as there are CPU cores available on the current platform. This makes
  ///     it more suitable for tasks you want to spread across all available cpu cores
  ///     explicitly.
  ///   </para>
  ///   <para>
  ///     However, it's not a good match if you want to run blocking or waiting tasks
  ///     inside the thread pool because the limited available threads will become
  ///     congested quickly. It is encouraged to use this class in parallel with
  ///     .NET's own thread pool, putting tasks that can block into the .NET thread
  ///     pool and task that perform pure processing into the affine thread pool.
  ///   </para>
  ///   <para>
  ///     Implementation based on original code provided by Stephen Toub
  ///     (stoub at microsoft ignorethis dot com)
  ///   </para>
  /// </remarks>
  public static class AffineThreadPool {

    /// <summary>Number of CPU cores available on the system</summary>
#if XBOX360
    public static readonly int Processors = 4;
#else
    public static readonly int Processors = Environment.ProcessorCount;
#endif

    /// <summary>Delegate used by the thread pool to report unhandled exceptions</summary>
    /// <param name="exception">Exception that has not been handled</param>
    public delegate void ExceptionDelegate(Exception exception);

    #region class UserWorkItem

    /// <summary>Used to hold a callback delegate and the state for that delegate.</summary>
    private struct UserWorkItem {

      /// <summary>Initialize the callback holding object.</summary>
      /// <param name="callback">Callback delegate for the callback.</param>
      /// <param name="state">State with which to call the callback delegate.</param>
      public UserWorkItem(WaitCallback callback, object state) {
        this.Callback = callback;
        this.State = state;
      }

      /// <summary>Callback delegate for the callback.</summary>
      public WaitCallback Callback;
      /// <summary>State with which to call the callback delegate.</summary>
      public object State;

    }

    #endregion // class UserWorkItem

    /// <summary>Initializes the thread pool</summary>
    static AffineThreadPool() {

      // Create our thread stores; we handle synchronization ourself
      // as we may run into situations where multiple operations need to be atomic.
      // We keep track of the threads we've created just for good measure; not actually
      // needed for any core functionality.
#if XBOX360 || WINDOWS_PHONE
      workAvailable = new Semaphore();
#else
      workAvailable = new System.Threading.Semaphore(0, int.MaxValue);
#endif
      userWorkItems = new Queue<UserWorkItem>(Processors * 4);
      workerThreads = new List<Thread>(Processors);
      inUseThreads = 0;

#if XBOX360
      // We can only use these hardware thread indices on the XBox 360
      hardwareThreads = new Queue<int>(new int[] { 5, 4, 3, 1 });
#else
      // We can use all cores on a PC, starting from index 1
      hardwareThreads = new Queue<int>(Processors);
      for(int core = Processors; core >= 1; --core) {
        hardwareThreads.Enqueue(core);
      }
#endif

      // Create all of the worker threads
      for(int index = 0; index < Processors; index++) {

        // Create a new thread and add it to the list of threads.
        Thread newThread = new Thread(new ThreadStart(ProcessQueuedItems));
        workerThreads.Add(newThread);

        // Configure the new thread and start it
        newThread.Name = "Nuclex.Support.AffineThreadPool Thread #" + index.ToString();
        newThread.IsBackground = true;
        newThread.Start();

      }

    }

    /// <summary>Queues a user work item to the thread pool</summary>
    /// <param name="callback">
    ///   A WaitCallback representing the delegate to invoke when a thread in the 
    ///   thread pool picks up the work item
    /// </param>
    public static void QueueUserWorkItem(WaitCallback callback) {

      // Queue the delegate with no state
      QueueUserWorkItem(callback, null);

    }

    /// <summary>Queues a user work item to the thread pool.</summary>
    /// <param name="callback">
    ///   A WaitCallback representing the delegate to invoke when a thread in the 
    ///   thread pool picks up the work item
    /// </param>
    /// <param name="state">
    ///   The object that is passed to the delegate when serviced from the thread pool
    /// </param>
    public static void QueueUserWorkItem(WaitCallback callback, object state) {

      // Create a waiting callback that contains the delegate and its state.
      // Add it to the processing queue, and signal that data is waiting.
      UserWorkItem waiting = new UserWorkItem(callback, state);
      lock(userWorkItems) {
        userWorkItems.Enqueue(waiting);
      }

      // Wake up one of the worker threads so this task will be processed
      workAvailable.Release();

    }

    /// <summary>Gets the number of threads at the disposal of the thread pool</summary>
    public static int MaxThreads { get { return Processors; } }

    /// <summary>Gets the number of currently active threads in the thread pool</summary>
    public static int ActiveThreads { get { return inUseThreads; } }

    /// <summary>
    ///   Gets the number of callback delegates currently waiting in the thread pool
    /// </summary>
    public static int WaitingWorkItems {
      get {
        lock(userWorkItems) {
          return userWorkItems.Count;
        }
      }
    }

    /// <summary>
    ///   Default handler used to respond to unhandled exceptions in ThreadPool threads
    /// </summary>
    /// <param name="exception">Exception that has occurred</param>
    internal static void DefaultExceptionHandler(Exception exception) {
      throw exception;
    }

#if WINDOWS
    /// <summary>Retrieves the ProcessThread for the calling thread</summary>
    /// <returns>The ProcessThread for the calling thread</returns>
    internal static ProcessThread GetProcessThread(int threadId) {
      ProcessThreadCollection threads = Process.GetCurrentProcess().Threads;
      for(int index = 0; index < threads.Count; ++index) {
        if(threads[index].Id == threadId) {
          return threads[index];
        }
      }

      return null;
    }
#endif

    /// <summary>A thread worker function that processes items from the work queue</summary>
    private static void ProcessQueuedItems() {

      // Get the system/hardware thread index this thread is going to use. We hope that
      // the threads more or less start after each other, but there is no guarantee that
      // tasks will be handled by the CPU cores in the order the queue was filled with.
      // This could be added, though, by using a WaitHandle so the thread creator could
      // wait for each thread to take one entry out of the queue.
      int hardwareThreadIndex;
      lock(hardwareThreads) {
        hardwareThreadIndex = hardwareThreads.Dequeue();
      }

#if XBOX360
      // On the XBox 360, the only way to get a thread to run on another core is to
      // explicitly move it to that core. MSDN states that SetProcessorAffinity() should
      // be called from the thread whose affinity is being changed.
      Thread.CurrentThread.SetProcessorAffinity(new int[] { hardwareThreadIndex });
#elif WINDOWS
      if(Environment.OSVersion.Platform == PlatformID.Win32NT) {
        // Prevent this managed thread from impersonating another system thread.
        // In .NET, managed threads can supposedly be moved to different system threads
        // and, more worryingly, even fibers. This should make sure we're sitting on
        // a normal system thread and stay with that thread during our lifetime.
        Thread.BeginThreadAffinity();

        // Assign the ideal processor, but don't force it. It's not a good idea to
        // circumvent the thread scheduler of a desktop machine, so we try to play nice.
        int threadId = GetCurrentThreadId();
        ProcessThread thread = GetProcessThread(threadId);
        if(thread != null) {
          thread.IdealProcessor = hardwareThreadIndex;
        }
      }
#endif

      // Keep processing tasks indefinitely
      for(; ; ) {
        UserWorkItem workItem = getNextWorkItem();

        // Execute the work item we just picked up. Make sure to accurately
        // record how many callbacks are currently executing.
        Interlocked.Increment(ref inUseThreads);
        try {
          workItem.Callback(workItem.State);
        }
        catch(Exception exception) { // Make sure we don't throw here.
          ExceptionDelegate exceptionHandler = ExceptionHandler;
          if(exceptionHandler != null) {
            exceptionHandler(exception);
          }
        }
        finally {
          Interlocked.Decrement(ref inUseThreads);
        }
      }
    }

    /// <summary>Obtains the next work item from the queue</summary>
    /// <returns>The next work item in the queue</returns>
    /// <remarks>
    ///   If the queue is empty, the call will block until an item is added to
    ///   the queue and the calling thread was the one picking it up.
    /// </remarks>
    private static UserWorkItem getNextWorkItem() {

      // Get the next item in the queue. If there is nothing there, go to sleep
      // for a while until we're woken up when a callback is waiting.
      for(; ; ) {

        // Try to get the next callback available.  We need to lock on the 
        // queue in order to make our count check and retrieval atomic.
        lock(userWorkItems) {
          if(userWorkItems.Count > 0) {
            return userWorkItems.Dequeue();
          }
        }

        // If we can't get one, go to sleep. The semaphore blocks until work
        // becomes available (then acting like an AutoResetEvent that counts
        // how often it has been triggered and letting that number of threads
        // pass through.)
        workAvailable.WaitOne();

      }

    }

    /// <summary>Delegate used to handle assertion checks in the code</summary>
    public static volatile ExceptionDelegate ExceptionHandler = DefaultExceptionHandler;

#if WINDOWS
    /// <summary>Retrieves the calling thread's thread id</summary>
    /// <returns>The thread is of the calling thread</returns>
    [DllImport("kernel32.dll")]
    internal static extern int GetCurrentThreadId();
#endif

    /// <summary>Available hardware threads the thread pool threads pick from</summary>
    private static Queue<int> hardwareThreads;
    /// <summary>Queue of all the callbacks waiting to be executed.</summary>
    private static Queue<UserWorkItem> userWorkItems;
    /// <summary>
    ///   Used to let the threads in the thread pool wait for new work to appear.
    /// </summary>
#if XBOX360 || WINDOWS_PHONE
    private static Semaphore workAvailable;
#else
    private static System.Threading.Semaphore workAvailable;
#endif
    /// <summary>List of all worker threads at the disposal of the thread pool.</summary>
    private static List<Thread> workerThreads;
    /// <summary>Number of threads currently active.</summary>
    private static int inUseThreads;

  }

} // namespace Nuclex.Support
