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

  /// <summary>Renders sliders in a traditional flat style</summary>
  public class FlatVerticalSliderControlRenderer :
    IFlatControlRenderer<Controls.Desktop.VerticalSliderControl> {

    /// <summary>
    ///   Renders the specified control using the provided graphics interface
    /// </summary>
    /// <param name="control">Control that will be rendered</param>
    /// <param name="graphics">
    ///   Graphics interface that will be used to draw the control
    /// </param>
    public void Render(
      Controls.Desktop.VerticalSliderControl control, IFlatGuiGraphics graphics
    ) {
      RectangleF controlBounds = control.GetAbsoluteBounds();

      float thumbHeight = controlBounds.Height * control.ThumbSize;
      float thumbY = (controlBounds.Height - thumbHeight) * control.ThumbPosition;

      graphics.DrawElement("rail.vertical", controlBounds);

      RectangleF thumbBounds = new RectangleF(
        controlBounds.X, controlBounds.Y + thumbY, controlBounds.Width, thumbHeight
      );

      if(control.ThumbDepressed) {
        graphics.DrawElement("slider.vertical.depressed", thumbBounds);
      } else if(control.MouseOverThumb) {
        graphics.DrawElement("slider.vertical.highlighted", thumbBounds);
      } else {
        graphics.DrawElement("slider.vertical.normal", thumbBounds);
      }

    }

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers
