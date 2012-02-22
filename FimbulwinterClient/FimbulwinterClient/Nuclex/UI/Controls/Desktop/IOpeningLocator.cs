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

using Microsoft.Xna.Framework;

namespace Nuclex.UserInterface.Controls.Desktop {

  /// <summary>
  ///   Interface which can be established between a control and its renderer to
  ///   allow the control to locate openings between letters
  /// </summary>
  /// <remarks>
  ///   A renderer can implement this interface and assign it to a control that
  ///   it renders so the control can ask the renderer for extended informations
  ///   regarding the look of its text. If this interface is provided, certain
  ///   controls will be able to correctly place the caret in user-editable text
  ///   when they are clicked by the mouse.
  /// </remarks>
  public interface IOpeningLocator {

    /// <summary>
    ///   Calculates which opening between two letters is closest to a position
    /// </summary>
    /// <param name="bounds">
    ///   Boundaries of the control, should be in absolute coordinates
    /// </param>
    /// <param name="text">Text in which the nearest opening will be located</param>
    /// <param name="position">
    ///   Position to which the closest opening will be found,
    ///   should be in absolute coordinates
    /// </param>
    /// <returns>The index of the opening closest to the provided position</returns>
    int GetClosestOpening(RectangleF bounds, string text, Vector2 position);

  }

} // namespace Nuclex.UserInterface.Controls.Desktop
