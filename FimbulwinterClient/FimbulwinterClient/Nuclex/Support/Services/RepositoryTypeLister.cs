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

using Nuclex.Support.Plugins;

#if ENABLE_SERVICEMANAGER

namespace Nuclex.Support.Services {

  /// <summary>
  ///   Lists the types of all assemblies contained in an assembly repository
  /// </summary>
  public class RepositoryTypeLister : MultiAssemblyTypeLister {

    /// <summary>
    ///   Initializes a new repository type lister using a new repository
    /// </summary>
    public RepositoryTypeLister() : this(new PluginRepository()) { }

    /// <summary>
    ///   Initializes a new repository type lister using an existing repository
    /// </summary>
    /// <param name="repository">
    ///   Repository containing the assemblies whose types will be listed
    /// </param>
    public RepositoryTypeLister(PluginRepository repository) {
      this.repository = repository;
    }

    /// <summary>
    ///   Returns an enumerable list of the assemblies in the repository
    /// </summary>
    /// <returns>An enumerable list of the assemblies in the repository</returns>
    protected override IEnumerable<Assembly> GetAssemblies() {
      return this.repository.LoadedAssemblies;
    }

    /// <summary>
    ///   The assembly repository containing the assemblies whose types the lister
    ///   operates on.
    /// </summary>
    public PluginRepository Repository {
      get { return this.repository; }
    }

    /// <summary>
    ///   Repository containing the assemblies with the type lister's types
    /// </summary>
    private PluginRepository repository;

  }

} // namespace Nuclex.Support.Services

#endif // ENABLE_SERVICEMANAGER
