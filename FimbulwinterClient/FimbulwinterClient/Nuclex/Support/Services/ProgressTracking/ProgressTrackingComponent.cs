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

#if ENABLE_SERVICEMANAGER

using System;
using System.Collections.Generic;

using Nuclex.Support.Tracking;

namespace Nuclex.Support.Services.ProgressTracking {

  /// <summary>Tracks the progress of running background processes</summary>
  public class ProgressTrackingComponent :
    IProgressCollectingService,
    IProgressPublishingService {

    /// <summary>Fired when the overall progress changes</summary>
    public event EventHandler<ProgressReportEventArgs> ProgressChanged;

    /// <summary>Initializes a new progress tracking component</summary>
    public ProgressTrackingComponent() {
    }

    /// <summary>Tracks the progress of the specified transaction</summary>
    /// <param name="transaction">
    ///   Transaction whose progress will be tracked
    /// </param>
    /// <param name="processIdentifier">
    ///   Identifier unique to the tracked background process. Can be null.
    /// </param>
    public void Track(Transaction transaction, object processIdentifier) {
    }

    /// <summary>Tracks the progress of the specified transaction</summary>
    /// <param name="transaction">
    ///   Transaction whose progress will be tracked
    /// </param>
    public void Track(Transaction transaction) {
    }

    /// <summary>Stops tracking the specified transaction</summary>
    /// <param name="transaction">Transaction that will no longer be tracked</param>
    public void Untrack(Transaction transaction) {
    }

    /// <summary>The overall progress of all tracked background processes</summary>
    public float TotalProgress {
      get { return 0.0f; }
    }

    /// <summary>Currently active background processes</summary>
    public IEnumerable<ITrackedProcess> TrackedProcesses {
      get { return null; }
    }

  }

} // namespace Nuclex.Support.Services.ProgressTracking

#endif // ENABLE_SERVICEMANAGER
