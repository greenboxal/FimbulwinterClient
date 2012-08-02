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

namespace Nuclex.Support {

  /// <summary>Helper methods for enumerations</summary>
  public static class EnumHelper {

    /// <summary>Returns the highest value encountered in an enumeration</summary>
    /// <typeparam name="EnumType">
    ///   Enumeration of which the highest value will be returned
    /// </typeparam>
    /// <returns>The highest value in the enumeration</returns>
    public static EnumType GetHighestValue<EnumType>() where EnumType : IComparable {
      EnumType[] values = GetValues<EnumType>();

      // If the enumeration is empty, return nothing
      if(values.Length == 0) {
        return default(EnumType);
      }

      // Look for the highest value in the enumeration. We initialize the highest value
      // to the first enumeration value so we don't have to use some arbitrary starting
      // value which might actually appear in the enumeration.
      EnumType highestValue = values[0];
      for(int index = 1; index < values.Length; ++index) {
        if(values[index].CompareTo(highestValue) > 0) {
          highestValue = values[index];
        }
      }

      return highestValue;
    }

    /// <summary>Returns the lowest value encountered in an enumeration</summary>
    /// <typeparam name="EnumType">
    ///   Enumeration of which the lowest value will be returned
    /// </typeparam>
    /// <returns>The lowest value in the enumeration</returns>
    public static EnumType GetLowestValue<EnumType>() where EnumType : IComparable {
      EnumType[] values = GetValues<EnumType>();

      // If the enumeration is empty, return nothing
      if(values.Length == 0) {
        return default(EnumType);
      }

      // Look for the lowest value in the enumeration. We initialize the lowest value
      // to the first enumeration value so we don't have to use some arbitrary starting
      // value which might actually appear in the enumeration.
      EnumType lowestValue = values[0];
      for(int index = 1; index < values.Length; ++index) {
        if(values[index].CompareTo(lowestValue) < 0) {
          lowestValue = values[index];
        }
      }

      return lowestValue;
    }

    /// <summary>Retrieves a list of all values contained in an enumeration</summary>
    /// <typeparam name="EnumType">
    ///   Type of the enumeration whose values will be returned
    /// </typeparam>
    /// <returns>All values contained in the specified enumeration</returns>
    /// <remarks>
    ///   This method produces collectable garbage so it's best to only call it once
    ///   and cache the result.
    /// </remarks>
    public static EnumType[] GetValues<EnumType>() {
#if XBOX360 || WINDOWS_PHONE
      return GetValuesXbox360<EnumType>();
#else
      return (EnumType[])Enum.GetValues(typeof(EnumType));
#endif
    }

    /// <summary>Retrieves a list of all values contained in an enumeration</summary>
    /// <typeparam name="EnumType">
    ///   Type of the enumeration whose values will be returned
    /// </typeparam>
    /// <returns>All values contained in the specified enumeration</returns>
    internal static EnumType[] GetValuesXbox360<EnumType>() {
      Type enumType = typeof(EnumType);
      if(!enumType.IsEnum) {
        throw new ArgumentException(
          "The provided type needs to be an enumeration", "EnumType"
        );
      }
      
      // Use reflection to get all fields in the enumeration      
      FieldInfo[] fieldInfos = enumType.GetFields(
        BindingFlags.Public | BindingFlags.Static
      );

      // Create an array to hold the enumeration values and copy them over from
      // the fields we just retrieved
      EnumType[] values = new EnumType[fieldInfos.Length];
      for(int index = 0; index < fieldInfos.Length; ++index) {
        values[index] = (EnumType)fieldInfos[index].GetValue(null);
      }

      return values;
    }

  }

} // namespace Nuclex.Support