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

  /// <summary>Employer to create factories of suiting types found in plugins</summary>
  /// <typeparam name="ProductType">
  ///   Interface or base class that the types need to implement
  /// </typeparam>
  /// <remarks>
  ///   <para>
  ///     This employer will not directly instanciate any compatible types found in
  ///     plugin assemblies, but generated runtime-factories of these types, enabling the
  ///     user to decide when and how many instances of a type will be created.
  ///   </para>
  ///   <para>
  ///     This approach has the advantage that it enables even assemblies that were not
  ///     intended to be plugins can be loaded as plugins, without risking an instanciation
  ///     or complex and possibly heavy-weight types. The disadvantage is that the
  ///     runtime-factory can not provide decent informationa about the plugin type like
  ///     a human-readable name, capabilities or an icon.
  ///   </para>
  /// </remarks>
  public class FactoryEmployer<ProductType> : Employer where ProductType : class {

    #region class ConcreteFactory

    /// <summary>Concrete factory for the types in a plugin assembly</summary>
    private class ConcreteFactory : IAbstractFactory<ProductType>, IAbstractFactory {

      /// <summary>
      ///   Initializes a factory and configures it for the specified product
      /// </summary>
      /// <param name="type">Type of which the factory creates instances</param>
      public ConcreteFactory(Type type) {
        this.concreteType = type;
      }

      /// <summary>Create a new instance of the type the factory is configured to</summary>
      /// <returns>The newly created instance</returns>
      public ProductType CreateInstance() {
        return (ProductType)Activator.CreateInstance(this.concreteType);
      }

      /// <summary>Create a new instance of the type the factory is configured to</summary>
      /// <returns>The newly created instance</returns>
      object IAbstractFactory.CreateInstance() {
        return Activator.CreateInstance(this.concreteType);
      }

      /// <summary>Concrete product which the factory instance creates</summary>
      private Type concreteType;

    }

    #endregion // class Factory

    /// <summary>Initializes a new FactoryEmployer</summary>
    public FactoryEmployer() {
      this.employedFactories = new List<IAbstractFactory<ProductType>>();
    }

    /// <summary>List of all factories that the instance employer has created</summary>
    public List<IAbstractFactory<ProductType>> Factories {
      get { return this.employedFactories; }
    }

    /// <summary>Determines whether the type suites the employer's requirements</summary>
    /// <param name="type">Type which will be assessed</param>
    /// <returns>True if the type can be employed</returns>
    public override bool CanEmploy(Type type) {
      return
        PluginHelper.HasDefaultConstructor(type) &&
        typeof(ProductType).IsAssignableFrom(type) &&
        !type.ContainsGenericParameters;
    }

    /// <summary>Employs the specified plugin type</summary>
    /// <param name="type">Type to be employed</param>
    public override void Employ(Type type) {
      if(!PluginHelper.HasDefaultConstructor(type)) {
        throw new MissingMethodException(
          "Cannot employ type because it does not have a public default constructor"
        );
      }
      if(!typeof(ProductType).IsAssignableFrom(type)) {
        throw new InvalidCastException(
          "Cannot employ type because it cannot be cast to the factory's product type"
        );
      }
      if(type.ContainsGenericParameters) {
        throw new ArgumentException(
          "Cannot employ type because it requires generic parameters", "type"
        );
      }

      this.employedFactories.Add(new ConcreteFactory(type));
    }

    /// <summary>All factories that the instance employer has created</summary>
    private List<IAbstractFactory<ProductType>> employedFactories;

  }

} // namespace Nuclex.Support.Plugins
