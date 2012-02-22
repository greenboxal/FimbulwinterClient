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

namespace Nuclex.Support.Plugins {

  /// <summary>Plugin type employer</summary>
  /// <remarks>
  ///   This class is used by the plugin host to assess whether a concrete type found
  ///   in a plugin assembly is suited to be processed the plugin user. If so,
  ///   the employer can employ the type. Employing can typically mean to create an
  ///   instance of the type in the plugin assembly or to build a runtime-factory
  ///   that can create instances of the type when it is needed.
  /// </remarks>
  public abstract class Employer {

    /// <summary>Determines whether the type suites the employer's requirements</summary>
    /// <param name="type">Type which will be assessed</param>
    /// <returns>True if the type can be employed, otherwise false</returns>
    public virtual bool CanEmploy(Type type) {
      return PluginHelper.HasDefaultConstructor(type);
    }

    /// <summary>Employs the specified plugin type</summary>
    /// <param name="type">Type to be employed</param>
    public abstract void Employ(Type type);

  }

} // namespace Nuclex.Support.Plugins
