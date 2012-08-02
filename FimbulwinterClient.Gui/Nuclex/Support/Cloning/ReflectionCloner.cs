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
using System.Reflection;
using System.Runtime.Serialization;

namespace Nuclex.Support.Cloning {

  /// <summary>Clones objects using reflection</summary>
  /// <remarks>
  ///   <para>
  ///     This type of cloning is a lot faster than cloning by serialization and
  ///     incurs no set-up cost, but requires cloned types to provide a default
  ///     constructor in order to work.
  ///   </para>
  /// </remarks>
  public class ReflectionCloner : ICloneFactory {

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A shallow clone of the provided object</returns>
    public static TCloned ShallowFieldClone<TCloned>(TCloned objectToClone) {
      Type originalType = objectToClone.GetType();
      if(originalType.IsPrimitive || (originalType == typeof(string))) {
        return objectToClone; // Being value types, primitives are copied by default
      } else if(originalType.IsArray) {
        return (TCloned)shallowCloneArray(objectToClone);
      } else if(originalType.IsValueType) {
        return objectToClone; // Value types can be copied directly
      } else {
        return (TCloned)shallowCloneComplexFieldBased(objectToClone);
      }
    }

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A shallow clone of the provided object</returns>
    public static TCloned ShallowPropertyClone<TCloned>(TCloned objectToClone) {
      Type originalType = objectToClone.GetType();
      if(originalType.IsPrimitive || (originalType == typeof(string))) {
        return objectToClone; // Being value types, primitives are copied by default
      } else if(originalType.IsArray) {
        return (TCloned)shallowCloneArray(objectToClone);
      } else if(originalType.IsValueType) {
        return (TCloned)shallowCloneComplexPropertyBased(objectToClone);
      } else {
        return (TCloned)shallowCloneComplexPropertyBased(objectToClone);
      }
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
      if(objectToClone == null) {
        return default(TCloned);
      } else {
        return (TCloned)deepCloneSingleFieldBased(objectToCloneAsObject);
      }
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
      if(objectToClone == null) {
        return default(TCloned);
      } else {
        return (TCloned)deepCloneSinglePropertyBased(objectToCloneAsObject);
      }
    }

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A shallow clone of the provided object</returns>
    TCloned ICloneFactory.ShallowFieldClone<TCloned>(TCloned objectToClone) {
      if(typeof(TCloned).IsClass || typeof(TCloned).IsArray) {
        if(ReferenceEquals(objectToClone, null)) {
          return default(TCloned);
        }
      }
      return ReflectionCloner.ShallowFieldClone<TCloned>(objectToClone);
    }

    /// <summary>
    ///   Creates a shallow clone of the specified object, reusing any referenced objects
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A shallow clone of the provided object</returns>
    TCloned ICloneFactory.ShallowPropertyClone<TCloned>(TCloned objectToClone) {
      if(typeof(TCloned).IsClass || typeof(TCloned).IsArray) {
        if(ReferenceEquals(objectToClone, null)) {
          return default(TCloned);
        }
      }
      return ReflectionCloner.ShallowPropertyClone<TCloned>(objectToClone);
    }

    /// <summary>
    ///   Creates a deep clone of the specified object, also creating clones of all
    ///   child objects being referenced
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A deep clone of the provided object</returns>
    TCloned ICloneFactory.DeepFieldClone<TCloned>(TCloned objectToClone) {
      return ReflectionCloner.DeepFieldClone<TCloned>(objectToClone);
    }

    /// <summary>
    ///   Creates a deep clone of the specified object, also creating clones of all
    ///   child objects being referenced
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned</typeparam>
    /// <param name="objectToClone">Object that will be cloned</param>
    /// <returns>A deep clone of the provided object</returns>
    TCloned ICloneFactory.DeepPropertyClone<TCloned>(TCloned objectToClone) {
      return ReflectionCloner.DeepPropertyClone<TCloned>(objectToClone);
    }

    /// <summary>Clones a complex type using field-based value transfer</summary>
    /// <param name="original">Original instance that will be cloned</param>
    /// <returns>A clone of the original instance</returns>
    private static object shallowCloneComplexFieldBased(object original) {
      Type originalType = original.GetType();
#if (XBOX360 || WINDOWS_PHONE)
      object clone = Activator.CreateInstance(originalType);
#else
      object clone = FormatterServices.GetUninitializedObject(originalType);
#endif

      FieldInfo[] fieldInfos = ClonerHelpers.GetFieldInfosIncludingBaseClasses(
        originalType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
      );
      for(int index = 0; index < fieldInfos.Length; ++index) {
        FieldInfo fieldInfo = fieldInfos[index];
        object originalValue = fieldInfo.GetValue(original);
        if(originalValue != null) {
          // Everything's just directly assigned in a shallow clone
          fieldInfo.SetValue(clone, originalValue);
        }
      }

      return clone;
    }

