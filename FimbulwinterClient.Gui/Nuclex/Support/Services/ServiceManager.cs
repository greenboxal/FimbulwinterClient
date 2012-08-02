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
using System.Diagnostics;
using System.Reflection;
using System.Threading;

using Nuclex.Support.Plugins;

#if ENABLE_SERVICEMANAGER

namespace Nuclex.Support.Services {

  // Allow Dependency on Container
  //   public Foo(IServiceProvider serviceProvider)
  //   public Foo(IserviceLocator serviceLocator)
  //   public Foo(Container container)

  /// <summary>
  ///   Inversion of Control container that manages the services of an application
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This is a very lightweight and simple inversion of control container that
  ///     relieves components of their duty to track down implementations for the services
  ///     they require to function. It will help with lazy initialization and prevent
  ///     components from becoming cross-linked balls of spaghetti references.
  ///   </para>
  ///   <para>
  ///     Here's a short list of the terms used throughout this container and their
  ///     specific meaning in this context.
  ///   </para>
  ///   <para>
  ///     <list type="bullet">
  ///       <item>
  ///         <term>Service</term>
  ///         <description>
  ///           Defined by an interface (service contract) and provided by a component
  ///           that implements the service contract. A service provides some kind of
  ///           utility to the application, for example it could provide access to
  ///           a data base or allow other components to control certain aspects of
  ///           the application.
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <term>Contract</term>
  ///         <description>
  ///           Interface defining the behavior that a service implementation has to
  ///           follow. In order for a component to become a suitable candidate for
  ///           providing a specific service, it has to implement the service contract
  ///           interface and should rigorously follow its specifications.
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <term>Component</term>
  ///         <description>
  ///           A component is simply a class that implements one or more service
  ///           contracts. The service manager will created instances of these classes
  ///           when all their dependencies can be provided for and an implementation
  ///           of their service contract is requested.
  ///         </description>
  ///       </item>
  ///     </list>
  ///   </para>
  /// </remarks>
  public partial class ServiceManager : IServiceProvider {

#region class Contract

    /// <summary>Stores the settings for an individual contract</summary>
    private class Contract {

      /// <summary>
      ///   Factory by which instances of the contract implementation can be created
      /// </summary>
      public IAbstractFactory Factory;

      /// <summary>How instances of the implementation are to be managed</summary>
      public Instancing Instancing;

      /// <summary>Single global instance of the contract implementation</summary>
      /// <remarks>
      ///   Used only if <paramref name="Instancing" /> is set to Singleton
      /// </remarks>
      public object SingletonInstance;

      /// <summary>Thread-local instance of the contract implementation</summary>
      /// <remarks>
      ///   Used only if <paramref name="Instancing" />is set to InstancePerThread
      /// </remarks>
      public object ThreadLocalInstance {
        get {
          initializeThreadLocalData();
          return Thread.GetData(this.threadLocalDataSlot);
        }
        set {
          initializeThreadLocalData();
          Thread.SetData(this.threadLocalDataSlot, value);
        }
      }

      /// <summary>Initializes the thread-local data slot</summary>
      private void initializeThreadLocalData() {
        if(this.threadLocalDataSlot == null) {
          lock(this) {
            if(this.threadLocalDataSlot == null) {
              this.threadLocalDataSlot = Thread.AllocateDataSlot();
            }
          }
        }
      }

      /// <summary>Arguments to be passed to the component constructor</summary>
      private Dictionary<string, object> arguments;

      /// <summary>Data slot for thread local storage</summary>
      /// <remarks>
      ///   We're using an explicit data slot because the ThreadStaticAttribute class
      ///   can only be used on static fields and also because this class is not
      ///   supported by the .NET Compact Framework.
      /// </remarks>
      private volatile LocalDataStoreSlot threadLocalDataSlot;

    }

    #endregion // class Contract

#if WINDOWS

    /// <summary>Initializes a new service manager</summary>
    /// <remarks>
    ///   This overload will automatically use a type lister that causes all types
    ///   in all loaded assemblies of the calling app domain to be considered
    ///   by the service manager for obtaining contract implementations.
    /// </remarks>
    public ServiceManager() : this(new AppDomainTypeLister()) { }

#endif // WINDOWS

    /// <summary>Initializes a new service manager</summary>
    /// <param name="typeLister">
    ///   Type lister providing the types considered by the service manager for
    ///   obtaining contract implementations.
    /// </param>
    public ServiceManager(ITypeLister typeLister) {
      this.typeLister = typeLister;
      this.contracts = new Dictionary<Type, Contract>();
    }

