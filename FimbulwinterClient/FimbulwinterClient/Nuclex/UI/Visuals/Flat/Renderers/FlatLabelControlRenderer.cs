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

namespace Nuclex.UserInterface.Visuals.Flat.Renderers {

  /// <summary>Renders label controls in a traditional flat style</summary>
  public class FlatLabelControlRenderer :
    IFlatControlRenderer<Controls.LabelControl> {

    /// <summary>
    ///   Renders the specified control using the provided graphics interface
    /// </summary>
    /// <param name="control">Control that will be rendered</param>
    /// <param name="graphics">
    ///   Graphics interface that will be used to draw the control
    /// </param>
    public void Render(
      Controls.LabelControl control, IFlatGuiGraphics graphics
    ) {
      graphics.DrawString("label", control.GetAbsoluteBounds(), control.Text);
    }

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers
