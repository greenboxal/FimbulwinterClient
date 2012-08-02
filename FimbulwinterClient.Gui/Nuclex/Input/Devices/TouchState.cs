#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2011 Nuclex Development Labs

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

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Nuclex.Input.Devices {

  /// <summary>Stores the state of a touch panel</summary>
  public struct TouchState {

    /// <summary>Initializes a new touch panel state</summary>
    /// <param name="isAttached">Whether the touch panel is connected</param>
    /// <param name="touches">Touch events since the last update</param>
    public TouchState(bool isAttached, TouchCollection touches) {
      this.isAttached = isAttached;
      this.touches = touches;
    }

    /// <summary>Whether the touch panel is connected</summary>
    /// <remarks>
    ///   If the touch panel is not connected, all data in the state will
    ///   be neutral
    /// </remarks>
    public bool IsAttached {
      get { return this.isAttached; }
    }

    /// <summary>Touch events that occured since the last update</summary>
    public TouchCollection Touches {
      get { return this.touches; }
    }

    /// <summary>Whether the touch panel is connected</summary>
    private bool isAttached;
    /// <summary>Collection of touches since the last update</summary>
    private TouchCollection touches;

  }

} // namespace Nuclex.Input.Devices
