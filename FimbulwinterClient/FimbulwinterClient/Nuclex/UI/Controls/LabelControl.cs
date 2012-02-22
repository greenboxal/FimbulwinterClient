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
using System.Collections.Generic;

namespace Nuclex.UserInterface.Controls {

  /// <summary>Control that draws a block of text</summary>
  public class LabelControl : Control {

    /// <summary>Initializes a new label control with an empty string</summary>
    public LabelControl() : this(string.Empty) { }

    /// <summary>Initializes a new label control</summary>
    /// <param name="text">Text to be printed at the location of the label control</param>
    public LabelControl(string text) {
      this.Text = text;
    }

    /// <summary>Text to be rendered in the control's frame</summary>
    public string Text;

  }

} // namespace Nuclex.UserInterface.Controls
