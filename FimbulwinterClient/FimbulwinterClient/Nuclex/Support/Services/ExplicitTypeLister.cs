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
using System.Text;

#if ENABLE_SERVICEMANAGER

namespace Nuclex.Support.Services {

  /// <summary>Type lister that returns a predefined list of types</summary>
  public class ExplicitTypeLister : ITypeLister {

    /// <summary>Initializes a new predefined type lister</summary>
    /// <param name="types">Types the predefined type lister will list</param>
    public ExplicitTypeLister(params Type[] types) {
      this.types = new List<Type>(types);
    }

    /// <summary>Initializes a new predefined type lister</summary>
    /// <param name="types">Types the predefined type lister will list</param>
    public ExplicitTypeLister(IEnumerable<Type> types) {
      this.types = new List<Type>(types);
    }

    /// <summary>
    ///   Returns an enumerable list of types that will be checked by the service manager
    /// </summary>
    /// <returns>An enumerable list of types for the service manager</returns>
    public IEnumerable<Type> GetTypes() {
      return this.types;
    }

    /// <summary>Predefined list of types the lister will list</summary>
    public List<Type> Types {
      get { return this.types; }
    }

    /// <summary>The predefined list of types</summary>
    private List<Type> types;

  }

} // namespace Nuclex.Support.Services

#endif // ENABLE_SERVICEMANAGER
