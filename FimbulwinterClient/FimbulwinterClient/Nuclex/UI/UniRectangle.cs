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

using Microsoft.Xna.Framework;

namespace Nuclex.UserInterface {

  /// <summary>
  ///   Two-dimensional rectangle of combined fraction and offset coordinates
  /// </summary>
  public struct UniRectangle {

    /// <summary>An empty unified rectangle</summary>
    public static readonly UniRectangle Empty = new UniRectangle();

    /// <summary>Initializes a new rectangle from a location and a size</summary>
    /// <param name="location">Location of the rectangle's upper left corner</param>
    /// <param name="size">Size of the area covered by the rectangle</param>
    public UniRectangle(UniVector location, UniVector size) {
      this.Location = location;
      this.Size = size;
    }

    /// <summary>
    ///   Initializes a new rectangle from the provided individual coordinates
    /// </summary>
    /// <param name="x">X coordinate of the rectangle's left border</param>
    /// <param name="y">Y coordinate of the rectangle's upper border</param>
    /// <param name="width">Width of the area covered by the rectangle</param>
    /// <param name="height">Height of the area covered by the rectangle</param>
    public UniRectangle(UniScalar x, UniScalar y, UniScalar width, UniScalar height) {
      this.Location = new UniVector(x, y);
      this.Size = new UniVector(width, height);
    }

    /// <summary>Converts the rectangle into pure offset coordinates</summary>
    /// <param name="containerSize">
    ///   Dimensions of the container the fractional part of the rectangle count for
    /// </param>
    /// <returns>A rectangle with the pure offset coordinates of the rectangle</returns>
    public RectangleF ToOffset(Vector2 containerSize) {
      return ToOffset(containerSize.X, containerSize.Y);
    }

    /// <summary>Converts the rectangle into pure offset coordinates</summary>
    /// <param name="containerWidth">
    ///   Width of the container the fractional part of the rectangle counts for
    /// </param>
    /// <param name="containerHeight">
    ///   Height of the container the fractional part of the rectangle counts for
    /// </param>
    /// <returns>A rectangle with the pure offset coordinates of the rectangle</returns>
    public RectangleF ToOffset(float containerWidth, float containerHeight) {
      float leftOffset = this.Left.Fraction * containerWidth + this.Left.Offset;
      float topOffset = this.Top.Fraction * containerHeight + this.Top.Offset;

      return new RectangleF(
        leftOffset,
        topOffset,
        (this.Right.Fraction * containerWidth + this.Right.Offset) - leftOffset,
        (this.Bottom.Fraction * containerHeight + this.Bottom.Offset) - topOffset
      );
    }

    /// <summary>X coordinate of the rectangle's left border</summary>
    public UniScalar Left {
      get { return this.Location.X; }
      set { this.Location.X = value; }
    }

    /// <summary>Y coordinate of the rectangle's upper border</summary>
    public UniScalar Top {
      get { return this.Location.Y; }
      set { this.Location.Y = value; }
    }

    /// <summary>X coordinate of the rectangle's right border</summary>
    public UniScalar Right {
      get { return this.Location.X + this.Size.X; }
      set { this.Size.X = value - this.Location.X; }
    }

    /// <summary>Y coordinate of the rectangle's lower border</summary>
    public UniScalar Bottom {
      get { return this.Location.Y + this.Size.Y; }
      set { this.Size.Y = value - this.Location.Y; }
    }

    /// <summary>Point consisting of the lesser coordinates of the rectangle</summary>
    public UniVector Min {
      get { return this.Location; }
      set {
        // In short: this.Size += this.Location - value;
        // Done for performance reasons
        this.Size.X.Fraction += this.Location.X.Fraction - value.X.Fraction;
        this.Size.X.Offset += this.Location.X.Offset - value.X.Offset;
        this.Size.Y.Fraction += this.Location.Y.Fraction - value.Y.Fraction;
        this.Size.Y.Offset += this.Location.Y.Offset - value.Y.Offset;

        this.Location = value;
      }
    }

    /// <summary>Point consisting of the greater coordinates of the rectangle</summary>
    public UniVector Max {
      get { return this.Location + this.Size; }
      set {
        // In short: this.Size = value - this.Location;
        // Done for performance reasons
        this.Size.X.Fraction = value.X.Fraction - this.Location.X.Fraction;
        this.Size.X.Offset = value.X.Offset - this.Location.X.Offset;
        this.Size.Y.Fraction = value.Y.Fraction - this.Location.X.Fraction;
        this.Size.Y.Offset = value.Y.Offset - this.Location.Y.Offset;
      }
    }

    /// <summary>Checks two rectangles for inequality</summary>
    /// <param name="first">First rectangle to be compared</param>
    /// <param name="second">Second rectangle to be compared</param>
    /// <returns>True if the instances differ or exactly one reference is set to null</returns>
    public static bool operator !=(UniRectangle first, UniRectangle second) {
      return !(first == second);
    }

    /// <summary>Checks two rectangles for equality</summary>
    /// <param name="first">First rectangle to be compared</param>
    /// <param name="second">Second rectangle to be compared</param>
    /// <returns>True if both instances are equal or both references are null</returns>
    public static bool operator ==(UniRectangle first, UniRectangle second) {
      // For a struct, neither 'first' nor 'second' can be null
      return first.Equals(second);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public override bool Equals(object other) {
      if(!(other is UniRectangle))
        return false;

      return Equals((UniRectangle)other);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public bool Equals(UniRectangle other) {
      // For a struct, 'other' cannot be null
      return (this.Location == other.Location) && (this.Size == other.Size);
    }

    /// <summary>Obtains a hash code of this instance</summary>
    /// <returns>The hash code of the instance</returns>
    public override int GetHashCode() {
      return this.Location.GetHashCode() ^ this.Size.GetHashCode();
    }

    /// <summary>
    ///   Returns a human-readable string representation for the unified rectangle
    /// </summary>
    /// <returns>The human-readable string representation of the unified rectangle</returns>
    public override string ToString() {
      return string.Format(
        "{{Location:{0}, Size:{1}}}",
        this.Location.ToString(),
        this.Size.ToString()
      );
    }

    /// <summary>The location of the rectangle's upper left corner</summary>
    public UniVector Location;
    /// <summary>The size of the rectangle</summary>
    public UniVector Size;

  }

} // namespace Nuclex.UserInterface
