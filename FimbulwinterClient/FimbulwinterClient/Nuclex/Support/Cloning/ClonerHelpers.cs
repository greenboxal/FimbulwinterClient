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
using System.Linq;
using System.Text;
using System.Reflection;

namespace Nuclex.Support.Cloning {

  /// <summary>Contains helper methods for the cloners</summary>
  internal static class ClonerHelpers {

    /// <summary>
    ///   Returns all the fields of a type, working around a weird reflection issue
    ///   where explicitly declared fields in base classes are returned, but not
    ///   automatic property backing fields.
    /// </summary>
    /// <param name="type">Type whose fields will be returned</param>
    /// <param name="bindingFlags">Binding flags to use when querying the fields</param>
    /// <returns>All of the type's fields, including its base types</returns>
    public static FieldInfo[] GetFieldInfosIncludingBaseClasses(
      Type type, BindingFlags bindingFlags
    ) {
      FieldInfo[] fieldInfos = type.GetFields(bindingFlags);

      // If this class doesn't have a base, don't waste any time
      if(type.BaseType == typeof(object)) {
        return fieldInfos;
      } else { // Otherwise, collect all types up to the furthest base class
        var fieldInfoList = new List<FieldInfo>(fieldInfos);
        while(type.BaseType != typeof(object)) {
          type = type.BaseType;
          fieldInfos = type.GetFields(bindingFlags);

          // Look for fields we do not have listed yet and merge them into the main list
          for(int index = 0; index < fieldInfos.Length; ++index) {
            bool found = false;

            for(int searchIndex = 0; searchIndex < fieldInfoList.Count; ++searchIndex) {
              bool match =
                (fieldInfoList[searchIndex].DeclaringType == fieldInfos[index].DeclaringType) &&
                (fieldInfoList[searchIndex].Name == fieldInfos[index].Name);

              if(match) {
                found = true;
                break;
              }
            }

            if(!found) {
              fieldInfoList.Add(fieldInfos[index]);
            }
          }
        }

        return fieldInfoList.ToArray();
      }
    }

  }

} // namespace Nuclex.Support.Cloning
