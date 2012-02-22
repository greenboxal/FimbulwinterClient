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

#if !(XBOX360 || WINDOWS_PHONE)

using System;
using System.Collections.Concurrent;

namespace Nuclex.Support.Cloning {

  /// <summary>
  ///   Cloning factory which uses expression trees to improve performance when cloning
  ///   is a high-frequency action.
  /// </summary>
  public partial class ExpressionTreeCloner : ICloneFactory {

    /// <summary>Initializes the static members of the expression tree cloner</summary>
    static ExpressionTreeCloner() {
      shallowFieldBasedCloners = new ConcurrentDictionary<Type, Func<object, object>>();
      deepFieldBasedCloners = new ConcurrentDictionary<Type, Func<object, object>>();
      shallowPropertyBasedCloners = new ConcurrentDictionary<Type, Func<object, object>>();
      deepPropertyBasedCloners = new ConcurrentDictionary<Type, Func<object, object>>();
    }

    /// <summary>
    ///   Creates a deep clone of the specified object, also creating clones of all
    ///   child objects being referenced
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A deep clone of the provided object</returns>
    public static TCloned DeepFieldClone<TCloned>(TCloned objectToClone) {
      object objectToCloneAsObject = objectToClone;
      if(objectToCloneAsObject == null) {
        return default(TCloned);
      }

      Func<object, object> cloner = getOrCreateDeepFieldBasedCloner(typeof(TCloned));
      return (TCloned)cloner(objectToCloneAsObject);
    }

    /// <summary>
    ///   Creates a deep clone of the specified object, also creating clones of all
    ///   child objects being referenced
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A deep clone of the provided object</returns>
    public static TCloned DeepPropertyClone<TCloned>(TCloned objectToClone) {
      object objectToCloneAsObject = objectToClone;
      if(objectToCloneAsObject == null) {
        return default(TCloned);
      }

      Func<object, object> cloner = getOrCreateDeepPropertyBasedCloner(typeof(TCloned));
      return (TCloned)cloner(objectToCloneAsObject);
    }

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A shallow clone of the provided object</returns>
    public static TCloned ShallowFieldClone<TCloned>(TCloned objectToClone) {
      object objectToCloneAsObject = objectToClone;
      if(objectToCloneAsObject == null) {
        return default(TCloned);
      }

      Func<object, object> cloner = getOrCreateShallowFieldBasedCloner(typeof(TCloned));
      return (TCloned)cloner(objectToCloneAsObject);
    }

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A shallow clone of the provided object</returns>
    public static TCloned ShallowPropertyClone<TCloned>(TCloned objectToClone) {
      object objectToCloneAsObject = objectToClone;
      if(objectToCloneAsObject == null) {
        return default(TCloned);
      }

      Func<object, object> cloner = getOrCreateShallowPropertyBasedCloner(typeof(TCloned));
      return (TCloned)cloner(objectToCloneAsObject);
    }

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A shallow clone of the provided object</returns>
    TCloned ICloneFactory.ShallowFieldClone<TCloned>(TCloned objectToClone) {
      return ExpressionTreeCloner.ShallowFieldClone<TCloned>(objectToClone);
    }

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A shallow clone of the provided object</returns>
    TCloned ICloneFactory.ShallowPropertyClone<TCloned>(TCloned objectToClone) {
      return ExpressionTreeCloner.ShallowPropertyClone<TCloned>(objectToClone);
    }

    /// <summary>
    ///   Creates a deep clone of the specified object, also creating clones of all
    ///   child objects being referenced
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A deep clone of the provided object</returns>
    TCloned ICloneFactory.DeepFieldClone<TCloned>(TCloned objectToClone) {
      return ExpressionTreeCloner.DeepFieldClone<TCloned>(objectToClone);
    }

    /// <summary>
    ///   Creates a deep clone of the specified object, also creating clones of all
    ///   child objects being referenced
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A deep clone of the provided object</returns>
    TCloned ICloneFactory.DeepPropertyClone<TCloned>(TCloned objectToClone) {
      return ExpressionTreeCloner.DeepPropertyClone<TCloned>(objectToClone);
    }

#if false
    /// <summary>
    ///   Transfers the state of one object into another, creating clones of referenced objects
    /// </summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    public void DeepCopyState<TState>(TState original, TState target, bool propertyBased)
      where TState : class {
      throw new NotImplementedException();
    }

