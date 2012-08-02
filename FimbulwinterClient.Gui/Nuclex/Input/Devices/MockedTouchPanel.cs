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

#if WINDOWS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input.Touch;

namespace Nuclex.Input.Devices {

  /// <summary>Code-controllable touch panel for unit testing</summary>
  public class MockedTouchPanel : TouchPanelBase {

    /// <summary>Initializes a new mocked touch panel</summary>
    public MockedTouchPanel() {
      this.states = new Queue<TouchState>();
    }

    /// <summary>Whether the input device is connected to the system</summary>
    public override bool IsAttached {
      get { return this.isAttached; }
    }

    /// <summary>Human-readable name of the input device</summary>
    public override string Name {
      get { return "Mocked touch panel"; }
    }

    /// <summary>Retrieves the current state of the touch panel</summary>
    /// <returns>The current state of the touch panel</returns>
    public override TouchState GetState() {
      return this.current;
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
    public override void Update() {
      TouchState previous = this.current;

      if (this.states.Count == 0) {
        this.current = buildState();
      } else {
        this.current = this.states.Dequeue();
      }

      GenerateEvents(ref previous, ref this.current);
    }

    /// <summary>Takes a snapshot of the current state of the input device</summary>
    /// <remarks>
    ///   This snapshot will be queued until the user calls the Update() method,
    ///   where the next polled snapshot will be taken from the queue and provided
    ///   to the user.
    /// </remarks>
    public override void TakeSnapshot() {
      this.states.Enqueue(buildState());
    }

    /// <summary>Maximum number of simultaneous touches the panel supports</summary>
    public override int MaximumTouchCount {
      get { return TouchPanel.GetCapabilities().MaximumTouchCount; }
    }

    /// <summary>Attaches (connects) the game pad</summary>
    public void Attach() {
      this.isAttached = true;
    }

    /// <summary>Detaches (disconnects) the game pad</summary>
    public void Detach() {
      this.isAttached = false;
    }

    /// <summary>Presses down on the touch panel at the specified location</summary>
    /// <param name="id">ID this touch can be tracked with</param>
    /// <param name="x">X coordinate at which the touch panel was pressed</param>
    /// <param name="y">Y coordinate at which the touch panel was pressed</param>
    public void Press(int id, float x, float y) {
      TouchCollectionHelper.AddTouchLocation(
        ref this.touchCollection,
        id,
        TouchLocationState.Pressed,
        x,
        y,
        TouchLocationState.Invalid,
        -1.0f,
        -1.0f
      );
    }

    /// <summary>Releases the touch with the specified id</summary>
    /// <param name="id">Id of the touch that will be released</param>
    public void Release(int id) {
      TouchCollectionHelper.AddTouchLocation(
        ref this.touchCollection,
        id,
        TouchLocationState.Released,
        -1.0f,
        -1.0f,
        TouchLocationState.Invalid,
        -1.0f,
        -1.0f
      );
    }

    /// <summary>Moves an existing touch to a different location</summary>
    /// <param name="id">Id of the touch that will be moved</param>
    /// <param name="x">New X coordinate the touch moved to</param>
    /// <param name="y">New Y coordinate the touch moved to</param>
    public void Move(int id, float x, float y) {
      TouchCollectionHelper.AddTouchLocation(
        ref this.touchCollection,
        id,
        TouchLocationState.Moved,
        x,
        y,
        TouchLocationState.Invalid,
        -1.0f,
        -1.0f
      );
    }

    /// <summary>Builds a touch state from the current touches</summary>
    /// <returns>The currently mocked touches in a touch state</returns>
    private TouchState buildState() {
      var state = new TouchState(this.isAttached, this.touchCollection);
      TouchCollectionHelper.Clear(ref this.touchCollection);
      return state;
    }

    /// <summary>Snapshots of the touch panel state waiting to be processed</summary>
    private Queue<TouchState> states;

    /// <summary>Currently published touch panel state</summary>
    private TouchState current;

    /// <summary>Whether the touch panel is currently attached</summary>
    private bool isAttached;

    /// <summary>Collection of touch events for the next state being built</summary>
    private TouchCollection touchCollection;

  }

} // namespace Nuclex.Input.Devices

#endif // WINDOWS
