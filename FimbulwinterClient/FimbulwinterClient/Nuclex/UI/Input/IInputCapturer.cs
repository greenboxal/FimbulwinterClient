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

namespace Nuclex.UserInterface.Input {

  /// <summary>
  ///   Interface for input capturers that monitor user input and forward it to
  ///   a freely settable input receiver
  /// </summary>
  public interface IInputCapturer {

    /// <summary>Input receiver any captured input will be sent to</summary>
    IInputReceiver InputReceiver { get; set; }
    
  }

} // namespace Nuclex.UserInterface.Input
