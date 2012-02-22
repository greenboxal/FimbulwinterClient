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

namespace Nuclex.Input.Devices {

  /// <summary>Dummy that takes the place on unfilled mouse slots</summary>
  internal class NoMouse : IMouse {

    /// <summary>Fired when the mouse has been moved</summary>
    public event MouseMoveDelegate MouseMoved { add { } remove { } }

    /// <summary>Fired when one or more mouse buttons have been pressed</summary>
    public event MouseButtonDelegate MouseButtonPressed { add { } remove { } }

    /// <summary>Fired when one or more mouse buttons have been released</summary>
    public event MouseButtonDelegate MouseButtonReleased { add { } remove { } }

    /// <summary>Fired when the mouse wheel has been rotated</summary>
    public event MouseWheelDelegate MouseWheelRotated { add { } remove { } }

    /// <summary>Initializes a new mouse dummy</summary>
    public NoMouse() { }

    /// <summary>Retrieves the current state of the mouse</summary>
    /// <returns>The current state of the mouse</returns>
    public MouseState GetState() { return new MouseState(); }

    /// <summary>Moves the mouse cursor to the specified location</summary>
    /// <param name="x">New X coordinate of the mouse cursor</param>
    /// <param name="y">New Y coordinate of the mouse cursor</param>
    public void MoveTo(float x, float y) { }

    /// <summary>Whether the input device is connected to the system</summary>
    public bool IsAttached {
      get { return false; }
    }

    /// <summary>Human-readable name of the input device</summary>
    public string Name {
      get { return "No mouse connected"; }
    }

    /// <summary>Updates the state of the input device</summary>
    /// <remarks>
    ///   <para>
    ///     If this method is called with no snapshots in the queue, it will take
    ///     an immediate snapshot and make it the current state. This way, you
    ///     can use the input devices without caring for the snapshot system if
    ///     you wish.
    ///   </para>
    ///   <para>
    ///     If this method is called while one or more snapshots are waiting in
    ///     the queue, this method takes the next snapshot from the queue and makes
    ///     it the current state.
    ///   </para>
    /// </remarks>
    public void Update() { }

    /// <summary>Takes a snapshot of the current state of the input device</summary>
    /// <remarks>
    ///   This snapshot will be queued until the user calls the Update() method,
    ///   where the next polled snapshot will be taken from the queue and provided
    ///   to the user.
    /// </remarks>
    public void TakeSnapshot() { }

  }

} // namespace Nuclex.Input.Devices
