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

  /// <summary>Attribute that prevents a class from being seen by the PluginHost</summary>
  /// <remarks>
  ///   When this attribute is attached to a class it will be invisible to the
  ///   PluginHost and not become accessable as a plugin.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class NoPluginAttribute : Attribute {

    /// <summary>Initializes an instance of the NoPluginAttributes</summary>
    public NoPluginAttribute() : base() { }

  }

} // namespace Nuclex.Support.Plugins
