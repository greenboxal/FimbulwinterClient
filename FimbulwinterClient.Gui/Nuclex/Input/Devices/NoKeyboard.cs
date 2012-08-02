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
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Nuclex.Input.Devices {

  /// <summary>Dummy that takes the place of unfilled keyboard slots</summary>
  internal partial class NoKeyboard : IKeyboard {

    /// <summary>Fired when a key has been pressed</summary>
    public event KeyDelegate KeyPressed { add { } remove { } }

    /// <summary>Fired when a key has been released</summary>
    public event KeyDelegate KeyReleased { add { } remove { } }

    /// <summary>Fired when the user has entered a character</summary>
    /// <remarks>
    ///   This provides the complete, translated character the user has entered.
    ///   Handling of international keyboard layouts, shift key, accents and
    ///   other special cases is done by Windows according to the current users'
    ///   country and selected keyboard layout.
    /// </remarks>
    public event CharacterDelegate CharacterEntered { add { } remove { } }

    /// <summary>Initializes a new keyboard dummy</summary>
    public NoKeyboard() { }

    /// <summary>Retrieves the current state of the keyboard</summary>
    /// <returns>The current state of the keyboard</returns>
    public KeyboardState GetState() { return new KeyboardState(); }

    /// <summary>Whether the input device is connected to the system</summary>
    public bool IsAttached {
      get { return false; }
    }

    /// <summary>Human-readable name of the input device</summary>
    public string Name {
      get { return "No keyboard attached"; }
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

  }

} // namespace Nuclex.Input.Devices
