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

namespace Nuclex.Support {

#if WINDOWS_PHONE

  /// <summary>
  ///   Type-safe weak reference, referencing an object while still allowing
  ///   that object to be garbage collected.
  /// </summary>
  public class WeakReference<ReferencedType>
    where ReferencedType : class {

    /// <summary>
    ///   Initializes a new instance of the WeakReference class, referencing
    ///   the specified object.
    /// </summary>
    /// <param name="target">The object to track or null.</param>
    public WeakReference(ReferencedType target) {
      this.weakReference = new WeakReference(target);
    }

    /// <summary>
    ///   Initializes a new instance of the WeakReference class, referencing
    ///   the specified object optionally using resurrection tracking.
    /// </summary>
    /// <param name="target">An object to track.</param>
    /// <param name="trackResurrection">
    ///   Indicates when to stop tracking the object. If true, the object is tracked
    ///   after finalization; if false, the object is only tracked until finalization.
    /// </param>
    public WeakReference(ReferencedType target, bool trackResurrection) {
      this.weakReference = new WeakReference(target, trackResurrection);
    }

    /// <summary>
    ///   Implicitly converts a typed WeakReference into a non-typesafe WeakReference
    /// </summary>
    /// <param name="self">The types WeakReference that will be converted</param>
    /// <returns>The non-typesafe WeakReference</returns>
    public static implicit operator WeakReference(WeakReference<ReferencedType> self) {
      return self.weakReference;
    }

    /// <summary>
    ///   Gets or sets the object (the target) referenced by the current WeakReference
    ///   object.
    /// </summary>
    /// <remarks>
    ///   Is null if the object referenced by the current System.WeakReference object
    ///   has been garbage collected; otherwise, a reference to the object referenced
    ///   by the current System.WeakReference object.
    /// </remarks>
    /// <exception cref="System.InvalidOperationException">
    ///   The reference to the target object is invalid. This can occur if the current
    ///   System.WeakReference object has been finalized
    /// </exception>
    public ReferencedType Target {
      get { return this.weakReference.Target as ReferencedType; }
      set { this.weakReference.Target = value; }
    }

    /// <summary>
    ///   whether the object referenced by the WeakReference has been garbage collected
    /// </summary>
    public virtual bool IsAlive {
      get { return this.weakReference.IsAlive; }
    }

    /// <summary>
    ///   Whether the object referenced by the WeakReference is tracked after it is finalized
    /// </summary>
    public virtual bool TrackResurrection {
      get { return this.weakReference.TrackResurrection; }
    }

    /// <summary>The non-typesafe WeakReference being wrapped</summary>
    private WeakReference weakReference;

  }

#endif // WINDOWS_PHONE

} // namespace Nuclex.Support