    /// <summary>
    ///   Returns all available implementations for the specified contract
    /// </summary>
    /// <returns>
    ///   A new enumerator for the available contract implementations
    /// </returns>
    public IEnumerable<Type> GetComponents<ContractType>() where ContractType : class {
      Type contractType = typeof(ContractType);

      foreach(Type checkedType in this.typeLister.GetTypes()) {
        bool isImplementationOfContract =
          (!checkedType.IsAbstract) &&
          contractType.IsAssignableFrom(checkedType);

        if(isImplementationOfContract) {
          yield return checkedType;
        }
      }
    }

    /// <summary>
    ///   Returns all available implementations for the specified contract
    /// </summary>
    /// <param name="completeOnly">
    ///   If true, only services whose dependencies can be completely
    ///   satisfied by the container are returned.
    /// </param>
    /// <returns>
    ///   A new enumerator for the available contract implementations
    /// </returns>
    public IEnumerable<Type> GetComponents<ContractType>(bool completeOnly)
      where ContractType : class {
      if(completeOnly) {
        return filterCompleteComponents(GetComponents<ContractType>());
      } else {
        return GetComponents<ContractType>();
      }
    }

    /// <summary>
    ///   Filters a list of components so only components whose dependencies can be
    ///   completely provided are enumerated
    /// </summary>
    /// <param name="types">Enumerable type list that will be filtered</param>
    /// <returns>
    ///   Only those components whose dependencies can be completely provided
    /// </returns>
    private IEnumerable<Type> filterCompleteComponents(IEnumerable<Type> types) {
      foreach(Type type in types) {

        bool isCandidate =
          (!type.IsValueType) &&
          (!type.IsAbstract) &&
          (type.IsPublic || type.IsNestedPublic);

        if(isCandidate) {

          ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public);
          


          // If a contract has been 
          Contract contract;
          if(this.contracts.TryGetValue(type, out contract)) {
            yield return type;
          } else {
          }

          yield return type;

        }

      }
    }

    /// <summary>
    ///   Allows the adjustment of the container's behavior in regard to
    ///   the specified contract
    /// </summary>
    /// <typeparam name="ContractType">
    ///   Contract for which the behavior will be adjusted
    /// </typeparam>
    /// <returns>
    ///   A context object through which the behavior of the container can be
    ///   adjusted for the specified type
    /// </returns>
    public ForContext<ContractType> For<ContractType>() where ContractType : class {
      return new ForContext<ContractType>(this);
    }

    /// <summary>
    ///   Allows the adjustment of the container's behavior in regard to
    ///   the specified contract
    /// </summary>
    /// <param name="contractType">
    ///   Contract for which the behavior will be adjusted
    /// </param>
    /// <returns>
    ///   A context object through which the behavior of the container can be
    ///   adjusted for the specified type
    /// </returns>
    public ForContext For(Type contractType) {
      return new ForContext(this, contractType);
    }

    /// <summary>Retrieves the service of the specified type</summary>
    /// <typeparam name="ContractType">
    ///   Contract for which the service will be retrieved
    /// </typeparam>
    /// <returns>The service for the specified contract</returns>
    public ContractType GetService<ContractType>() where ContractType : class {
      return (ContractType)GetService(typeof(ContractType));
    }

    /// <summary>Retrieves the service of the specified type</summary>
    /// <param name="contractType">
    ///   Contract for which the service will be retrieved
    /// </param>
    /// <returns>The service for the specified contract</returns>
    public object GetService(Type contractType) {
      Contract c = resolveContract(contractType);
      return c.Factory.CreateInstance(); // TODO: Honor the contract settings
    }

    /// <summary>
    ///   Resolves all dependencies required to create a service for a contract
    /// </summary>
    /// <param name="contractType">
    ///   Type of contract for which to resolve the implementation
    /// </param>
    /// <returns>The settings for the contract including a valid factory</returns>
    private Contract resolveContract(Type contractType) {
      if(contractType.IsValueType) {
        throw new ArgumentException(
          "Contracts have to be interfaces or classes", "contractType"
        );
      }
      /*
            Contract contract;
            if(this.contracts.TryGetValue(contractType, out contract)) {
              return contract;
            }
      */


      throw new NotImplementedException();
    }

    /// <summary>Lists all types partaking in the dependency injection</summary>
    private ITypeLister typeLister;
    /// <summary>Dictionary with settings for each individual contract</summary>
    private Dictionary<Type, Contract> contracts;

  }

} // namespace Nuclex.Support.Services

#endif // ENABLE_SERVICEMANAGER
