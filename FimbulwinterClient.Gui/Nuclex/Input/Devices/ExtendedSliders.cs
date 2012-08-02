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

namespace Nuclex.Input.Devices {

  /// <summary>Extended slider axes provided by a game pad or joystick</summary>
  [Flags]
  public enum ExtendedSliders {

    /// <summary>First additional axis (formerly called U-axis)</summary>
    Slider1 = (1 << 0),
    /// <summary>Second additional axis (formerly called V-axis)</summary>
    Slider2 = (1 << 1),
    /// <summary>First extra velocity axis</summary>
    Velocity1 = (1 << 2),
    /// <summary>Second extra velocity axis</summary>
    Velocity2 = (1 << 3),
    /// <summary>First extra acceleration axis</summary>
    Acceleration1 = (1 << 4),
    /// <summary>Second extra acceleration axis</summary>
    Acceleration2 = (1 << 5),
    /// <summary>First extra force axis</summary>
    Force1 = (1 << 6),
    /// <summary>Second extra force axis</summary>
    Force2 = (1 << 7),

  }

} // namespace Nuclex.Input.Devices
