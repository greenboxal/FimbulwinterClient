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

  /// <summary>Delegate used to report movement of the mouse cursor</summary>
  /// <param name="x">New X coordinate of the mouse cursor</param>
  /// <param name="y">New Y coordinate of the mouse cursor</param>
  public delegate void MouseMoveDelegate(float x, float y);

  /// <summary>
  ///   Delegate used to report a press or released of one or more mouse buttons
  /// </summary>
  /// <param name="buttons">Button or buttons that have been pressed or released</param>
  public delegate void MouseButtonDelegate(MouseButtons buttons);

  /// <summary>Delegate used to report a rotation of the mouse wheel</summary>
  /// <param name="ticks">Number of ticks the mouse wheel has been rotated</param>
  public delegate void MouseWheelDelegate(float ticks);

  /// <summary>Specializd input devices for mouse-like controllers</summary>
  public interface IMouse : IInputDevice {

    /// <summary>Fired when the mouse has been moved</summary>
    event MouseMoveDelegate MouseMoved;

    /// <summary>Fired when one or more mouse buttons have been pressed</summary>
    event MouseButtonDelegate MouseButtonPressed;

    /// <summary>Fired when one or more mouse buttons have been released</summary>
    event MouseButtonDelegate MouseButtonReleased;

    /// <summary>Fired when the mouse wheel has been rotated</summary>
    event MouseWheelDelegate MouseWheelRotated;

    /// <summary>Retrieves the current state of the mouse</summary>
    /// <returns>The current state of the mouse</returns>
    MouseState GetState();
    
    /// <summary>Moves the mouse cursor to the specified location</summary>
    /// <param name="x">New X coordinate of the mouse cursor</param>
    /// <param name="y">New Y coordinate of the mouse cursor</param>
    void MoveTo(float x, float y);

#if false // Hard to implement - how to filter out the specific movement?
    /// <summary>
    ///   Moves the mouse cursor to the specified location and optionally
    ///   suppresses the MouseMoved event for this position adjustment
    /// </summary>
    /// <param name="x">New X coordinate of the mouse cursor</param>
    /// <param name="y">New Y coordinate of the mouse cursor</param>
    /// <param name="suppressMoveEvent">
    ///   Whether to suppress the MouseMoved event.
    /// </param>
    /// <remarks>
    ///   Supressing the MouseMoved event when repositioning the mouse cursor
    ///   is useful for shooters and similar games where relative mouse input
    ///   is required. These games typically reset the mouse cursor to the center
    ///   of the screen or window after each update so the mouse can be moved
    ///   endlessly in any direction. By supressing the MouseMoved event you
    ///   do not have to add special logic to the MouseMoved subscribers.
    /// </remarks>
    void MoveTo(float x, float y, bool suppressMoveEvent);
#endif

  }

} // namespace Nuclex.Input.Devices
