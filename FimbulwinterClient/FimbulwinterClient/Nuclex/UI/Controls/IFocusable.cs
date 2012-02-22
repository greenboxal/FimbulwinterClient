#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nuclex.Support.Collections;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls {

  /// <summary>Interface for controls which can obtain the input focus</summary>
  /// <remarks>
  ///   Implement this interface in any control which can obtain the input focus.
  /// </remarks>
  public interface IFocusable {
  
    /// <summary>
    ///   Whether the control can currently obtain the input focus
    /// </summary>
    /// <remarks>
    ///   Usually returns true. For controls that can be disabled to disallow user
    ///   interaction, false can be returned to prevent the control from being
    ///   traversed when the user presses the tab key or uses the cursor / game pad
    ///   to select a control.
    /// </remarks>
    bool CanGetFocus { get; }

  }

} // namespace Nuclex.UserInterface.Controls
