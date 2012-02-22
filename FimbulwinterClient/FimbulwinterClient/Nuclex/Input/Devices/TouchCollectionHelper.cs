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
using Microsoft.Xna.Framework.Input.Touch;

namespace Nuclex.Input.Devices {

  /// <summary>An XNA TouchCollection that can be modified</summary>
  internal static class TouchCollectionHelper {

    /// <summary>Delegate for setting the TouchCollection's location count</summary>
    /// <param name="touchCollection">Collection whose location count will be set</param>
    /// <param name="locationCount">New location count</param>
    public delegate void SetLocationCountDelegate(
      ref TouchCollection touchCollection, int locationCount
    );

    /// <summary>Delegate for calling the TouchCollection's AddTouchLocation() method</summary>
    /// <param name="touchCollection">Collection the location will be added to</param>
    /// <param name="id">ID of the touch location</param>
    /// <param name="state">What happened at the touch location</param>
    /// <param name="x">X coordinate of the touch location</param>
    /// <param name="y">Y coordinate of the touch location</param>
    /// <param name="prevState">What happened at the previous touch location</param>
    /// <param name="prevX">Previous X coordinate of the touch location</param>
    /// <param name="prevY">Previous Y coordinate of the touch location</param>
    public delegate void AddTouchLocationDelegate(
      ref TouchCollection touchCollection,
      int id,
      TouchLocationState state,
      float x,
      float y,
      TouchLocationState prevState,
      float prevX,
      float prevY
    );

    /// <summary>Initializes the static fields of the class</summary>
    static TouchCollectionHelper() {
      SetLocationCount = createLocationCountDelegate();
      AddTouchLocation = createAddTouchLocationDelegate();
    }

    /// <summary>Removes all touch locations from the collection</summary>
    /// <param name="touchCollection">Touch collection that will be cleared</param>
    public static void Clear(ref TouchCollection touchCollection) {
      SetLocationCount(ref touchCollection, 0);
    }

    /// <summary>
    ///   Creates a delegate for setting the location count in a TouchCollection
    /// </summary>
    /// <returns>A delegate that can be used to change the location count</returns>
    private static SetLocationCountDelegate createLocationCountDelegate() {
      FieldInfo locationCountField = typeof(TouchCollection).GetField(
        "locationCount", BindingFlags.Instance | BindingFlags.NonPublic
      );
      Type byrefTouchCollection = typeof(TouchCollection).MakeByRefType();

      ParameterExpression instance = Expression.Parameter(byrefTouchCollection, "instance");
      ParameterExpression value = Expression.Parameter(typeof(int), "value");

      Expression<SetLocationCountDelegate> expression =
        Expression.Lambda<SetLocationCountDelegate>(
          Expression.Assign(
            Expression.Field(instance, locationCountField),
            value
          ),
          instance,
          value
        );

      return expression.Compile();
    }

    /// <summary>
    ///   Creates a delegate for adding a touch location to a TouchCollection
    /// </summary>
    /// <returns>A delegate that can be used to add a touch location</returns>
    private static AddTouchLocationDelegate createAddTouchLocationDelegate() {
      MethodInfo addTouchLocationMethod = typeof(TouchCollection).GetMethod(
        "AddTouchLocation", BindingFlags.Instance | BindingFlags.NonPublic
      );
      Type byrefTouchCollection = typeof(TouchCollection).MakeByRefType();

      ParameterExpression instance = Expression.Parameter(byrefTouchCollection, "instance");
      ParameterExpression idValue = Expression.Parameter(typeof(int), "id");
      ParameterExpression stateValue = Expression.Parameter(
        typeof(TouchLocationState), "state"
      );
      ParameterExpression xValue = Expression.Parameter(typeof(float), "x");
      ParameterExpression yValue = Expression.Parameter(typeof(float), "y");
      ParameterExpression prevStateValue = Expression.Parameter(
        typeof(TouchLocationState), "prevState"
      );
      ParameterExpression prevXValue = Expression.Parameter(typeof(float), "prevX");
      ParameterExpression prevYValue = Expression.Parameter(typeof(float), "prevY");

      Expression<AddTouchLocationDelegate> expression =
        Expression.Lambda<AddTouchLocationDelegate>(
          Expression.Call(
            instance, addTouchLocationMethod,
            idValue, stateValue, xValue, yValue, prevStateValue, prevXValue, prevYValue
          ),
          instance,
          idValue, stateValue, xValue, yValue, prevStateValue, prevXValue, prevYValue
        );

      return expression.Compile();
    }

    /// <summary>Sets the location count of a TouchCollection</summary>
    public static readonly SetLocationCountDelegate SetLocationCount;

    /// <summary>Adds a touch location to a TouchCollection</summary>
    public static readonly AddTouchLocationDelegate AddTouchLocation;

  }

} // namespace Nuclex.Input.Devices

#endif // WINDOWS
