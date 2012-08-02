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

#if WINDOWS

namespace Nuclex.Input.Devices {

  /// <summary>Code-controllable mouse for unit testing</summary>
  public class MockedMouse : BufferedMouse {

    /// <summary>Initializes a new window message based mouse</summary>
    public MockedMouse() { }

    /// <summary>Moves the mouse cursor to the specified location</summary>
    /// <param name="x">X coordinate the mouse cursor will be moved to</param>
    /// <param name="y">Y coordinate the mouse cursor will be moved to</param>
    public override void MoveTo(float x, float y) {
      BufferCursorMovement(x, y);
    }

    /// <summary>Whether the input device is connected to the system</summary>
    public override bool IsAttached {
      get { return this.isAttached; }
    }

    /// <summary>Human-readable name of the input device</summary>
    public override string Name {
      get { return "Mocked mouse"; }
    }

    /// <summary>Presses a button on the mouse</summary>
    /// <param name="button">Button that will be pressed</param>
    public void Press(MouseButtons button) {
      BufferButtonPress(button);
    }

    /// <summary>Buffers a button release on the mouse</summary>
    /// <param name="button">Button that will be released</param>
    public void Release(MouseButtons button) {
      BufferButtonRelease(button);
    }

    /// <summary>Rotates the mouse wheel by the specified number of ticks</summary>
    /// <param name="ticks">Number of ticks the mouse wheel will be rotated</param>
    public void RotateWheel(float ticks) {
      BufferWheelRotation(ticks);
    }

    /// <summary>Attaches (connects) the game pad</summary>
    public void Attach() {
      this.isAttached = true;
    }

    /// <summary>Detaches (disconnects) the game pad</summary>
    public void Detach() {
      this.isAttached = false;
    }

    /// <summary>Whether the game pad is attached</summary>
    private bool isAttached;

  }

} // namespace Nuclex.Input.Devices

#endif // WINDOWS
