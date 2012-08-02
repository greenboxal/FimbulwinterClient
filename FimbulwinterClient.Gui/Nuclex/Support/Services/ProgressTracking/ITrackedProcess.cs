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

  /// <summary>Process whose progress is being tracked</summary>
  public interface ITrackedProcess {

    /// <summary>Fired whenever the progress of the process changes</summary>
    event EventHandler<ProgressReportEventArgs> ProgressChanged;

    /// <summary>Unique identifier of the overall process</summary>
    /// <remarks>
    ///   As a convention, using this object's ToString() method should return
    ///   something usable, either a string that can be displayed in the user
    ///   interface or, depending on your architecture, the object could
    ///   implement certain interfaces that allow a localized version of
    ///   the string to be created.
    /// </remarks>
    object ProcessIdentifier { get; }

    /// <summary>Progress that process has achieved so far</summary>
    float CurrentProgress { get; }

  }

} // namespace Nuclex.Support.DependencyInjection.ProgressTracking

#endif // ENABLE_SERVICEMANAGER
