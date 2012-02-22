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

  /// <summary>Visual indicator for the progress of some operation</summary>
  public class ProgressControl : Control {

    // TODO: Make a derived, interactive version of this control
    //   Name: VolumeControl

    /// <summary>The displayed progress in the range between 0.0 and 1.0</summary>
    public float Progress;

  }

} // namespace Nuclex.UserInterface.Controls
