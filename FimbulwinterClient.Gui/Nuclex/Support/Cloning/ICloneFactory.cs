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

  /// <summary>Constructs new objects by cloning existing objects</summary>
  public interface ICloneFactory {

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A shallow clone of the provided object</returns>
    /// <remarks>
    ///   Field-based clones are guaranteed to be complete - there will be no missed
    ///   members. This type of clone is also able to clone types that do not provide
    ///   a default constructor.
    /// </remarks>
    TCloned ShallowFieldClone<TCloned>(TCloned objectToClone);

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A shallow clone of the provided object</returns>
    /// <remarks>
    ///   <para>
    ///     A property-based clone is useful if you're using dynamically generated proxies,
    ///     such as when working with entities returned by an ORM like NHibernate.
    ///     When not using a property-based clone, internal proxy fields would be cloned
    ///     and might cause problems with the ORM.
    ///   </para>
    ///   <para>
    ///     Property-based clones require a default constructor because there's no guarantee
    ///     that all fields will are assignable through properties and starting with
    ///     an uninitialized object is likely to end up with a broken clone.
    ///   </para>
    /// </remarks>
    TCloned ShallowPropertyClone<TCloned>(TCloned objectToClone);

    /// <summary>
    ///   Creates a deep clone of the specified object, also creating clones of all
    ///   child objects being referenced
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A deep clone of the provided object</returns>
    /// <remarks>
    ///   Field-based clones are guaranteed to be complete - there will be no missed
    ///   members. This type of clone is also able to clone types that do not provide
    ///   a default constructor.
    /// </remarks>
    TCloned DeepFieldClone<TCloned>(TCloned objectToClone);

    /// <summary>
    ///   Creates a deep clone of the specified object, also creating clones of all
    ///   child objects being referenced
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A deep clone of the provided object</returns>
    /// <remarks>
    ///   <para>
    ///     A property-based clone is useful if you're using dynamically generated proxies,
    ///     such as when working with entities returned by an ORM like NHibernate.
    ///     When not using a property-based clone, internal proxy fields would be cloned
    ///     and might cause problems with the ORM.
    ///   </para>
    ///   <para>
    ///     Property-based clones require a default constructor because there's no guarantee
    ///     that all fields will are assignable through properties and starting with
    ///     an uninitialized object is likely to end up with a broken clone.
    ///   </para>
    /// </remarks>
    TCloned DeepPropertyClone<TCloned>(TCloned objectToClone);

  }

} // namespace Nuclex.Support.Cloning
