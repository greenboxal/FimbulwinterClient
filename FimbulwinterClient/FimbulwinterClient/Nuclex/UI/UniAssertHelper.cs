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

#if UNITTEST

using System;

using NUnit.Framework;

using Nuclex.Support;

namespace Nuclex.UserInterface {

  /// <summary>Contains special assertions for floating numbers</summary>
  public static class UniAssertHelper {

    /// <summary>Ensures that two unified scalars are nearly equal to each other</summary>
    /// <param name="expected">Expected unified scalar value</param>
    /// <param name="actual">Actual unified scalar value</param>
    /// <param name="deltaUlps">
    ///   Allowed deviation in representable floating point values for each of the
    ///   unified scalar's fields
    /// </param>
    public static void AreAlmostEqual(UniScalar expected, UniScalar actual, int deltaUlps) {
      if(!areAlmostEqual(expected, actual, deltaUlps)) {
        Assert.AreEqual(expected, actual);
      }
    }

    /// <summary>Ensures that two unified vectors are nearly equal to each other</summary>
    /// <param name="expected">Expected unified vector</param>
    /// <param name="actual">Actual unified vector</param>
    /// <param name="deltaUlps">
    ///   Allowed deviation in representable floating point values for each of the
    ///   unified vector's fields
    /// </param>
    public static void AreAlmostEqual(UniVector expected, UniVector actual, int deltaUlps) {
      if(!areAlmostEqual(expected, actual, deltaUlps)) {
        Assert.AreEqual(expected, actual);
      }
    }

    /// <summary>Ensures that two unified vectors are nearly equal to each other</summary>
    /// <param name="expected">Expected unified vector</param>
    /// <param name="actual">Actual unified vector</param>
    /// <param name="deltaUlps">
    ///   Allowed deviation in representable floating point values for each of the
    ///   unified vector's fields
    /// </param>
    public static void AreAlmostEqual(
      UniRectangle expected, UniRectangle actual, int deltaUlps
    ) {
      if(!areAlmostEqual(expected, actual, deltaUlps)) {
        Assert.AreEqual(expected, actual);
      }
    }

    /// <summary>Determines whether two unified scalar values are nearly equal</summary>
    /// <param name="left">Unified scalar value to compare on the left side</param>
    /// <param name="right">Unified scalar value to compare on the right side</param>
    /// <param name="deltaUlps">
    ///   Allowed deviation in representable floating point values for each of the
    ///   unified vector's fields
    /// </param>
    /// <returns>True if the provided unified scalar values are nearly equal</returns>
    private static bool areAlmostEqual(UniScalar left, UniScalar right, int deltaUlps) {
      return
        FloatHelper.AreAlmostEqual(left.Fraction, right.Fraction, deltaUlps) &&
        FloatHelper.AreAlmostEqual(left.Offset, right.Offset, deltaUlps);
    }

    /// <summary>Determines whether two unified vectors are nearly equal</summary>
    /// <param name="left">Unified vector to compare on the left side</param>
    /// <param name="right">Unified vector to compare on the right side</param>
    /// <param name="deltaUlps">
    ///   Allowed deviation in representable floating point values for each of the
    ///   unified vector's fields
    /// </param>
    /// <returns>True if the provided unified vectors are nearly equal</returns>
    private static bool areAlmostEqual(UniVector left, UniVector right, int deltaUlps) {
      return
        areAlmostEqual(left.X, right.X, deltaUlps) &&
        areAlmostEqual(left.Y, right.Y, deltaUlps);
    }

    /// <summary>Determines whether two unified rectangles are nearly equal</summary>
    /// <param name="left">Unified rectangle to compare on the left side</param>
    /// <param name="right">Unified rectangle to compare on the right side</param>
    /// <param name="deltaUlps">
    ///   Allowed deviation in representable floating point values for each of the
    ///   unified vector's fields
    /// </param>
    /// <returns>True if the provided unified rectangles are nearly equal</returns>
    private static bool areAlmostEqual(UniRectangle left, UniRectangle right, int deltaUlps) {
      return
        areAlmostEqual(left.Location, right.Location, deltaUlps) &&
        areAlmostEqual(left.Size, right.Size, deltaUlps);
    }

  }

} // namespace Nuclex.UserInterface

#endif // UNITTEST
