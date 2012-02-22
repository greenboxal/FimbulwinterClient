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

namespace Nuclex.UserInterface.Controls.Desktop {

  /// <summary>Horizontal slider that can be moved using the mouse</summary>
  public class HorizontalSliderControl : SliderControl {

    /// <summary>Obtains the region covered by the slider's thumb</summary>
    /// <returns>The region covered by the slider's thumb</returns>
    protected override RectangleF GetThumbRegion() {
      RectangleF bounds = GetAbsoluteBounds();

      if(base.ThumbLocator != null) {
        return base.ThumbLocator.GetThumbPosition(
          bounds, base.ThumbPosition, base.ThumbSize
        );
      } else {
        float thumbWidth = bounds.Width * base.ThumbSize;
        float thumbX = (bounds.Width - thumbWidth) * base.ThumbPosition;

        return new RectangleF(thumbX, 0, thumbWidth, bounds.Height);
      }
    }

    /// <summary>Moves the thumb to the specified location</summary>
    /// <returns>Location the thumb will be moved to</returns>
    protected override void MoveThumb(float x, float y) {
      RectangleF bounds = GetAbsoluteBounds();

      float thumbWidth = bounds.Width * base.ThumbSize;
      float maxX = bounds.Width - thumbWidth;

      // Prevent divide-by-zero if the thumb fills out the whole rail
      if(maxX > 0.0f) {
        base.ThumbPosition = MathHelper.Clamp(x / maxX, 0.0f, 1.0f);
      } else {
        base.ThumbPosition = 0.0f;
      }

      OnMoved();
    }

  }

} // namespace Nuclex.UserInterface.Controls.Desktop
