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

  /// <summary>
  ///   Interface for controls that can be written into using the keyboard
  /// </summary>
  public interface IWritable : IFocusable {

    /// <summary>Title to be displayed in the on-screen keyboard</summary>
    string GuideTitle { get; }

    /// <summary>Description to be displayed in the on-screen keyboard</summary>
    string GuideDescription { get; }

    /// <summary>Text currently contained in the control</summary>
    /// <remarks>
    ///   Called before the on-screen keyboard is displayed to get the text currently
    ///   contained in the control and after the on-screen keyboard has been
    ///   acknowledged to assign the edited text to the control
    /// </remarks>
    string Text { get; set; }

    /// <summary>Called when the user has entered a character</summary>
    /// <param name="character">Character that has been entered</param>
    void OnCharacterEntered(char character);

  }

} // namespace Nuclex.UserInterface.Controls
