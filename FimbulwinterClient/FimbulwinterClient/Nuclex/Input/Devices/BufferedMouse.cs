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

  /// <summary>Mouse that buffers mouse events until Update() is called</summary>
  public abstract class BufferedMouse : IMouse {

    #region enum EventType

    /// <summary>Types of event the buffered mouse can queue</summary>
    private enum EventType {
      /// <summary>A button has been pressed</summary>
      ButtonPress,
      /// <summary>A button has been released</summary>
      ButtonRelease,
      /// <summary>The mouse wheel has been rotated</summary>
      WheelRotation,
      /// <summary>The mouse cursor has been moved</summary>
      Movement,
      /// <summary>A snapshot was taken at this point in time</summary>
      Snapshot
    }

    #endregion // enum EventType

    #region struct MouseEvent

    /// <summary>Stores the properties of a mouse event</summary>
    [StructLayout(LayoutKind.Explicit)]
    private struct MouseEvent {

      /// <summary>Creates a new mouse event for a button press</summary>
      /// <param name="button">Button that has been pressed</param>
      /// <returns>The new mouse event</returns>
      public static MouseEvent ButtonPress(MouseButtons button) {
        var mouseEvent = new MouseEvent();
        mouseEvent.EventType = EventType.ButtonPress;
        mouseEvent.Button = button;
        return mouseEvent;
      }

      /// <summary>Creates a new mouse event for a button release</summary>
      /// <param name="button">Button that has been released</param>
      /// <returns>The new mouse event</returns>
      public static MouseEvent ButtonRelease(MouseButtons button) {
        var mouseEvent = new MouseEvent();
        mouseEvent.EventType = EventType.ButtonRelease;
        mouseEvent.Button = button;
        return mouseEvent;
      }

      /// <summary>Creates a new mouse event for a mouse wheel rotation</summary>
      /// <param name="ticks">Number of ticks wheel has been rotated</param>
      /// <returns>The new mouse event</returns>
      public static MouseEvent WheelRotation(float ticks) {
        var mouseEvent = new MouseEvent();
        mouseEvent.EventType = EventType.WheelRotation;
        mouseEvent.Ticks = ticks;
        return mouseEvent;
      }

      /// <summary>Creates a new mouse event for a mouse cursor movement</summary>
      /// <param name="x">New X coordinate of the mouse cursor</param>
      /// <param name="y">New Y coordinate of the mouse cursor</param>
      /// <returns>The new mouse event</returns>
      public static MouseEvent Movement(float x, float y) {
        var mouseEvent = new MouseEvent();
        mouseEvent.EventType = EventType.Movement;
        mouseEvent.X = x;
        mouseEvent.Y = y;
        return mouseEvent;
      }

      /// <summary>Creates a new mouse event for a snapshot point</summary>
      /// <returns>The new mouse event</returns>
      public static MouseEvent Snapshot() {
        var mouseEvent = new MouseEvent();
        mouseEvent.EventType = EventType.Snapshot;
        return mouseEvent;
      }

      /// <summary>Type of the event</summary>
      [FieldOffset(0)]
      public EventType EventType;
      /// <summary>Mouse buttons that were pressed or released</summary>
      [FieldOffset(4)]
      public MouseButtons Button;
      /// <summary>Number of ticks the mouse wheel was rotated</summary>
      [FieldOffset(4)]
      public float Ticks;
      /// <summary>New X coordinate of the mouse</summary>
      [FieldOffset(4)]
      public float X;
      /// <summary>New Y coordinate of the mouse</summary>
      [FieldOffset(8)]
      public float Y;

    }

    #endregion // struct MouseEvent

    /// <summary>Fired when the mouse has been moved</summary>
    public event MouseMoveDelegate MouseMoved;

    /// <summary>Fired when one or more mouse buttons have been pressed</summary>
    public event MouseButtonDelegate MouseButtonPressed;

    /// <summary>Fired when one or more mouse buttons have been released</summary>
    public event MouseButtonDelegate MouseButtonReleased;

    /// <summary>Fired when the mouse wheel has been rotated</summary>
    public event MouseWheelDelegate MouseWheelRotated;

    /// <summary>Initializes a new buffered mouse device</summary>
    public BufferedMouse() {
      this.queuedEvents = new Queue<MouseEvent>();
    }

    /// <summary>Retrieves the current state of the mouse</summary>
    /// <returns>The current state of the mouse</returns>
    public MouseState GetState() {
      bool leftButtonPressed = (this.pressedButtons & MouseButtons.Left) != 0;
      bool middleButtonPressed = (this.pressedButtons & MouseButtons.Middle) != 0;
      bool rightButtonPressed = (this.pressedButtons & MouseButtons.Right) != 0;
      bool x1ButtonPressed = (this.pressedButtons & MouseButtons.X1) != 0;
      bool x2ButtonPressed = (this.pressedButtons & MouseButtons.X2) != 0;

      return new MouseState(
        (int)this.x, (int)this.y,
        (int)(this.wheelPosition * 120.0f),
        leftButtonPressed ? ButtonState.Pressed : ButtonState.Released,
        middleButtonPressed ? ButtonState.Pressed : ButtonState.Released,
        rightButtonPressed ? ButtonState.Pressed : ButtonState.Released,
        x1ButtonPressed ? ButtonState.Pressed : ButtonState.Released,
        x2ButtonPressed ? ButtonState.Pressed : ButtonState.Released
      );
    }

    /// <summary>Moves the mouse cursor to the specified location</summary>
    /// <param name="x">New X coordinate of the mouse cursor</param>
    /// <param name="y">New Y coordinate of the mouse cursor</param>
    public abstract void MoveTo(float x, float y);

    /// <summary>Whether the input device is connected to the system</summary>
    public abstract bool IsAttached { get; }

    /// <summary>Human-readable name of the input device</summary>
    public abstract string Name { get; }

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
    public void Update() {
      while (this.queuedEvents.Count > 0) {
        MouseEvent nextEvent = this.queuedEvents.Dequeue();
        switch (nextEvent.EventType) {
          case EventType.ButtonRelease: {
            this.pressedButtons &= (MouseButtons)(~nextEvent.Button);
            OnButtonReleased(nextEvent.Button);
            break;
          }
          case EventType.ButtonPress: {
            this.pressedButtons |= nextEvent.Button;
            OnButtonPressed(nextEvent.Button);
            break;
          }
          case EventType.WheelRotation: {
            this.wheelPosition += nextEvent.Ticks;
            OnWheelRotated(nextEvent.Ticks);
            break;
          }
          case EventType.Movement: {
            this.x = nextEvent.X;
            this.y = nextEvent.Y;
            OnMouseMoved(nextEvent.X, nextEvent.Y);
            break;
          }
          case EventType.Snapshot: {
            return;
          }
        }
      }
    }

    /// <summary>Takes a snapshot of the current state of the input device</summary>
    /// <remarks>
    ///   This snapshot will be queued until the user calls the Update() method,
    ///   where the next polled snapshot will be taken from the queue and provided
    ///   to the user.
    /// </remarks>
    public void TakeSnapshot() {
      this.queuedEvents.Enqueue(MouseEvent.Snapshot());
    }

    /// <summary>Records a mouse button press in the event queue</summary>
    /// <param name="buttons">Buttons that have been pressed</param>
    protected void OnButtonPressed(MouseButtons buttons) {
      if (MouseButtonPressed != null) {
        MouseButtonPressed(buttons);
      }
    }

    /// <summary>Records a mouse button release in the event queue</summary>
    /// <param name="buttons">Buttons that have been released</param>
    protected void OnButtonReleased(MouseButtons buttons) {
      if (MouseButtonReleased != null) {
        MouseButtonReleased(buttons);
      }
    }

    /// <summary>Records a mouse wheel rotation in the event queue</summary>
    /// <param name="ticks">Ticks the mouse wheel has been rotated</param>
    protected void OnWheelRotated(float ticks) {
      if (MouseWheelRotated != null) {
        MouseWheelRotated(ticks);
      }
    }

    /// <summary>Records a mouse cursor movement in the event queue</summary>
    /// <param name="x">X coordinate the mouse cursor has been moved to</param>
    /// <param name="y">Y coordinate the mouse cursor has been moved to</param>
    protected void OnMouseMoved(float x, float y) {
      if (MouseMoved != null) {
        MouseMoved(x, y);
      }
    }

    /// <summary>Records a mouse button press in the event queue</summary>
    /// <param name="buttons">Buttons that have been pressed</param>
    protected void BufferButtonPress(MouseButtons buttons) {
      this.queuedEvents.Enqueue(MouseEvent.ButtonPress(buttons));
    }

    /// <summary>Records a mouse button release in the event queue</summary>
    /// <param name="buttons">Buttons that have been released</param>
    protected void BufferButtonRelease(MouseButtons buttons) {
      this.queuedEvents.Enqueue(MouseEvent.ButtonRelease(buttons));
    }

    /// <summary>Records a mouse wheel rotation in the event queue</summary>
    /// <param name="ticks">Ticks the mouse wheel has been rotated</param>
    protected void BufferWheelRotation(float ticks) {
      this.queuedEvents.Enqueue(MouseEvent.WheelRotation(ticks));
    }

    /// <summary>Records a mouse cursor movement in the event queue</summary>
    /// <param name="x">X coordinate the mouse cursor has been moved to</param>
    /// <param name="y">Y coordinate the mouse cursor has been moved to</param>
    protected void BufferCursorMovement(float x, float y) {
      this.queuedEvents.Enqueue(MouseEvent.Movement(x, y));
    }

    /// <summary>Mouse buttons currently pressed</summary>
    private MouseButtons pressedButtons;
    /// <summary>Cumulative position of the mouse wheel</summary>
    private float wheelPosition;
    /// <summary>X coordinate of the mouse cursor</summary>
    private float x;
    /// <summary>Y coordinate of the mouse cursor</summary>
    private float y;

    /// <summary>Queued mouse events</summary>
    private Queue<MouseEvent> queuedEvents;

  }

} // namespace Nuclex.Input.Devices

#endif // WINDOWS
