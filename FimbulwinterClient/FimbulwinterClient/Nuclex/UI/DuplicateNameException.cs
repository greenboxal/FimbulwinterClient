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

namespace Nuclex.UserInterface {

  /// <summary>The control's id has already been taken by another control</summary>
  /// <remarks>
  ///   This exception indicates that you have a name collision between two controls
  ///   in the same collection. It will either occur when you add a control to a
  ///   collection that already contains a control with the same name, or when you
  ///   change the name of a control to that of another control in the same collection.
  /// </remarks>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class DuplicateNameException : Exception {

    /// <summary>Initializes the exception</summary>
    public DuplicateNameException() { }

    /// <summary>Initializes the exception with an error message</summary>
    /// <param name="message">Error message describing the cause of the exception</param>
    public DuplicateNameException(string message) : base(message) { }

    /// <summary>Initializes the exception as a followup exception</summary>
    /// <param name="message">Error message describing the cause of the exception</param>
    /// <param name="inner">Preceding exception that has caused this exception</param>
    public DuplicateNameException(string message, Exception inner) : base(message, inner) { }

#if !NO_SERIALIZATION

    /// <summary>Initializes the exception from its serialized state</summary>
    /// <param name="info">Contains the serialized fields of the exception</param>
    /// <param name="context">Additional environmental informations</param>
    protected DuplicateNameException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context
    )
      : base(info, context) { }

#endif // NO_SERIALIZATION

  }

} // namespace Nuclex.UserInterface
