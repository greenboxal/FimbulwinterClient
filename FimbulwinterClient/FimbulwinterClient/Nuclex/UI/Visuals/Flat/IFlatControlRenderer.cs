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

namespace Nuclex.UserInterface.Visuals.Flat {

  /// <summary>Interface for a class that renders a control</summary>
  public interface IFlatControlRenderer { }

  /// <summary>
  ///   Interface for a class responsible to render a specific control type
  /// </summary>
  /// <typeparam name="ControlType">
  ///   Type of control the implementation class will render
  /// </typeparam>
  public interface IFlatControlRenderer<ControlType> : IFlatControlRenderer
    where ControlType : Controls.Control {

    /// <summary>
    ///   Renders the specified control using the provided graphics interface
    /// </summary>
    /// <param name="control">Control that will be rendered</param>
    /// <param name="graphics">
    ///   Graphics interface that will be used to draw the control
    /// </param>
    void Render(ControlType control, IFlatGuiGraphics graphics);

  }

} // namespace Nuclex.UserInterface.Visuals.Flat
