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

using Microsoft.Xna.Framework;

namespace Nuclex.Input {

  /// <summary>Player index enumeration with slots for 8 players</summary>
  public enum ExtendedPlayerIndex {

    /// <summary>First player</summary>
    One = PlayerIndex.One,
    /// <summary>Second player</summary>
    Two = PlayerIndex.Two,
    /// <summary>Third player</summary>
    Three = PlayerIndex.Three,
    /// <summary>Fourth player</summary>
    Four = PlayerIndex.Four,
    /// <summary>Fifth player</summary>
    Five,
    /// <summary>Sixth player</summary>
    Six,
    /// <summary>Seventh player</summary>
    Seven,
    /// <summary>Eigth player</summary>
    Eight

  }

} // namespace Nuclex.Input
