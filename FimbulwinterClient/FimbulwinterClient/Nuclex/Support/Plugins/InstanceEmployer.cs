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

namespace Nuclex.Support.Plugins {

  /// <summary>Employer that directly creates instances of the types in a plugin</summary>
  /// <typeparam name="T">Interface or base class required for the employed types</typeparam>
  /// <remarks>
  ///   <para>
  ///     This employer directly creates an instance of any type in a plugin assembly that
  ///     implements or is derived from the type the generic InstanceEmployer is instanced
  ///     to. This is useful when the plugin user already has a special plugin interface
  ///     through which additional informations about a plugin type can be queried or
  ///     when actually exactly one instance per plugin type is wanted (think of the
  ///     prototype pattern for example)
  ///   </para>
  ///   <para>
  ///     Because this employer blindly creates an instance of any compatible type found
  ///     in a plugin assembly it should be used with care. If big types with high
  ///     construction time or huge memory requirements are loaded this can become
  ///     a real resource hog. The intention of this employer was to let the plugin user
  ///     define his own factory interface which possibly provides further details about
  ///     the type the factory is reponsible for (like a description field). This
  ///     factory would then be implemented on the plugin side.
  ///   </para>
  /// </remarks>
  public class InstanceEmployer<T> : Employer {

    /// <summary>Initializes a new instance employer</summary>
    public InstanceEmployer() {
      this.employedInstances = new List<T>();
    }

    /// <summary>All instances that have been employed</summary>
    public List<T> Instances {
      get { return this.employedInstances; }
    }

    /// <summary>Determines whether the type suites the employer's requirements</summary>
    /// <param name="type">Type that is checked for employability</param>
    /// <returns>True if the type can be employed</returns>
    public override bool CanEmploy(Type type) {
      return
        PluginHelper.HasDefaultConstructor(type) &&
        typeof(T).IsAssignableFrom(type) &&
        !type.ContainsGenericParameters;
    }

    /// <summary>Employs the specified plugin type</summary>
    /// <param name="type">Type to be employed</param>
    public override void Employ(Type type) {
      this.employedInstances.Add((T)Activator.CreateInstance(type));
    }

    /// <summary>All instances employed by the instance employer</summary>
    private List<T> employedInstances;

  }

} // namespace Nuclex.Support.Plugins
