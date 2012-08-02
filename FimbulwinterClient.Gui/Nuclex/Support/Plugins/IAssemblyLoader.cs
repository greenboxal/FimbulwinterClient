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
using System.Reflection;

namespace Nuclex.Support.Plugins {

  /// <summary>Interface for an assembly loading helper</summary>
  public interface IAssemblyLoader {

    /// <summary>Tries to loads an assembly from a file</summary>
    /// <param name="path">Path to the file that is loaded as an assembly</param>
    /// <param name="loadedAssembly">
    ///   Output parameter that receives the loaded assembly or null
    /// </param>
    /// <returns>True if the assembly was loaded successfully, otherwise false</returns>
    bool TryLoadFile(string path, out Assembly loadedAssembly);

  }

} // namespace Nuclex.Support.Plugins
