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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Nuclex.Support.Cloning {

  partial class ExpressionTreeCloner : ICloneFactory {

    /// <summary>Compiles a method that creates a deep clone of an object</summary>
    /// <param name="clonedType">Type for which a clone method will be created</param>
    /// <returns>A method that clones an object of the provided type</returns>
    /// <remarks>
    ///   <para>
    ///     The 'null' check is supposed to take place before running the cloner. This
    ///     avoids having redundant 'null' checks on nested types - first before calling
    ///     GetType() on the property to be cloned and second when runner the matching
    ///     cloner for the property.
    ///   </para>
    ///   <para>
    ///     This design also enables the cloning of nested value types (which can never
    ///     be null) without any null check whatsoever.
    ///   </para>
    /// </remarks>
    private static Func<object, object> createDeepPropertyBasedCloner(Type clonedType) {
      ParameterExpression original = Expression.Parameter(typeof(object), "original");

      var transferExpressions = new List<Expression>();
      var variables = new List<ParameterExpression>();

      if(clonedType.IsPrimitive || (clonedType == typeof(string))) {
        // Primitives and strings are copied on direct assignment
        transferExpressions.Add(original);
      } else if(clonedType.IsArray) {
        // Arrays need to be cloned element-by-element
        Type elementType = clonedType.GetElementType();

        if(elementType.IsPrimitive || (elementType == typeof(string))) {
          // For primitive arrays, the Array.Clone() method is sufficient
          transferExpressions.Add(
            generatePropertyBasedPrimitiveArrayTransferExpressions(
              clonedType, original, variables, transferExpressions
            )
          );
        } else {
          // To access the properties of the original type, we need it to be of the actual
          // type instead of an object, so perform a downcast
          ParameterExpression typedOriginal = Expression.Variable(clonedType);
          variables.Add(typedOriginal);
          transferExpressions.Add(
            Expression.Assign(typedOriginal, Expression.Convert(original, clonedType))
          );

          // Arrays of complex types require manual cloning
          transferExpressions.Add(
            generatePropertyBasedComplexArrayTransferExpressions(
              clonedType, typedOriginal, variables, transferExpressions
            )
          );
        }
      } else {
        // We need a variable to hold the clone because due to the assignments it
        // won't be last in the block when we're finished
        ParameterExpression clone = Expression.Variable(clonedType);
        variables.Add(clone);

        // Give it a new instance of the type being cloned
        transferExpressions.Add(Expression.Assign(clone, Expression.New(clonedType)));

        // To access the properties of the original type, we need it to be of the actual
        // type instead of an object, so perform a downcast
        ParameterExpression typedOriginal = Expression.Variable(clonedType);
        variables.Add(typedOriginal);
        transferExpressions.Add(
          Expression.Assign(typedOriginal, Expression.Convert(original, clonedType))
        );

        // Generate the expressions required to transfer the type property by property
        generatePropertyBasedComplexTypeTransferExpressions(
          clonedType, typedOriginal, clone, variables, transferExpressions
        );

        // Make sure the clone is the last thing in the block to set the return value
        transferExpressions.Add(clone);
      }

      // Turn all transfer expressions into a single block if necessary
      Expression resultExpression;
      if((transferExpressions.Count == 1) && (variables.Count == 0)) {
        resultExpression = transferExpressions[0];
      } else {
        resultExpression = Expression.Block(variables, transferExpressions);
      }

      // Value types require manual boxing
      if(clonedType.IsValueType) {
        resultExpression = Expression.Convert(resultExpression, typeof(object));
      }

      return Expression.Lambda<Func<object, object>>(resultExpression, original).Compile();
    }

    /// <summary>Compiles a method that creates a deep clone of an object</summary>
    /// <param name="clonedType">Type for which a clone method will be created</param>
    /// <returns>A method that clones an object of the provided type</returns>
    /// <remarks>
    ///   <para>
    ///     The 'null' check is supposed to take place before running the cloner. This
    ///     avoids having redundant 'null' checks on nested types - first before calling
    ///     GetType() on the property to be cloned and second when runner the matching
    ///     cloner for the property.
    ///   </para>
    ///   <para>
    ///     This design also enables the cloning of nested value types (which can never
    ///     be null) without any null check whatsoever.
    ///   </para>
    /// </remarks>
    private static Func<object, object> createShallowPropertyBasedCloner(Type clonedType) {
      ParameterExpression original = Expression.Parameter(typeof(object), "original");

      var transferExpressions = new List<Expression>();
      var variables = new List<ParameterExpression>();

      if(clonedType.IsPrimitive || (clonedType == typeof(string))) {
        // Primitives and strings are copied on direct assignment
        transferExpressions.Add(original);
      } else if(clonedType.IsArray) {
        transferExpressions.Add(
          generatePropertyBasedPrimitiveArrayTransferExpressions(
            clonedType, original, variables, transferExpressions
          )
        );
      } else {
        // We need a variable to hold the clone because due to the assignments it
        // won't be last in the block when we're finished
        ParameterExpression clone = Expression.Variable(clonedType);
        variables.Add(clone);
        transferExpressions.Add(Expression.Assign(clone, Expression.New(clonedType)));

        // To access the properties of the original type, we need it to be of the actual
        // type instead of an object, so perform a downcast
        ParameterExpression typedOriginal = Expression.Variable(clonedType);
        variables.Add(typedOriginal);
        transferExpressions.Add(
          Expression.Assign(typedOriginal, Expression.Convert(original, clonedType))
        );

        generateShallowPropertyBasedComplexCloneExpressions(
          clonedType, typedOriginal, clone, transferExpressions, variables
        );

        // Make sure the clone is the last thing in the block to set the return value
        transferExpressions.Add(clone);
      }

      // Turn all transfer expressions into a single block if necessary
      Expression resultExpression;
      if((transferExpressions.Count == 1) && (variables.Count == 0)) {
        resultExpression = transferExpressions[0];
      } else {
        resultExpression = Expression.Block(variables, transferExpressions);
      }

      // Value types require manual boxing
      if(clonedType.IsValueType) {
        resultExpression = Expression.Convert(resultExpression, typeof(object));
      }

      return Expression.Lambda<Func<object, object>>(resultExpression, original).Compile();
    }

    /// <summary>
    ///   Generates expressions to transfer the properties of a complex value type
    /// </summary>
    /// <param name="clonedType">Complex value type that will be cloned</param>
    /// <param name="original">Original instance whose properties will be cloned</param>
    /// <param name="clone">Target instance into which the properties will be copied</param>
    /// <param name="transferExpressions">Receives the value transfer expressions</param>
    /// <param name="variables">Receives temporary variables used during the clone</param>
    private static void generateShallowPropertyBasedComplexCloneExpressions(
      Type clonedType,
      ParameterExpression original,
      ParameterExpression clone,
      ICollection<Expression> transferExpressions,
      ICollection<ParameterExpression> variables
    ) {
      // Enumerate all of the type's properties and generate transfer expressions for each
      PropertyInfo[] propertyInfos = clonedType.GetProperties(
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );
      for(int index = 0; index < propertyInfos.Length; ++index) {
        PropertyInfo propertyInfo = propertyInfos[index];
        if(propertyInfo.CanRead && propertyInfo.CanWrite) {
          Type propertyType = propertyInfo.PropertyType;

          if(propertyType.IsPrimitive || (propertyType == typeof(string))) {
            transferExpressions.Add(
              Expression.Assign(
                Expression.Property(clone, propertyInfo),
                Expression.Property(original, propertyInfo)
              )
            );
          } else if(propertyType.IsValueType) {
            ParameterExpression originalProperty = Expression.Variable(propertyType);
            variables.Add(originalProperty);
            transferExpressions.Add(
              Expression.Assign(
                originalProperty, Expression.Property(original, propertyInfo)
              )
            );

            ParameterExpression clonedProperty = Expression.Variable(propertyType);
            variables.Add(clonedProperty);
            transferExpressions.Add(
              Expression.Assign(clonedProperty, Expression.New(propertyType))
            );

            generateShallowPropertyBasedComplexCloneExpressions(propertyType, originalProperty, clonedProperty, transferExpressions, variables);

            transferExpressions.Add(
              Expression.Assign(
                Expression.Property(clone, propertyInfo), clonedProperty
              )
            );
          } else {
            transferExpressions.Add(
              Expression.Assign(
                Expression.Property(clone, propertyInfo),
                Expression.Property(original, propertyInfo)
              )
            );
          }
        }
      }
    }

    /// <summary>
    ///   Generates state transfer expressions to copy an array of primitive types
    /// </summary>
    /// <param name="clonedType">Type of array that will be cloned</param>
    /// <param name="original">Variable expression for the original array</param>
    /// <param name="variables">Receives variables used by the transfer expressions</param>
    /// <param name="transferExpressions">Receives the generated transfer expressions</param>
    /// <returns>The variable holding the cloned array</returns>
    private static Expression generatePropertyBasedPrimitiveArrayTransferExpressions(
      Type clonedType,
      Expression original,
      ICollection<ParameterExpression> variables,
      ICollection<Expression> transferExpressions
    ) {
      MethodInfo arrayCloneMethodInfo = typeof(Array).GetMethod("Clone");
      return Expression.Convert(
        Expression.Call(
          Expression.Convert(original, typeof(Array)), arrayCloneMethodInfo
        ),
        clonedType
      );
    }

    /// <summary>
    ///   Generates state transfer expressions to copy an array of complex types
    /// </summary>
    /// <param name="clonedType">Type of array that will be cloned</param>
    /// <param name="original">Variable expression for the original array</param>
    /// <param name="variables">Receives variables used by the transfer expressions</param>
    /// <param name="transferExpressions">Receives the generated transfer expressions</param>
    /// <returns>The variable holding the cloned array</returns>
    private static ParameterExpression generatePropertyBasedComplexArrayTransferExpressions(
      Type clonedType,
      Expression original,
      IList<ParameterExpression> variables,
      ICollection<Expression> transferExpressions
    ) {
      // We need a temporary variable in order to transfer the elements of the array
      ParameterExpression clone = Expression.Variable(clonedType);
      variables.Add(clone);

      int dimensionCount = clonedType.GetArrayRank();
      int baseVariableIndex = variables.Count;
      Type elementType = clonedType.GetElementType();

      var lengths = new List<ParameterExpression>();
      var indexes = new List<ParameterExpression>();
      var labels = new List<LabelTarget>();

      // Retrieve the length of each of the array's dimensions
      MethodInfo arrayGetLengthMethodInfo = typeof(Array).GetMethod("GetLength");
      for(int index = 0; index < dimensionCount; ++index) {

        // Obtain the length of the array in the current dimension
        lengths.Add(Expression.Variable(typeof(int)));
        variables.Add(lengths[index]);
        transferExpressions.Add(
          Expression.Assign(
            lengths[index],
            Expression.Call(
              original, arrayGetLengthMethodInfo, Expression.Constant(index)
            )
          )
        );

        // Set up a variable to index the array in this dimension
        indexes.Add(Expression.Variable(typeof(int)));
        variables.Add(indexes[index]);

        // Also set up a label than can be used to break out of the dimension's
        // transfer loop
        labels.Add(Expression.Label());

      }

      // Create a new (empty) array with the same dimensions and lengths as the original
      transferExpressions.Add(
        Expression.Assign(clone, Expression.NewArrayBounds(elementType, lengths))
      );

      // Initialize the indexer of the outer loop (indexers are initialized one up
      // in the loops (ie. before the loop using it begins), so we have to set this
      // one outside of the loop building code.
      transferExpressions.Add(
        Expression.Assign(indexes[0], Expression.Constant(0))
      );

      // Build the nested loops (one for each dimension) from the inside out
      Expression innerLoop = null;
      for(int index = dimensionCount - 1; index >= 0; --index) {
        var loopVariables = new List<ParameterExpression>();
        var loopExpressions = new List<Expression>();

        // If we reached the end of the current array dimension, break the loop
        loopExpressions.Add(
          Expression.IfThen(
            Expression.GreaterThanOrEqual(indexes[index], lengths[index]),
            Expression.Break(labels[index])
          )
        );

        if(innerLoop == null) {
          // The innermost loop clones an actual array element

          if(elementType.IsPrimitive || (elementType == typeof(string))) {
            // Primitive array elements can be copied by simple assignment. This case
            // should not occur since Array.Clone() should be used instead.
            loopExpressions.Add(
              Expression.Assign(
                Expression.ArrayAccess(clone, indexes),
                Expression.ArrayAccess(original, indexes)
              )
            );
          } else if(elementType.IsValueType) {
            // Arrays of complex value types can be transferred by assigning all properties
            // of the source array element to the destination array element (cloning
            // any nested reference types appropriately)
            generatePropertyBasedComplexTypeTransferExpressions(
              elementType,
              Expression.ArrayAccess(original, indexes),
              Expression.ArrayAccess(clone, indexes),
              variables,
              loopExpressions
            );

          } else {
            // Arrays of reference types need to be cloned by creating a new instance
            // of the reference type and then transferring the properties over
            ParameterExpression originalElement = Expression.Variable(elementType);
            loopVariables.Add(originalElement);

            loopExpressions.Add(
              Expression.Assign(originalElement, Expression.ArrayAccess(original, indexes))
            );

            var nestedVariables = new List<ParameterExpression>();
            var nestedTransferExpressions = new List<Expression>();

            // A nested array should be cloned by directly creating a new array (not invoking
            // a cloner) since you cannot derive from an array
            if(elementType.IsArray) {
              Expression clonedElement;

              Type nestedElementType = elementType.GetElementType();
              if(nestedElementType.IsPrimitive || (nestedElementType == typeof(string))) {
                clonedElement = generatePropertyBasedPrimitiveArrayTransferExpressions(
                  elementType, originalElement, nestedVariables, nestedTransferExpressions
                );
              } else {
                clonedElement = generatePropertyBasedComplexArrayTransferExpressions(
                  elementType, originalElement, nestedVariables, nestedTransferExpressions
                );
              }
              nestedTransferExpressions.Add(
                Expression.Assign(Expression.ArrayAccess(clone, indexes), clonedElement)
              );
            } else {
              // Complex types are cloned by checking their actual, concrete type (properties
              // may be typed to an interface or base class) and requesting a cloner for that
              // type during runtime
              MethodInfo getOrCreateClonerMethodInfo = typeof(ExpressionTreeCloner).GetMethod(
                "getOrCreateDeepPropertyBasedCloner",
                BindingFlags.NonPublic | BindingFlags.Static
              );
              MethodInfo getTypeMethodInfo = typeof(object).GetMethod("GetType");
              MethodInfo invokeMethodInfo = typeof(Func<object, object>).GetMethod("Invoke");

              // Generate expressions to do this:
              //   clone.SomeProperty = getOrCreateDeepPropertyBasedCloner(
              //     original.SomeProperty.GetType()
              //   ).Invoke(original.SomeProperty);
              nestedTransferExpressions.Add(
                Expression.Assign(
                  Expression.ArrayAccess(clone, indexes),
                  Expression.Convert(
                    Expression.Call(
                      Expression.Call(
                        getOrCreateClonerMethodInfo,
                        Expression.Call(originalElement, getTypeMethodInfo)
                      ),
                      invokeMethodInfo,
                      originalElement
                    ),
                    elementType
                  )
                )
              );
            }

            // Whether array-in-array of reference-type-in-array, we need a null check before
            // doing anything to avoid NullReferenceExceptions for unset members
            loopExpressions.Add(
              Expression.IfThen(
                Expression.NotEqual(originalElement, Expression.Constant(null)),
                Expression.Block(
                  nestedVariables,
                  nestedTransferExpressions
                )
              )
            );
          }

        } else {
          // Outer loops of any level just reset the inner loop's indexer and execute
          // the inner loop
          loopExpressions.Add(
            Expression.Assign(indexes[index + 1], Expression.Constant(0))
          );
          loopExpressions.Add(innerLoop);
        }

        // Each time we executed the loop instructions, increment the indexer
        loopExpressions.Add(Expression.PreIncrementAssign(indexes[index]));

        // Build the loop using the expressions recorded above
        innerLoop = Expression.Loop(
          Expression.Block(loopVariables, loopExpressions),
          labels[index]
        );
      }

      // After the loop builder has finished, the innerLoop variable contains
      // the entire hierarchy of nested loops, so add this to the clone expressions.
      transferExpressions.Add(innerLoop);

      return clone;
    }

    /// <summary>Generates state transfer expressions to copy a complex type</summary>
    /// <param name="clonedType">Complex type that will be cloned</param>
    /// <param name="original">Variable expression for the original instance</param>
    /// <param name="clone">Variable expression for the cloned instance</param>
    /// <param name="variables">Receives variables used by the transfer expressions</param>
    /// <param name="transferExpressions">Receives the generated transfer expressions</param>
    private static void generatePropertyBasedComplexTypeTransferExpressions(
      Type clonedType, // Actual, concrete type (not declared type)
      Expression original, // Expected to be an object
      Expression clone,	// As actual, concrete type
      IList<ParameterExpression> variables,
      ICollection<Expression> transferExpressions
    ) {
      // Enumerate all of the type's properties and generate transfer expressions for each
      PropertyInfo[] propertyInfos = clonedType.GetProperties(
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );
      for(int index = 0; index < propertyInfos.Length; ++index) {
        PropertyInfo propertyInfo = propertyInfos[index];
        if(propertyInfo.CanRead && propertyInfo.CanWrite) {
          Type propertyType = propertyInfo.PropertyType;

          if(propertyType.IsPrimitive || (propertyType == typeof(string))) {
            // Primitive types and strings can be transferred by simple assignment
            transferExpressions.Add(
              Expression.Assign(
                Expression.Property(clone, propertyInfo),
                Expression.Property(original, propertyInfo)
              )
            );
          } else if(propertyType.IsValueType) {
            ParameterExpression originalProperty = Expression.Variable(propertyType);
            variables.Add(originalProperty);
            ParameterExpression clonedProperty = Expression.Variable(propertyType);
            variables.Add(clonedProperty);

            transferExpressions.Add(
              Expression.Assign(
                originalProperty, Expression.Property(original, propertyInfo)
              )
            );
            transferExpressions.Add(
              Expression.Assign(clonedProperty, Expression.New(propertyType))
            );

            // A nested value type is part of the parent and will have its properties directly
            // assigned without boxing, new instance creation or anything like that.
            generatePropertyBasedComplexTypeTransferExpressions(
              propertyType,
              originalProperty,
              clonedProperty,
              variables,
              transferExpressions
            );

            transferExpressions.Add(
              Expression.Assign(
                Expression.Property(clone, propertyInfo),
                clonedProperty
              )
            );

          } else {
            generatePropertyBasedReferenceTypeTransferExpressions(
              original, clone, transferExpressions, variables, propertyInfo, propertyType
            );
          }
        }
      }
    }

    /// <summary>
    ///   Generates the expressions to transfer a reference type (array or class)
    /// </summary>
    /// <param name="original">Original value that will be cloned</param>
    /// <param name="clone">Variable that will receive the cloned value</param>
    /// <param name="transferExpressions">
    ///   Receives the expression generated to transfer the values
    /// </param>
    /// <param name="variables">Receives variables used by the transfer expressions</param>
    /// <param name="propertyInfo">Reflection informations about the property being cloned</param>
    /// <param name="propertyType">Type of the property being cloned</param>
    private static void generatePropertyBasedReferenceTypeTransferExpressions(
      Expression original,
      Expression clone,
      ICollection<Expression> transferExpressions,
      ICollection<ParameterExpression> variables,
      PropertyInfo propertyInfo,
      Type propertyType
    ) {
      ParameterExpression originalProperty = Expression.Variable(propertyType);
      variables.Add(originalProperty);

      transferExpressions.Add(
        Expression.Assign(originalProperty, Expression.Property(original, propertyInfo))
      );

      // Reference types and arrays require special care because they can be null,
      // so gather the transfer expressions in a separate block for the null check
      var propertyTransferExpressions = new List<Expression>();
      var propertyVariables = new List<ParameterExpression>();

      if(propertyType.IsArray) {
        // Arrays need to be cloned element-by-element
        Expression propertyClone;

        Type elementType = propertyType.GetElementType();
        if(elementType.IsPrimitive || (elementType == typeof(string))) {
          // For primitive arrays, the Array.Clone() method is sufficient
          propertyClone = generatePropertyBasedPrimitiveArrayTransferExpressions(
            propertyType,
            originalProperty,
            propertyVariables,
            propertyTransferExpressions
          );
        } else {
          // Arrays of complex types require manual cloning
          propertyClone = generatePropertyBasedComplexArrayTransferExpressions(
            propertyType,
            originalProperty,
            propertyVariables,
            propertyTransferExpressions
          );
        }

        // Add the assignment to the transfer expressions. The array transfer expression
        // generator will either have set up a temporary variable to hold the array or
        // returned the conversion expression straight away
        propertyTransferExpressions.Add(
          Expression.Assign(Expression.Property(clone, propertyInfo), propertyClone)
        );
      } else {
        // Complex types are cloned by checking their actual, concrete type (properties
        // may be typed to an interface or base class) and requesting a cloner for that
        // type during runtime
        MethodInfo getOrCreateClonerMethodInfo = typeof(ExpressionTreeCloner).GetMethod(
          "getOrCreateDeepPropertyBasedCloner",
          BindingFlags.NonPublic | BindingFlags.Static
        );
        MethodInfo getTypeMethodInfo = typeof(object).GetMethod("GetType");
        MethodInfo invokeMethodInfo = typeof(Func<object, object>).GetMethod("Invoke");

        // Generate expressions to do this:
        //   clone.SomeProperty = getOrCreateDeepPropertyBasedCloner(
        //     original.SomeProperty.GetType()
        //   ).Invoke(original.SomeProperty);
        propertyTransferExpressions.Add(
          Expression.Assign(
            Expression.Property(clone, propertyInfo),
            Expression.Convert(
              Expression.Call(
                Expression.Call(
                  getOrCreateClonerMethodInfo,
                  Expression.Call(originalProperty, getTypeMethodInfo)
                ),
                invokeMethodInfo,
                originalProperty
              ),
              propertyType
            )
          )
        );
      }

      // Wrap up the generated array or complex reference type transfer expressions
      // in a null check so the property is skipped if it is not holding an instance.
      transferExpressions.Add(
        Expression.IfThen(
          Expression.NotEqual(
            originalProperty, Expression.Constant(null)
          ),
          Expression.Block(propertyVariables, propertyTransferExpressions)
        )
      );
    }

  }

} // namespace Nuclex.Support.Cloning

#endif // !(XBOX360 || WINDOWS_PHONE)
