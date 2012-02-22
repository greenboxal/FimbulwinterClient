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

namespace Nuclex.Input.Devices {

  /// <summary>Delegate use to report presses and releases of game pad buttons</summary>
  /// <param name="buttons">Button or buttons that have been pressed or released</param>
  public delegate void GamePadButtonDelegate(Buttons buttons);

  /// <summary>Delegate use to report presses and releases of game pad buttons</summary>
  /// <param name="buttons1">Button or buttons that have been pressed or released</param>
  /// <param name="buttons2">Button or buttons that have been pressed or released</param>
  public delegate void ExtendedGamePadButtonDelegate(ulong buttons1, ulong buttons2);

  /// <summary>Specialized input device for game pad-like controllers</summary>
  public interface IGamePad : IInputDevice {

    /// <summary>Called when one or more buttons on the game pad have been pressed</summary>
    event GamePadButtonDelegate ButtonPressed;
    /// <summary>Called when one or more buttons on the game pad have been released</summary>
    event GamePadButtonDelegate ButtonReleased;

    /// <summary>Called when one or more buttons on the game pad have been pressed</summary>
    event ExtendedGamePadButtonDelegate ExtendedButtonPressed;
    /// <summary>Called when one or more buttons on the game pad have been released</summary>
    event ExtendedGamePadButtonDelegate ExtendedButtonReleased;

    /// <summary>Retrieves the current state of the game pad</summary>
    /// <returns>The current state of the game pad</returns>
    GamePadState GetState();

    /// <summary>Retrieves the current DirectInput joystick state</summary>
    /// <returns>The current state of the DirectInput joystick</returns>
    ExtendedGamePadState GetExtendedState();

  }

} // namespace Nuclex.Input.Devices
