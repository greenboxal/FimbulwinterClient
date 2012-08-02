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

using Nuclex.Support.Plugins;

#if ENABLE_SERVICEMANAGER

namespace Nuclex.Support.Services {

  /// <summary>Modes in which services can be instantiated</summary>
  public enum Instancing {

    /// <summary>Disallow any service from being created for a contract</summary>
    Never,

    /// <summary>There will only be one service in the whole process</summary>
    Singleton,

    /// <summary>Each thread will be assigned its own service</summary>
    InstancePerThread,

    /// <summary>A new service will be created each time it is queried for</summary>
    Factory

  }

} // namespace Nuclex.Support.Services

#endif // ENABLE_SERVICEMANAGER
