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

  /// <summary>Allows application-wide tracking of progress</summary>
  interface IProgressCollectingService {

    /// <summary>Tracks the progress of the specified transaction</summary>
    /// <param name="transaction">
    ///   Transaction whose progress will be tracked
    /// </param>
    /// <param name="processIdentifier">
    ///   Identifier unique to the tracked background process. Can be null.
    /// </param>
    /// <remarks>
    ///   <para>
    ///     Using the process identifier allows you to track the progress of multiple
    ///     transactions that are working independently of each other. This could,
    ///     for example, result in multiple progress bars being displayed in a GUI.
    ///     The global progress always is a combination of all transactions tracked
    ///     by the service.
    ///   </para>
    ///   <para>
    ///     A good analogy for this might be a download manager which uses several
    ///     threads to download multiple sections of a file at the same time. You
    ///     want a progress bar per file, but not one for each downloading thread.
    ///     Specifying the name object as a process identifer, all transactions
    ///     belonging to the same file would be merged into a single progress source.
    ///   </para>
    ///   <para>
    ///     The process identifier can be a string or any object. Note however that
    ///     as common practice, this object's ToString() method should return
    ///     something reasonable, like "Downloading xy.txt". Localization can be
    ///     achieved by implementing an interface (eg. ILocalizableToString) which
    ///     provides a string and some parameters - or you could return the already
    ///     translated versions of  the string if you prefer to have localized versions
    ///     of internal assemblies.
    ///   </para>
    /// </remarks>
    void Track(Transaction transaction, object processIdentifier);

    /// <summary>Tracks the progress of the specified transaction</summary>
    /// <param name="transaction">
    ///   Transaction whose progress will be tracked
    /// </param>
    /// <remarks>
    ///   Tracks the transaction as if it was added with the process identifier
    ///   set to null.
    /// </remarks>
    void Track(Transaction transaction);

    /// <summary>Stops tracking the specified transaction</summary>
    /// <param name="transaction">Transaction that will no longer be tracked</param>
    /// <remarks>
    ///   Untracking a transaction is optional. The service will automatically
    ///   remove finished transactions from its list of tracked transactions. Calling
    ///   this method is only required if you drop a transaction in a way that
    ///   prevents its AsyncEnded event from being fired (eg. by not executing it
    ///   at all, dispite adding it to the progress tracking service).
    /// </remarks>
    void Untrack(Transaction transaction);

  }

} // namespace Nuclex.Support.DependencyInjection.ProgressTracking

#endif // ENABLE_SERVICEMANAGER
