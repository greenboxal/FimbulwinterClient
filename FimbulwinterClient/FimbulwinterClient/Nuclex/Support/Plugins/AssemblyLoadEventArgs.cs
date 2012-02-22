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

#if XBOX360 || WINDOWS_PHONE

  /// <summary>Signature for the AssemblyLoad event</summary>
  /// <param name="sender">Object that is reporting that an assembly was loaded</param>
  /// <param name="arguments">Contains the loaded assembly</param>
  public delegate void AssemblyLoadEventHandler(
    object sender, AssemblyLoadEventArgs arguments
  );

  /// <summary>Argument container for the AssemblyLoad event arguments</summary>
  public class AssemblyLoadEventArgs : EventArgs {

    /// <summary>Initializes a new event argument container</summary>
    /// <param name="loadedAssembly">Assembly that has been loaded</param>
    public AssemblyLoadEventArgs(Assembly loadedAssembly) {
      this.loadedAssembly = loadedAssembly;
    }
    
    /// <summary>Assembly that was loaded by the sender of the event</summary>
    public Assembly LoadedAssembly {
      get { return this.loadedAssembly; }
    }
    
    /// <summary>Loaded assembly that will be provided to the event receivers</summary>
    private Assembly loadedAssembly;

  }

#endif // XBOX360 || WINDOWS_PHONE

} // namespace Nuclex.Support.Plugins
