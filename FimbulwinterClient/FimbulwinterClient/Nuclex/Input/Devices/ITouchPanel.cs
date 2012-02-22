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
using Microsoft.Xna.Framework;

namespace Nuclex.Input.Devices {

  /// <summary>Delegate used to report touch actions</summary>
  /// <param name="id">ID of the distinct touch</param>
  /// <param name="position">Position the action occurred at</param>
  public delegate void TouchDelegate(int id, Vector2 position);

  /// <summary>Specializd input devices for mouse-like controllers</summary>
  public interface ITouchPanel : IInputDevice {

    /// <summary>Triggered when the user presses on the screen</summary>
    event TouchDelegate Pressed;

    /// <summary>Triggered when the user moves his touch on the screen</summary>
    event TouchDelegate Moved;

    /// <summary>Triggered when the user releases the screen again</summary>
    event TouchDelegate Released;

    /// <summary>Maximum number of simultaneous touches the panel supports</summary>
    int MaximumTouchCount { get; }

    /// <summary>Retrieves the current state of the touch panel</summary>
    /// <returns>The current state of the touch panel</returns>
    TouchState GetState();

  }

} // namespace Nuclex.Input.Devices
