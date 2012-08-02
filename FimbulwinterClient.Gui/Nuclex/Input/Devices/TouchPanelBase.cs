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

  /// <summary>Common functionality for the touch input panel</summary>
  public abstract class TouchPanelBase : ITouchPanel {

    /// <summary>Triggered when the user presses on the screen</summary>
    public event TouchDelegate Pressed;
    /// <summary>Triggered when the user moves his touch on the screen</summary>
    public event TouchDelegate Released;
    /// <summary>Triggered when the user releases the screen again</summary>
    public event TouchDelegate Moved;

    /// <summary>Maximum number of simultaneous touches the panel supports</summary>
    public abstract int MaximumTouchCount { get; }

    /// <summary>Retrieves the current state of the touch panel</summary>
    /// <returns>The current state of the touch panel</returns>
    public abstract TouchState GetState();

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
    public abstract void Update();

    /// <summary>Takes a snapshot of the current state of the input device</summary>
    /// <remarks>
    ///   This snapshot will be queued until the user calls the Update() method,
    ///   where the next polled snapshot will be taken from the queue and provided
    ///   to the user.
    /// </remarks>
    public abstract void TakeSnapshot();

    /// <summary>Fires the Pressed event when the touch screen received a press</summary>
    /// <param name="id">ID of the touch</param>
    /// <param name="position">Position the user is touching the screen at</param>
    protected void OnPressed(int id, ref Vector2 position) {
      if (Pressed != null) {
        Pressed(id, position);
      }
    }

    /// <summary>Fires the Moved event when the user moved on the touch screen</summary>
    /// <param name="id">ID of the touch</param>
    /// <param name="position">Position the user has moved his touch to</param>
    protected void OnMoved(int id, ref Vector2 position) {
      if (Moved != null) {
        Moved(id, position);
      }
    }

    /// <summary>Fires the Released event when the user released the touch screen</summary>
    /// <param name="id">ID of the touch</param>
    /// <param name="position">Position at which the user has released the screen</param>
    protected void OnReleased(int id, ref Vector2 position) {
      if (Released != null) {
        Released(id, position);
      }
    }

    /// <summary>Checks two touch states for changes an generates events</summary>
    /// <param name="previous">Previous touch state that will be compared</param>
    /// <param name="touchState">New touch state events will be generated for</param>
    protected void GenerateEvents(ref TouchState previous, ref TouchState touchState) {
      for (int index = 0; index < touchState.Touches.Count; ++index) {
        switch (touchState.Touches[index].State) {
          case TouchLocationState.Moved: {
            Moved(touchState.Touches[index].Id, touchState.Touches[index].Position);
            break;
          }
          case TouchLocationState.Pressed: {
            Pressed(touchState.Touches[index].Id, touchState.Touches[index].Position);
            break;
          }
          case TouchLocationState.Released: {
            Released(touchState.Touches[index].Id, touchState.Touches[index].Position);
            break;
          }
        }
      }
    }

  }

} // namespace Nuclex.Input.Devices
