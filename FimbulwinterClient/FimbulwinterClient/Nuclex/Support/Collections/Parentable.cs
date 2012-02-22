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

namespace Nuclex.Support.Collections {

  /// <summary>Base class for objects that can be parented to an owner</summary>
  /// <typeparam name="ParentType">Type of the parent object</typeparam>
  public class Parentable<ParentType> {

    /// <summary>The parent object that owns this instance</summary>
    protected ParentType Parent {
      get { return this.parent; }
    }

    /// <summary>Invoked whenever the instance's owner changes</summary>
    /// <remarks>
    ///   When items are parented for the first time, the oldParent argument will
    ///   be null. Also, if the element is removed from the collection, the
    ///   current parent will be null.
    /// </remarks>
    /// <param name="oldParent">Previous owner of the instance</param>
    protected virtual void OnParentChanged(ParentType oldParent) { }

    /// <summary>Assigns a new parent to this instance</summary>
    internal void SetParent(ParentType parent) {
      ParentType oldParent = this.parent;
      this.parent = parent;

      OnParentChanged(oldParent);
    }

    /// <summary>Current parent of this object</summary>
    private ParentType parent;

  }

} // namespace Nuclex.Support.Collections
