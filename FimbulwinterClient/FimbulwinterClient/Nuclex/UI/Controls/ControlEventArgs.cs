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

  /// <summary>Event argument class that carries a control instance</summary>
  public class ControlEventArgs : EventArgs {

    /// <summary>Initializes a new control event args instance</summary>
    /// <param name="control">Control to provide to the subscribers of the event</param>
    public ControlEventArgs(Controls.Control control) {
      this.control = control;
    }

    /// <summary>Control that has been provided for the event</summary>
    public Controls.Control Control {
      get { return this.control; }
    }

    /// <summary>Control that will be accessible to the event subscribers</summary>
    private Controls.Control control;

  }

} // namespace Nuclex.UserInterface.Controls
