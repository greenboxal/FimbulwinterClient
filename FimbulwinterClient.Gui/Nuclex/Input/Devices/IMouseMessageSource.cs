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

#if WINDOWS

namespace Nuclex.Input.Devices {

  /// <summary>Delegate used to report a mouse event</summary>
  /// <param name="button">Button that has been pressed or released</param>
  internal delegate void MouseButtonEventDelegate(MouseButtons button);

  /// <summary>Delegate used to report a mouse event</summary>
  /// <param name="ticks">Number of ticks the mouse wheel was rotated</param>
  internal delegate void MouseWheelEventDelegate(float ticks);

  /// <summary>Delegate used to report a mouse event</summary>
  /// <param name="x">X coordinate of the mouse cursor</param>
  /// <param name="y">Y coordinate or the mouse cursor</param>
  internal delegate void MouseMoveEventDelegate(float x, float y);

  /// <summary>
  ///   Sends out notifications for intercepted window messages related to the mouse
  /// </summary>
  internal interface IMouseMessageSource {

    /// <summary>Triggered when a mouse button has been pressed</summary>
    event MouseButtonEventDelegate MouseButtonPressed;

    /// <summary>Triggered when a mouse button has been released</summary>
    event MouseButtonEventDelegate MouseButtonReleased;

    /// <summary>Triggered when the mouse has been moved</summary>
    event MouseMoveEventDelegate MouseMoved;

    /// <summary>Triggered when the mouse wheel has been rotated</summary>
    event MouseWheelEventDelegate MouseWheelRotated;

  }

} // namespace Nuclex.Input.Devices

#endif // WINDOWS
