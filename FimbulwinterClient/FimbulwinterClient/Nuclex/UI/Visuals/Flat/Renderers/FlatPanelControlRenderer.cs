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

namespace Nuclex.UserInterface.Visuals.Flat.Renderers {

  /// <summary>Renders panel controls in a traditional flat style</summary>
  public class FlatPanelControlRenderer :
    IFlatControlRenderer<Controls.Arcade.PanelControl> {

    /// <summary>
    ///   Renders the specified control using the provided graphics interface
    /// </summary>
    /// <param name="control">Control that will be rendered</param>
    /// <param name="graphics">
    ///   Graphics interface that will be used to draw the control
    /// </param>
    public void Render(
      Controls.Arcade.PanelControl control, IFlatGuiGraphics graphics
    ) {

      // This is simple! A panel consists of a single element we need to draw.
      graphics.DrawElement("window", control.GetAbsoluteBounds());

    }

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers
