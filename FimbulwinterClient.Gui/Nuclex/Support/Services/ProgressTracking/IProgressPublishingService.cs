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

  /// <summary>Reports the progress of tracked background processes</summary>
  /// <remarks>
  ///   <para>
  ///     This service is intended for the consumer of progress reports. It will notify
  ///     subscribers when background processes start, when progress is achieved and
  ///     when they finish.
  ///   </para>
  ///   <para>
  ///     Usually, this interface, together with the IProgressTrackingService interface,
  ///     is provided by a single service component that tracks the progress of
  ///     transactions taking place asynchronously and reports it back this way.
  ///   </para>
  /// </remarks>
  interface IProgressPublishingService {

    /// <summary>Fired when the overall progress changes</summary>
    event EventHandler<ProgressReportEventArgs> ProgressChanged;

    /// <summary>The overall progress of all tracked background processes</summary>
    float TotalProgress { get; }

    /// <summary>Currently active background processes</summary>
    IEnumerable<ITrackedProcess> TrackedProcesses { get; }

  }

} // namespace Nuclex.Support.DependencyInjection.ProgressTracking

#endif // ENABLE_SERVICEMANAGER
