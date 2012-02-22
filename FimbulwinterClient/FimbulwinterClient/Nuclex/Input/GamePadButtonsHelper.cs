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

namespace Nuclex.Input {

  /// <summary>Contains helper methods for the GamePadButtons enumeration</summary>
  public static class GamePadButtonsHelper {

    /// <summary>Checks whether buttons are contained in a button flag field</summary>
    /// <param name="buttons">Button flag field that will be checked</param>
    /// <param name="button">Buttons for which the method will check</param>
    /// <returns>True if all specified buttons appear in the flag field</returns>
#if NET_40
    public static bool Contains(this Buttons buttons, Buttons button) {
#else
    public static bool Contains(Buttons buttons, Buttons button) {
#endif
      return ((buttons & button) != 0);
    }

  }

} // namespace Nuclex.Input
