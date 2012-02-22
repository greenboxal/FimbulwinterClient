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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Nuclex.UserInterface.Visuals;

namespace Nuclex.UserInterface {

  /// <summary>Game-wide interface for the GUI manager component</summary>
  public interface IGuiService {

    /// <summary>GUI that is being rendered</summary>
    /// <remarks>
    ///   The GUI manager renders one GUI full-screen onto the primary render target
    ///   (the backbuffer). This property holds the GUI that is being managed by
    ///   the GUI manager component. You can replace it at any time, for example,
    ///   if the player opens or closes your ingame menu.
    /// </remarks>
    Screen Screen { get; set; }

    /// <summary>
    ///   Responsible for creating a visual representation of the GUI on the screen
    /// </summary>
    IGuiVisualizer Visualizer { get; set; }

  }

} // namespace Nuclex.UserInterface