    /// <summary>
    ///   Transfers the state of one object into another, creating clones of referenced objects
    /// </summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    public void DeepCopyState<TState>(ref TState original, ref TState target, bool propertyBased)
      where TState : struct {
      throw new NotImplementedException();
    }

    /// <summary>Transfers the state of one object into another</summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    public void ShallowCopyState<TState>(TState original, TState target, bool propertyBased)
      where TState : class {
      throw new NotImplementedException();
    }

    /// <summary>Transfers the state of one object into another</summary>
    /// <typeparam name="TState">Type of the object whose sate will be transferred</typeparam>
    /// <param name="original">Original instance the state will be taken from</param>
    /// <param name="target">Target instance the state will be written to</param>
    /// <param name="propertyBased">Whether to perform a property-based state copy</param>
    public void ShallowCopyState<TState>(ref TState original, ref TState target, bool propertyBased)
      where TState : struct {
      throw new NotImplementedException();
    }

#endif

    /// <summary>
    ///   Retrieves the existing clone method for the specified type or compiles one if
    ///   none exists for the type yet
    /// </summary>
    /// <param name="clonedType">Type for which a clone method will be retrieved</param>
    /// <returns>The clone method for the specified type</returns>
    private static Func<object, object> getOrCreateShallowFieldBasedCloner(Type clonedType) {
      Func<object, object> cloner;

      if(!shallowFieldBasedCloners.TryGetValue(clonedType, out cloner)) {
        cloner = createShallowFieldBasedCloner(clonedType);
        shallowFieldBasedCloners.TryAdd(clonedType, cloner);
      }

      return cloner;
    }

    /// <summary>
    ///   Retrieves the existing clone method for the specified type or compiles one if
    ///   none exists for the type yet
    /// </summary>
    /// <param name="clonedType">Type for which a clone method will be retrieved</param>
    /// <returns>The clone method for the specified type</returns>
    private static Func<object, object> getOrCreateDeepFieldBasedCloner(Type clonedType) {
      Func<object, object> cloner;

      if(!deepFieldBasedCloners.TryGetValue(clonedType, out cloner)) {
        cloner = createDeepFieldBasedCloner(clonedType);
        deepFieldBasedCloners.TryAdd(clonedType, cloner);
      }

      return cloner;
    }

    /// <summary>
    ///   Retrieves the existing clone method for the specified type or compiles one if
    ///   none exists for the type yet
    /// </summary>
    /// <param name="clonedType">Type for which a clone method will be retrieved</param>
    /// <returns>The clone method for the specified type</returns>
    private static Func<object, object> getOrCreateShallowPropertyBasedCloner(Type clonedType) {
      Func<object, object> cloner;

      if(!shallowPropertyBasedCloners.TryGetValue(clonedType, out cloner)) {
        cloner = createShallowPropertyBasedCloner(clonedType);
        shallowPropertyBasedCloners.TryAdd(clonedType, cloner);
      }

      return cloner;
    }

    /// <summary>
    ///   Retrieves the existing clone method for the specified type or compiles one if
    ///   none exists for the type yet
    /// </summary>
    /// <param name="clonedType">Type for which a clone method will be retrieved</param>
    /// <returns>The clone method for the specified type</returns>
    private static Func<object, object> getOrCreateDeepPropertyBasedCloner(Type clonedType) {
      Func<object, object> cloner;

      if(!deepPropertyBasedCloners.TryGetValue(clonedType, out cloner)) {
        cloner = createDeepPropertyBasedCloner(clonedType);
        deepPropertyBasedCloners.TryAdd(clonedType, cloner);
      }

      return cloner;
    }

    /// <summary>Compiled cloners that perform shallow clone operations</summary>
    private static ConcurrentDictionary<Type, Func<object, object>> shallowFieldBasedCloners;
    /// <summary>Compiled cloners that perform deep clone operations</summary>
    private static ConcurrentDictionary<Type, Func<object, object>> deepFieldBasedCloners;
    /// <summary>Compiled cloners that perform shallow clone operations</summary>
    private static ConcurrentDictionary<Type, Func<object, object>> shallowPropertyBasedCloners;
    /// <summary>Compiled cloners that perform deep clone operations</summary>
    private static ConcurrentDictionary<Type, Func<object, object>> deepPropertyBasedCloners;

  }

} // namespace Nuclex.Support.Cloning

#endif // !(XBOX360 || WINDOWS_PHONE)
