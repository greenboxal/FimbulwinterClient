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
using System.Text;
using Microsoft.Xna.Framework;

namespace Nuclex.Input {

  /// <summary>Carries the arguments for the controller detection event</summary>
  public class ControllerEventArgs : EventArgs {

    /// <summary>Initializes a new argument container for keyboard/mouse input</summary>
    public ControllerEventArgs() {
      this.playerIndex = null;
    }

    /// <summary>Initializes a new argument container with a controller index</summary>
    /// <param name="playerIndex">Player whose controller was detected</param>
    public ControllerEventArgs(ExtendedPlayerIndex playerIndex) {
      this.playerIndex = playerIndex;
    }

    /// <summary>Index of the controller on which a button was pressed</summary>
    /// <remarks>
    ///   If this is null, the player pressed a button/key on his mouse/keyboard.
    /// </remarks>
    public ExtendedPlayerIndex? PlayerIndex {
      get { return this.playerIndex; }
    }

    /// <summary>Index of the detected controller</summary>
    private ExtendedPlayerIndex? playerIndex;

  }

} // namespace Nuclex.Input
