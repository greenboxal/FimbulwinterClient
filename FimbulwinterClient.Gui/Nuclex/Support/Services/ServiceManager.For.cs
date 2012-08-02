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

  partial class ServiceManager {

    #region class ForContext

    /// <summary>Manages the context of the "For" modifier</summary>
    public class ForContext {

      /// <summary>Initializes a new "For" context of the service manager</summary>
      /// <param name="serviceManager">Service manager the context operates on</param>
      /// <param name="contractType">Contract that is being modified</param>
      internal ForContext(ServiceManager serviceManager, Type contractType) {
        this.serviceManager = serviceManager;
        this.contractType = contractType;
      }

      /// <summary>Uses the specified implementation for the contract</summary>
      /// <param name="contractImplementation">
      ///   Implementation that will be used for the contract
      /// </param>
      public void Use(object contractImplementation) { }

      /// <summary>
      ///   Uses the provided object as a prototype for the contract implementation
      /// </summary>
      /// <param name="contractImplementationPrototype">
      ///   Contract implementation that will be used as a prototype
      /// </param>
      public void UsePrototype(object contractImplementationPrototype) { }

      /// <summary>Selects the default implementation to use for the contract</summary>
      /// <param name="implementationType">
      ///   Implementation that will be used as the default for the contract
      /// </param>
      public void UseDefault(Type implementationType) { }

      /// <summary>Service manager the "For" context operates on</summary>
      protected ServiceManager serviceManager;
      /// <summary>Contract that is being modified</summary>
      protected Type contractType;

    }

    #endregion // class ForContext

    #region class ForContext<>

    /// <summary>Manages the context of the "For" modifier</summary>
    public class ForContext<ContractType> : ForContext {

      /// <summary>Initializes a new "For" context of the service manager</summary>
      /// <param name="serviceManager">Service manager the context operates on</param>
      internal ForContext(ServiceManager serviceManager) :
        base(serviceManager, typeof(ContractType)) { }

      /// <summary>Uses the specified implementation for the contract</summary>
      /// <param name="implementation">
      ///   Implementation that will be used for the contract
      /// </param>
      public void Use(ContractType implementation) { }

      /// <summary>
      ///   Uses the provided object as a prototype for the contract implementation
      /// </summary>
      /// <typeparam name="PrototypeType">
      ///   Type of the implementation that will be used as a prototype
      /// </typeparam>
      /// <param name="contractImplementationPrototype">
      ///   Contract implementation that will be used as a prototype
      /// </param>
      public void UsePrototype<PrototypeType>(PrototypeType contractImplementationPrototype)
        where PrototypeType : ContractType, ICloneable { }

      /// <summary>Selects the default implementation to use for the contract</summary>
      /// <typeparam name="ImplementationType">
      ///   Implementation that will be used as the default for the contract
      /// </typeparam>
      public void UseDefault<ImplementationType>()
        where ImplementationType : ContractType { }

    }

    #endregion // class ForContext<>

  }

} // namespace Nuclex.Support.Services

#endif // ENABLE_SERVICEMANAGER
