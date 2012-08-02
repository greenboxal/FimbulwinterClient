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

  /// <summary>Abstract factory for a concrete type</summary>
  public interface IAbstractFactory {

    /// <summary>
    ///   Creates a new instance of the type to which the factory is specialized
    /// </summary>
    /// <returns>The newly created instance</returns>
    object CreateInstance();    
  
  }

  /// <summary>Abstract factory for a concrete type</summary>
  /// <typeparam name="ProductType">
  ///   Interface or base class of the product of the factory
  /// </typeparam>
  public interface IAbstractFactory<ProductType> {

    /// <summary>
    ///   Creates a new instance of the type to which the factory is specialized
    /// </summary>
    /// <returns>The newly created instance</returns>
    ProductType CreateInstance();

  }

} // namespace Nuclex.Support.Plugins
