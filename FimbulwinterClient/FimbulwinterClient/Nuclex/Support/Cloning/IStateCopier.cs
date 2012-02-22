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

namespace Nuclex.Support.Cloning {

  /// <summary>Copies the state of objects</summary>
  public interface IStateCopier {

    /// <summary>
    ///   Transfers the state of one object into another, creating clones of referenced objects
    /// </summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    /// <remarks>
    ///   A property-based copy is useful if you're using dynamically generated proxies,
    ///   such as when working with entities returned by an ORM like NHibernate.
    ///   When not using a property-based copy, internal proxy fields would be copied
    ///   and might cause problems with the ORM.
    /// </remarks>
    void DeepCopyState<TState>(TState original, TState target, bool propertyBased)
      where TState : class;

    /// <summary>
    ///   Transfers the state of one object into another, creating clones of referenced objects
    /// </summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    /// <remarks>
    ///   A property-based copy is useful if you're using dynamically generated proxies,
    ///   such as when working with entities returned by an ORM like NHibernate.
    ///   When not using a property-based copy, internal proxy fields would be copied
    ///   and might cause problems with the ORM.
    /// </remarks>
    void DeepCopyState<TState>(ref TState original, ref TState target, bool propertyBased)
      where TState : struct;

    /// <summary>Transfers the state of one object into another</summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    /// <remarks>
    ///   A property-based copy is useful if you're using dynamically generated proxies,
    ///   such as when working with entities returned by an ORM like NHibernate.
    ///   When not using a property-based copy, internal proxy fields would be copied
    ///   and might cause problems with the ORM.
    /// </remarks>
    void ShallowCopyState<TState>(TState original, TState target, bool propertyBased)
      where TState : class;

    /// <summary>Transfers the state of one object into another</summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    /// <remarks>
    ///   A property-based copy is useful if you're using dynamically generated proxies,
    ///   such as when working with entities returned by an ORM like NHibernate.
    ///   When not using a property-based copy, internal proxy fields would be copied
    ///   and might cause problems with the ORM.
    /// </remarks>
    void ShallowCopyState<TState>(ref TState original, ref TState target, bool propertyBased)
      where TState : struct;

  }

} // namespace Nuclex.Support.Cloning
