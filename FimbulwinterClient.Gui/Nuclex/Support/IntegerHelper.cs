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

namespace Nuclex.Support {

  /// <summary>Helper methods for working with integer types</summary>
  public static class IntegerHelper {

    /// <summary>Returns the next highest power of 2 from the specified value</summary>
    /// <param name="value">Value of which to return the next highest power of 2</param>
    /// <returns>The next highest power of 2 to the value</returns>
    public static long NextPowerOf2(this long value) {
      return (long)NextPowerOf2((ulong)value);
    }

    /// <summary>Returns the next highest power of 2 from the specified value</summary>
    /// <param name="value">Value of which to return the next highest power of 2</param>
    /// <returns>The next highest power of 2 to the value</returns>
    public static ulong NextPowerOf2(this ulong value) {
      if (value == 0)
        return 1;

      --value;
      value |= value >> 1;
      value |= value >> 2;
      value |= value >> 4;
      value |= value >> 8;
      value |= value >> 16;
      value |= value >> 32;
      ++value;

      return value;
    }

    /// <summary>Returns the next highest power of 2 from the specified value</summary>
    /// <param name="value">Value of which to return the next highest power of 2</param>
    /// <returns>The next highest power of 2 to the value</returns>
    public static int NextPowerOf2(this int value) {
      return (int)NextPowerOf2((uint)value);
    }

    /// <summary>Returns the next highest power of 2 from the specified value</summary>
    /// <param name="value">Value of which to return the next highest power of 2</param>
    /// <returns>The next highest power of 2 to the value</returns>
    public static uint NextPowerOf2(this uint value) {
      if (value == 0)
        return 1;

      --value;
      value |= value >> 1;
      value |= value >> 2;
      value |= value >> 4;
      value |= value >> 8;
      value |= value >> 16;
      ++value;

      return value;
    }

    /// <summary>Returns the number of bits set in an </summary>
    /// <param name="value">Value whose bits will be counted</param>
    /// <returns>The number of bits set in the integer</returns>
    public static int CountBits(this int value) {
      return CountBits((uint)value);
    }

    /// <summary>Returns the number of bits set in an unsigned integer</summary>
    /// <param name="value">Value whose bits will be counted</param>
    /// <returns>The number of bits set in the unsigned integer</returns>
    /// <remarks>
    ///   Based on a trick revealed here:
    ///   http://stackoverflow.com/questions/109023
    /// </remarks>
    public static int CountBits(this uint value) {
      value = value - ((value >> 1) & 0x55555555);
      value = (value & 0x33333333) + ((value >> 2) & 0x33333333);

      return (int)unchecked(
        ((value + (value >> 4) & 0xF0F0F0F) * 0x1010101) >> 24
      );
    }

    /// <summary>Returns the number of bits set in a long integer</summary>
    /// <param name="value">Value whose bits will be counted</param>
    /// <returns>The number of bits set in the long integer</returns>
    public static int CountBits(this long value) {
      return CountBits((ulong)value);
    }

    /// <summary>Returns the number of bits set in an unsigned long integer</summary>
    /// <param name="value">Value whose bits will be counted</param>
    /// <returns>The number of bits set in the unsigned long integer</returns>
    /// <remarks>
    ///   Based on a trick revealed here:
    ///   http://stackoverflow.com/questions/2709430
    /// </remarks>
    public static int CountBits(this ulong value) {
      value = value - ((value >> 1) & 0x5555555555555555UL);
      value = (value & 0x3333333333333333UL) + ((value >> 2) & 0x3333333333333333UL);

      return (int)unchecked(
        (((value + (value >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56
      );
    }

  }

} // namespace Nuclex.Support
