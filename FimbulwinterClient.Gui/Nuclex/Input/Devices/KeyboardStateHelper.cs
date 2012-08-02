#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2011 Nuclex Development Labs

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

#if WINDOWS

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.Xna.Framework.Input;

namespace Nuclex.Input.Devices {

  /// <summary>Contains helper methods to modify an XNA KeyboardState</summary>
  internal static class KeyboardStateHelper {

    /// <summary>Delegate for the KeyboardState.AddPressedKey() method</summary>
    /// <param name="keyboardState">KeyboardState that will be modified</param>
    /// <param name="key">Key that will be added to the pressed keys</param>
    public delegate void AddPressedKeyDelegate(
      ref KeyboardState keyboardState, int key
    );

    /// <summary>Delegate for the KeyboardState.RemovePressedKey() method</summary>
    /// <param name="keyboardState">KeyboardState that will be modified</param>
    /// <param name="key">Key that will be removed from the pressed keys</param>
    public delegate void RemovePressedKeyDelegate(
      ref KeyboardState keyboardState, int key
    );

    /// <summary>Initializes the static fields of the class</summary>
    static KeyboardStateHelper() {
      AddPressedKey = createAddPressedKeyDelegate();
      RemovePressedKey = createRemovePressedKeyDelegate();
    }

    /// <summary>
    ///   Creates a delegate for adding a pressed key to a keyboard state
    /// </summary>
    /// <returns>A delegate that can be used to add a pressed key</returns>
    private static AddPressedKeyDelegate createAddPressedKeyDelegate() {
      MethodInfo addPressedKeyMethod = typeof(KeyboardState).GetMethod(
        "AddPressedKey", BindingFlags.Instance | BindingFlags.NonPublic
      );
      Type byrefKeyboardState = typeof(KeyboardState).MakeByRefType();

      ParameterExpression instance = Expression.Parameter(byrefKeyboardState, "instance");
      ParameterExpression keyValue = Expression.Parameter(typeof(int), "key");

      Expression<AddPressedKeyDelegate> expression =
        Expression.Lambda<AddPressedKeyDelegate>(
          Expression.Call(instance, addPressedKeyMethod, keyValue),
          instance, keyValue
        );

      return expression.Compile();
    }

    /// <summary>
    ///   Creates a delegate for removing a pressed key from a keyboard state
    /// </summary>
    /// <returns>A delegate that can be used to remove a pressed key</returns>
    private static RemovePressedKeyDelegate createRemovePressedKeyDelegate() {
      MethodInfo addPressedKeyMethod = typeof(KeyboardState).GetMethod(
        "RemovePressedKey", BindingFlags.Instance | BindingFlags.NonPublic
      );
      Type byrefKeyboardState = typeof(KeyboardState).MakeByRefType();

      ParameterExpression instance = Expression.Parameter(byrefKeyboardState, "instance");
      ParameterExpression keyValue = Expression.Parameter(typeof(int), "key");

      Expression<RemovePressedKeyDelegate> expression =
        Expression.Lambda<RemovePressedKeyDelegate>(
          Expression.Call(instance, addPressedKeyMethod, keyValue),
          instance, keyValue
        );

      return expression.Compile();
    }

    /// <summary>Adds a pressed key to a KeyboardState</summary>
    public static readonly AddPressedKeyDelegate AddPressedKey;

    /// <summary>Removes a pressed key from a KeyboardState</summary>
    public static readonly RemovePressedKeyDelegate RemovePressedKey;

  }

} // namespace Nuclex.Input.Devices

#endif // WINDOWS
