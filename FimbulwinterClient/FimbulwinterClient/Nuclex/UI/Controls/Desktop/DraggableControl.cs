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
using System.Diagnostics;

using Microsoft.Xna.Framework.Input;

using Nuclex.Input;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls.Desktop {

  // Always move in absolute (offset) coordinates?
  // Or always move in fractional coordinates?
  //
  // Preferring b), because I restores the user's display to the exact
  // state it was if the resolution is changed, including that fact that
  // lower resolutions would cause the windows to go off-screen.
  //
  // However, b) would mean a call to GetAbsolutePosition() each frame.
  // Which isn't so bad, but... avoidable with a)

  // Properties:
  //   Boundaries (for constraining a control to a region)
  //   Moveable (turn moveability on or off)

  /// <summary>Control the user can drag around with the mouse</summary>
  public abstract class DraggableControl : Control {

    /// <summary>Initializes a new draggable control</summary>
    public DraggableControl() {
      EnableDragging = true;
    }

    /// <summary>Initializes a new draggable control</summary>
    /// <param name="canGetFocus">Whether the control can obtain the input focus</param>
    public DraggableControl(bool canGetFocus) :
      base(canGetFocus) {
      EnableDragging = true;
    }

    /// <summary>Called when the mouse position is updated</summary>
    /// <param name="x">X coordinate of the mouse cursor on the GUI</param>
    /// <param name="y">Y coordinate of the mouse cursor on the GUI</param>
    protected override void OnMouseMoved(float x, float y) {
      if(this.beingDragged) {

        // Adjust the control's position within the container
        this.Bounds.Location.X.Offset += x - this.pickupX;
        this.Bounds.Location.Y.Offset += y - this.pickupY;

      } else {

        // Remember the current mouse position so we know where the user picked
        // up the control when a drag operation begins
        this.pickupX = x;
        this.pickupY = y;

      }
    }

    /// <summary>Called when a mouse button has been pressed down</summary>
    /// <param name="button">Index of the button that has been pressed</param>
    protected override void OnMousePressed(MouseButtons button)
    {
        if (button == MouseButtons.Left)
        {
        this.beingDragged = this.enableDragging;
      }
    }

    /// <summary>Called when a mouse button has been released again</summary>
    /// <param name="button">Index of the button that has been released</param>
    protected override void OnMouseReleased(MouseButtons button)
    {
        if (button == MouseButtons.Left)
        {
        this.beingDragged = false;
      }
    }

    /// <summary>Whether the control can be dragged with the mouse</summary>
    protected bool EnableDragging {
      get { return this.enableDragging; }
      set {
        this.enableDragging = value;
        this.beingDragged &= value;
      }
    }

    /// <summary>Whether the control can be dragged</summary>
    private bool enableDragging;

    /// <summary>Whether the control is currently being dragged</summary>
    private bool beingDragged;
    /// <summary>X coordinate at which the control was picked up</summary>
    private float pickupX;
    /// <summary>Y coordinate at which the control was picked up</summary>
    private float pickupY;

  }

} // namespace Nuclex.UserInterface.Controls.Desktop