    /// <summary>Clones a complex type using property-based value transfer</summary>
    /// <param name="original">Original instance that will be cloned</param>
    /// <returns>A clone of the original instance</returns>
    private static object shallowCloneComplexPropertyBased(object original) {
      Type originalType = original.GetType();
      object clone = Activator.CreateInstance(originalType);

      PropertyInfo[] propertyInfos = originalType.GetProperties(
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );
      for(int index = 0; index < propertyInfos.Length; ++index) {
        PropertyInfo propertyInfo = propertyInfos[index];
        if(propertyInfo.CanRead && propertyInfo.CanWrite) {
          Type propertyType = propertyInfo.PropertyType;
          object originalValue = propertyInfo.GetValue(original, null);
          if(originalValue != null) {
            if(propertyType.IsPrimitive || (propertyType == typeof(string))) {
              // Primitive types can be assigned directly
              propertyInfo.SetValue(clone, originalValue, null);
            } else if(propertyType.IsValueType) {
              // Value types are seen as part of the original type and are thus recursed into
              propertyInfo.SetValue(clone, shallowCloneComplexPropertyBased(originalValue), null);
            } else if(propertyType.IsArray) { // Arrays are assigned directly in a shallow clone
              propertyInfo.SetValue(clone, originalValue, null);
            } else { // Complex types are directly assigned without creating a copy
              propertyInfo.SetValue(clone, originalValue, null);
            }
          }
        }
      }

      return clone;
    }

    /// <summary>Clones an array using field-based value transfer</summary>
    /// <param name="original">Original array that will be cloned</param>
    /// <returns>A clone of the original array</returns>
    private static object shallowCloneArray(object original) {
      return ((Array)original).Clone();
    }

    /// <summary>Copies a single object using field-based value transfer</summary>
    /// <param name="original">Original object that will be cloned</param>
    /// <returns>A clone of the original object</returns>
    private static object deepCloneSingleFieldBased(object original) {
      Type originalType = original.GetType();
      if(originalType.IsPrimitive || (originalType == typeof(string))) {
        return original; // Creates another box, does not reference boxed primitive
      } else if(originalType.IsArray) {
        return deepCloneArrayFieldBased((Array)original, originalType.GetElementType());
      } else {
        return deepCloneComplexFieldBased(original);
      }
    }

    /// <summary>Clones a complex type using field-based value transfer</summary>
    /// <param name="original">Original instance that will be cloned</param>
    /// <returns>A clone of the original instance</returns>
    private static object deepCloneComplexFieldBased(object original) {
      Type originalType = original.GetType();
#if (XBOX360 || WINDOWS_PHONE)
      object clone = Activator.CreateInstance(originalType);
#else
      object clone = FormatterServices.GetUninitializedObject(originalType);
#endif

      FieldInfo[] fieldInfos = ClonerHelpers.GetFieldInfosIncludingBaseClasses(
        originalType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
      );
      for(int index = 0; index < fieldInfos.Length; ++index) {
        FieldInfo fieldInfo = fieldInfos[index];
        Type fieldType = fieldInfo.FieldType;
        object originalValue = fieldInfo.GetValue(original);
        if(originalValue != null) {
          // Primitive types can be assigned directly
          if(fieldType.IsPrimitive || (fieldType == typeof(string))) {
            fieldInfo.SetValue(clone, originalValue);
          } else if(fieldType.IsArray) { // Arrays need to be cloned element-by-element
            fieldInfo.SetValue(
              clone,
              deepCloneArrayFieldBased((Array)originalValue, fieldType.GetElementType())
            );
          } else { // Complex types need to be cloned member-by-member
            fieldInfo.SetValue(clone, deepCloneSingleFieldBased(originalValue));
          }
        }
      }

      return clone;
    }

    /// <summary>Clones an array using field-based value transfer</summary>
    /// <param name="original">Original array that will be cloned</param>
    /// <param name="elementType">Type of elements the original array contains</param>
    /// <returns>A clone of the original array</returns>
    private static object deepCloneArrayFieldBased(Array original, Type elementType) {
      if(elementType.IsPrimitive || (elementType == typeof(string))) {
        return original.Clone();
      }

      int dimensionCount = original.Rank;

      // Find out the length of each of the array's dimensions, also calculate how
      // many elements there are in the array in total.
      var lengths = new int[dimensionCount];
      int totalElementCount = 0;
      for(int index = 0; index < dimensionCount; ++index) {
        lengths[index] = original.GetLength(index);
        if(index == 0) {
          totalElementCount = lengths[index];
        } else {
          totalElementCount *= lengths[index];
        }
      }

      // Knowing the number of dimensions and the length of each dimension, we can
      // create another array of the exact same sizes.
      Array clone = Array.CreateInstance(elementType, lengths);

