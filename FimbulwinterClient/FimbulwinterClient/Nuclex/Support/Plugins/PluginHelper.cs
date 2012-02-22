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
using System.Reflection;

namespace Nuclex.Support.Plugins {

  /// <summary>Supporting functions for the plugin classes</summary>
  public static class PluginHelper {

    /// <summary>Determines whether the given type has a default constructor</summary>
    /// <param name="type">Type which is to be checked</param>
    /// <returns>True if the type has a default constructor</returns>
    public static bool HasDefaultConstructor(Type type) {
      ConstructorInfo[] constructors = type.GetConstructors();

      foreach(ConstructorInfo constructor in constructors)
        if(constructor.IsPublic && (constructor.GetParameters().Length == 0))
          return true;

      return false;
    }

  }

} // namespace Nuclex.Support.Plugins
