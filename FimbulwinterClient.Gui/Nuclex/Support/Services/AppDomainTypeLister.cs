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

#if ENABLE_SERVICEMANAGER

namespace Nuclex.Support.Services {

#if WINDOWS

  /// <summary>Lists the types of all assemblies in an application domain</summary>
  public class AppDomainTypeLister : MultiAssemblyTypeLister {

    /// <summary>
    ///   Initializes a new application domain type lister using the application domain
    ///   of the calling method
    /// </summary>
    public AppDomainTypeLister() : this(AppDomain.CurrentDomain) { }

    /// <summary>Initializes a new application domain type lister</summary>
    /// <param name="appDomain">Application domain whose types will be listed</param>
    public AppDomainTypeLister(AppDomain appDomain) {
      this.appDomain = appDomain;
    }

    /// <summary>
    ///   Obtains an enumerable list of all assemblies in the application domain
    /// </summary>
    /// <returns>An enumerable list of the assemblies in the application domain</returns>
    protected override IEnumerable<Assembly> GetAssemblies() {
      return this.appDomain.GetAssemblies();
    }

    /// <summary>Application domain whose types the lister works on</summary>
    private AppDomain appDomain;

  }

#endif // WINDOWS

} // namespace Nuclex.Support.Services

#endif // ENABLE_SERVICEMANAGER