      // If this is a one-dimensional array (most common type), do an optimized copy
      // directly specifying the indices
      if(dimensionCount == 1) {

        // Clone each element of the array directly
        for(int index = 0; index < totalElementCount; ++index) {
          object originalElement = original.GetValue(index);
          if(originalElement != null) {
            clone.SetValue(deepCloneSingleFieldBased(originalElement), index);
          }
        }

      } else { // Otherwise use the generic code for multi-dimensional arrays

        var indices = new int[dimensionCount];
        for(int index = 0; index < totalElementCount; ++index) {

          // Determine the index for each of the array's dimensions
          int elementIndex = index;
          for(int dimensionIndex = dimensionCount - 1; dimensionIndex >= 0; --dimensionIndex) {
            indices[dimensionIndex] = elementIndex % lengths[dimensionIndex];
            elementIndex /= lengths[dimensionIndex];
          }

          // Clone the current array element
          object originalElement = original.GetValue(indices);
          if(originalElement != null) {
            clone.SetValue(deepCloneSingleFieldBased(originalElement), indices);
          }

        }

      }

      return clone;
    }

    /// <summary>Copies a single object using property-based value transfer</summary>
    /// <param name="original">Original object that will be cloned</param>
    /// <returns>A clone of the original object</returns>
    private static object deepCloneSinglePropertyBased(object original) {
      Type originalType = original.GetType();
      if(originalType.IsPrimitive || (originalType == typeof(string))) {
        return original; // Creates another box, does not reference boxed primitive
      } else if(originalType.IsArray) {
        return deepCloneArrayPropertyBased((Array)original, originalType.GetElementType());
      } else {
        return deepCloneComplexPropertyBased(original);
      }
    }

    /// <summary>Clones a complex type using property-based value transfer</summary>
    /// <param name="original">Original instance that will be cloned</param>
    /// <returns>A clone of the original instance</returns>
    private static object deepCloneComplexPropertyBased(object original) {
      Type originalType = original.GetType();
      object clone = Activator.CreateInstance(originalType);

      PropertyInfo[] propertyInfos = originalType.GetProperties(
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );
      for(int index = 0; index < propertyInfos.Length; ++index) {
        PropertyInfo propertyInfo = propertyInfos[index];
        if(propertyInfo.CanRead && propertyInfo.CanWrite) {
          Type propertyType = propertyInfo.PropertyType;
          object originalValue = propertyInfo.GetValue(original, null);
          if(originalValue != null) {
            if(propertyType.IsPrimitive || (propertyType == typeof(string))) {
              // Primitive types can be assigned directly
              propertyInfo.SetValue(clone, originalValue, null);
            } else if(propertyType.IsArray) { // Arrays need to be cloned element-by-element
              propertyInfo.SetValue(
                clone,
                deepCloneArrayPropertyBased((Array)originalValue, propertyType.GetElementType()),
                null
              );
            } else { // Complex types need to be cloned member-by-member
              propertyInfo.SetValue(clone, deepCloneSinglePropertyBased(originalValue), null);
            }
          }
        }
      }

      return clone;
    }

    /// <summary>Clones an array using property-based value transfer</summary>
    /// <param name="original">Original array that will be cloned</param>
    /// <param name="elementType">Type of elements the original array contains</param>
    /// <returns>A clone of the original array</returns>
    private static object deepCloneArrayPropertyBased(Array original, Type elementType) {
      if(elementType.IsPrimitive || (elementType == typeof(string))) {
        return original.Clone();
      }

      int dimensionCount = original.Rank;

      // Find out the length of each of the array's dimensions, also calculate how
      // many elements there are in the array in total.
      var lengths = new int[dimensionCount];
      int totalElementCount = 0;
      for(int index = 0; index < dimensionCount; ++index) {
        lengths[index] = original.GetLength(index);
        if(index == 0) {
          totalElementCount = lengths[index];
        } else {
          totalElementCount *= lengths[index];
        }
      }

      // Knowing the number of dimensions and the length of each dimension, we can
      // create another array of the exact same sizes.
      Array clone = Array.CreateInstance(elementType, lengths);

      // If this is a one-dimensional array (most common type), do an optimized copy
      // directly specifying the indices
      if(dimensionCount == 1) {

        // Clone each element of the array directly
        for(int index = 0; index < totalElementCount; ++index) {
          object originalElement = original.GetValue(index);
          if(originalElement != null) {
            clone.SetValue(deepCloneSinglePropertyBased(originalElement), index);
          }
        }

      } else { // Otherwise use the generic code for multi-dimensional arrays

        var indices = new int[dimensionCount];
        for(int index = 0; index < totalElementCount; ++index) {

          // Determine the index for each of the array's dimensions
          int elementIndex = index;
          for(int dimensionIndex = dimensionCount - 1; dimensionIndex >= 0; --dimensionIndex) {
            indices[dimensionIndex] = elementIndex % lengths[dimensionIndex];
            elementIndex /= lengths[dimensionIndex];
          }

          // Clone the current array element
          object originalElement = original.GetValue(indices);
          if(originalElement != null) {
            clone.SetValue(deepCloneSinglePropertyBased(originalElement), indices);
          }

        }

      }

      return clone;
    }

  }

} // namespace Nuclex.Support.Cloning
