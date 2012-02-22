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
using Microsoft.Xna.Framework;

namespace Nuclex.Input.Devices {

  /// <summary>Dummy that takes the place of unfilled player slots</summary>
  internal class NoGamePad : IGamePad {

    /// <summary>Called when one or more buttons on the game pad have been pressed</summary>
    public event GamePadButtonDelegate ButtonPressed { add { } remove { } }
    /// <summary>Called when one or more buttons on the game pad have been released</summary>
    public event GamePadButtonDelegate ButtonReleased { add { } remove { } }

    /// <summary>Called when one or more buttons on the game pad have been pressed</summary>
    public event ExtendedGamePadButtonDelegate ExtendedButtonPressed { add { } remove { } }
    /// <summary>Called when one or more buttons on the game pad have been released</summary>
    public event ExtendedGamePadButtonDelegate ExtendedButtonReleased { add { } remove { } }

    /// <summary>Initializes a new game pad dummy</summary>
    public NoGamePad() {
      this.axes = new float[24];
      this.sliders = new float[8];
      this.buttons = new bool[128];
      this.povs = new int[4] { -1, -1, -1, -1 };
    }

    /// <summary>Retrieves the current state of the game pad</summary>
    /// <returns>The current state of the game pad</returns>
    public GamePadState GetState() {
      return new GamePadState();
    }

    /// <summary>Retrieves the current DirectInput joystick state</summary>
    /// <returns>The current state of the DirectInput joystick</returns>
    public ExtendedGamePadState GetExtendedState() {
      return new ExtendedGamePadState(
        0, this.axes,
        0, this.sliders,
        0, this.buttons,
        0, this.povs
     );
    }

    /// <summary>Whether the input device is connected to the system</summary>
    public bool IsAttached {
      get { return false; }
    }

    /// <summary>Human-readable name of the input device</summary>
    public string Name {
      get { return "No game pad installed"; }
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
    public void Update() { }

    /// <summary>Takes a snapshot of the current state of the input device</summary>
    /// <remarks>
    ///   This snapshot will be queued until the user calls the Update() method,
    ///   where the next polled snapshot will be taken from the queue and provided
    ///   to the user.
    /// </remarks>
    public void TakeSnapshot() { }

    /// <summary>Neutral axis states for the extended game pad state</summary>
    private float[/*24*/] axes;
    /// <summary>Neutral slider states for the extended game pad state</summary>
    private float[/*8*/] sliders;
    /// <summary>Neutral button states for the extended game pad state</summary>
    private bool[/*128*/] buttons;
    /// <summary>Neutral PoV controller states for the extended game pad state</summary>
    private int[/*4*/] povs;

  }

} // namespace Nuclex.Input.Devices
