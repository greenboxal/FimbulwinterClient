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

#if WINDOWS_PHONE

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;

namespace Nuclex.Input.Devices {

  /// <summary>XNA-based touch input panel for Windows Phone 7 devices</summary>
  internal class XnaTouchPanel : TouchPanelBase {

    /// <summary>Initializes a new touch panel based on the XNA framework</summary>
    public XnaTouchPanel() {
      this.states = new Queue<TouchState>();
    }

    /// <summary>Maximum number of simultaneous touches the panel supports</summary>
    public override int MaximumTouchCount {
      get { return TouchPanel.GetCapabilities().MaximumTouchCount; }
    }

    /// <summary>Retrieves the current state of the touch panel</summary>
    /// <returns>The current state of the touch panel</returns>
    public override TouchState GetState() {
      return this.current;
    }

    /// <summary>Whether the input device is connected to the system</summary>
    public override bool IsAttached {
      get {
        return true; // Is there a WP7 device without touch screen?
        // return TouchPanel.GetCapabilities().IsConnected;
      }
    }

    /// <summary>Human-readable name of the input device</summary>
    public override string Name {
      get { return "Windows Phone 7 Touch Panel"; }
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
        this.current = new TouchState(IsAttached, TouchPanel.GetState());
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
      this.states.Enqueue(new TouchState(IsAttached, TouchPanel.GetState()));
    }

    /// <summary>Snapshots of the touch panel state waiting to be processed</summary>
    private Queue<TouchState> states;
    /// <summary>Currently published game pad state</summary>
    private TouchState current;

  }

} // namespace Nuclex.Input.Devices

#endif // WINDOWS_PHONE
