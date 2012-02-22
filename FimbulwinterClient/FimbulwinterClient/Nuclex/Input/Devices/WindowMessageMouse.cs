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
using System.Runtime.InteropServices;

#if WINDOWS

namespace Nuclex.Input.Devices {

  /// <summary>Interfaces with a PC mouse via window messages</summary>
  internal class WindowMessageMouse : BufferedMouse, IDisposable {

    /// <summary>Initializes a new window message based mouse</summary>
    /// <param name="messageSource">Source the window messages are obtained from</param>
    public WindowMessageMouse(IMouseMessageSource messageSource) {
      this.buttonPressedDelegate = new MouseButtonEventDelegate(BufferButtonPress);
      this.buttonReleasedDelegate = new MouseButtonEventDelegate(BufferButtonRelease);
      this.wheelRotatedDelegate = new MouseWheelEventDelegate(BufferWheelRotation);
      this.cursorMovedDelegate = new MouseMoveEventDelegate(BufferCursorMovement);

      this.messageSource = messageSource;
      this.messageSource.MouseButtonPressed += this.buttonPressedDelegate;
      this.messageSource.MouseButtonReleased += this.buttonReleasedDelegate;
      this.messageSource.MouseWheelRotated += this.wheelRotatedDelegate;
      this.messageSource.MouseMoved += this.cursorMovedDelegate;
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if (this.messageSource != null) {
        this.messageSource.MouseMoved -= this.cursorMovedDelegate;
        this.messageSource.MouseWheelRotated -= this.wheelRotatedDelegate;
        this.messageSource.MouseButtonReleased -= this.buttonReleasedDelegate;
        this.messageSource.MouseButtonPressed -= this.buttonPressedDelegate;

        this.messageSource = null;
      }
    }

    /// <summary>Moves the mouse cursor to the specified location</summary>
    /// <param name="x">New X coordinate of the mouse cursor</param>
    /// <param name="y">New Y coordinate of the mouse cursor</param>
    public override void MoveTo(float x, float y) {
      Mouse.SetPosition((int)x, (int)y);
    }

    /// <summary>Whether the input device is connected to the system</summary>
    public override bool IsAttached {
      get { return true; }
    }

    /// <summary>Human-readable name of the input device</summary>
    public override string Name {
      get { return "PC Mouse"; }
    }

    /// <summary>Delegate for the buttonPressed() method</summary>
    private MouseButtonEventDelegate buttonPressedDelegate;
    /// <summary>Delegate for the buttonReleased() method</summary>
    private MouseButtonEventDelegate buttonReleasedDelegate;
    /// <summary>Delegate for the wheelRotated() method</summary>
    private MouseWheelEventDelegate wheelRotatedDelegate;
    /// <summary>Delegate for the cursorMoved() method</summary>
    private MouseMoveEventDelegate cursorMovedDelegate;

    /// <summary>Window message source the instance is currently subscribed to</summary>
    private IMouseMessageSource messageSource;

  }

} // namespace Nuclex.Input.Devices

#endif // WINDOWS
