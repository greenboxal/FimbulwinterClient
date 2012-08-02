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

namespace Nuclex.Support.Collections {

  /// <summary>Allows an object to be returned to its initial state</summary>
  /// <remarks>
  ///   <para>
  ///     This interface is typically implemented by objects which can be recycled
  ///     in order to avoid the construction overhead of a heavyweight class and to
  ///     eliminate garbage by reusing instances.
  ///   </para>
  ///   <para>
  ///     Recyclable objects should have a parameterless constructor and calling
  ///     their Recycle() method should bring them back into the state they were
  ///     in right after they had been constructed.
  ///   </para>
  /// </remarks>
  public interface IRecyclable {

    /// <summary>Returns the object to its initial state</summary>
    void Recycle();

  }

} // namespace Nuclex.Support.Collections
