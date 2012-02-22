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

  /// <summary>Interfaces with an XBox 360 controller via XNA (XINPUT)</summary>
  public abstract class GamePad : IGamePad {

    /// <summary>Called when one or more buttons on the game pad have been pressed</summary>
    public event GamePadButtonDelegate ButtonPressed;
    /// <summary>Called when one or more buttons on the game pad have been released</summary>
    public event GamePadButtonDelegate ButtonReleased;

    /// <summary>Called when one or more buttons on the game pad have been pressed</summary>
    public event ExtendedGamePadButtonDelegate ExtendedButtonPressed;
    /// <summary>Called when one or more buttons on the game pad have been released</summary>
    public event ExtendedGamePadButtonDelegate ExtendedButtonReleased;

    /// <summary>Retrieves the current state of the game pad</summary>
    /// <returns>The current state of the game pad</returns>
    public abstract GamePadState GetState();

    /// <summary>Retrieves the current DirectInput joystick state</summary>
    /// <returns>The current state of the DirectInput joystick</returns>
    public abstract ExtendedGamePadState GetExtendedState();

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

    /// <summary>Whether subscribers to the standard button events exist</summary>
    protected bool HaveEventSubscribers {
      get { return (ButtonPressed != null) || (ButtonReleased != null); }
    }

    /// <summary>Whether subscribers to the extended button events exist</summary>
    protected bool HaveExtendedEventSubscribers {
      get { return (ExtendedButtonPressed != null) || (ExtendedButtonReleased != null); }
    }

    /// <summary>Fires the ButtonPressed event</summary>
    /// <param name="buttons">Buttons that have been pressed</param>
    protected void OnButtonPressed(Buttons buttons) {
      if (ButtonPressed != null) {
        ButtonPressed(buttons);
      }
    }

    /// <summary>Fires the ButtonReleased event</summary>
    /// <param name="buttons">Buttons that have been released</param>
    protected void OnButtonReleased(Buttons buttons) {
      if (ButtonReleased != null) {
        ButtonReleased(buttons);
      }
    }

    /// <summary>Fires the ExtendedButtonPressed event</summary>
    /// <param name="buttons1">Button or buttons that have been pressed or released</param>
    /// <param name="buttons2">Button or buttons that have been pressed or released</param>
    protected void OnExtendedButtonPressed(ulong buttons1, ulong buttons2) {
      if (ExtendedButtonPressed != null) {
        ExtendedButtonPressed(buttons1, buttons2);
      }
    }

    /// <summary>Fires the ButtonReleased event</summary>
    /// <param name="buttons1">Button or buttons that have been pressed or released</param>
    /// <param name="buttons2">Button or buttons that have been pressed or released</param>
    protected void OnExtendedButtonReleased(ulong buttons1, ulong buttons2) {
      if (ExtendedButtonReleased != null) {
        ExtendedButtonReleased(buttons1, buttons2);
      }
    }

  }

} // namespace Nuclex.Input.Devices
