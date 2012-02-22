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

  /// <summary>Renders button controls in a traditional flat style</summary>
  public class FlatButtonControlRenderer :
    IFlatControlRenderer<Controls.Desktop.ButtonControl> {

    /// <summary>
    ///   Renders the specified control using the provided graphics interface
    /// </summary>
    /// <param name="control">Control that will be rendered</param>
    /// <param name="graphics">
    ///   Graphics interface that will be used to draw the control
    /// </param>
    public void Render(
      Controls.Desktop.ButtonControl control, IFlatGuiGraphics graphics
    ) {
      RectangleF controlBounds = control.GetAbsoluteBounds();

      // Determine the style to use for the button
      int stateIndex = 0;
      if(control.Enabled) {
        if(control.Depressed) {
          stateIndex = 3;
        } else if(control.MouseHovering || control.HasFocus) {
          stateIndex = 2;
        } else {
          stateIndex = 1;
        }
      }

      // Draw the button's frame
      graphics.DrawElement(states[stateIndex], controlBounds);

      // If there's text assigned to the button, draw it into the button
      if(!string.IsNullOrEmpty(control.Text)) {
        graphics.DrawString(states[stateIndex], controlBounds, control.Text);
      }
    }

    /// <summary>Names of the states the button control can be in</summary>
    /// <remarks>
    ///   Storing this as full strings instead of building them dynamically prevents
    ///   any garbage from forming during rendering.
    /// </remarks>
    private static readonly string[] states = new string[] {
      "button.disabled",
      "button.normal",
      "button.highlighted",
      "button.depressed"
    };

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers
