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

namespace Nuclex.UserInterface.Controls.Desktop {

  /// <summary>A window for hosting other controls</summary>
  public class WindowControl : DraggableControl {

    /// <summary>Initializes a new window control</summary>
    public WindowControl() : base(true) {}

    /// <summary>Closes the window</summary>
    public void Close() {
      if(IsOpen) {
        Parent.Children.Remove(this);
      }
    }
    
    /// <summary>Whether the window is currently open</summary>
    public bool IsOpen {
      get { return Screen != null; }
    }

    /// <summary>Whether the window can be dragged with the mouse</summary>
    public new bool EnableDragging {
      get { return base.EnableDragging; }
      set { base.EnableDragging = value; }
    }

    /// <summary>Text in the title bar of the window</summary>
    public string Title;

  }

} // namespace Nuclex.UserInterface.Controls.Desktop
