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
using System.Text;

namespace Nuclex.Support {

  /*
    public enum Garbage {
      Avoid,
      Accept
    }
  */
  /// <summary>Contains helper methods for the string builder class</summary>
  public static class StringBuilderHelper {

    /// <summary>Predefined unicode characters for the numbers 0 to 9</summary>
    private static readonly char[] numbers = new char[] {
      '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    /// <summary>Clears the contents of a string builder</summary>
    /// <param name="builder">String builder that will be cleared</param>
    public static void Clear(this StringBuilder builder) {
      builder.Remove(0, builder.Length);
    }

    /// <summary>
    ///   Appends an integer to a string builder without generating garbage
    /// </summary>
    /// <param name="builder">String builder to which an integer will be appended</param>
    /// <param name="value">Byte that will be appended to the string builder</param>
    /// <remarks>
    ///   The normal StringBuilder.Append() method generates garbage when converting
    ///   integer arguments whereas this method will avoid any garbage, albeit probably
    ///   with a small performance impact compared to the built-in method.
    /// </remarks>
    public static void Append(StringBuilder builder, byte value) {
      recursiveAppend(builder, value);
    }

    /// <summary>
    ///   Appends an integer to a string builder without generating garbage
    /// </summary>
    /// <param name="builder">String builder to which an integer will be appended</param>
    /// <param name="value">Integer that will be appended to the string builder</param>
    /// <remarks>
    ///   The normal StringBuilder.Append() method generates garbage when converting
    ///   integer arguments whereas this method will avoid any garbage, albeit probably
    ///   with a small performance impact compared to the built-in method.
    /// </remarks>
    public static void Append(StringBuilder builder, int value) {
      if (value < 0) {
        builder.Append('-');
        recursiveAppend(builder, -value);
      } else {
        recursiveAppend(builder, value);
      }
    }

    /// <summary>
    ///   Appends an long integer to a string builder without generating garbage
    /// </summary>
    /// <param name="builder">String builder to which an integer will be appended</param>
    /// <param name="value">Long integer that will be appended to the string builder</param>
    /// <remarks>
    ///   The normal StringBuilder.Append() method generates garbage when converting
    ///   integer arguments whereas this method will avoid any garbage, albeit probably
    ///   with a small performance impact compared to the built-in method.
    /// </remarks>
    public static void Append(StringBuilder builder, long value) {
      if (value < 0) {
        builder.Append('-');
        recursiveAppend(builder, -value);
      } else {
        recursiveAppend(builder, value);
      }
    }

    /// <summary>
    ///   Appends a floating point value to a string builder without generating garbage
    /// </summary>
    /// <param name="builder">String builder the value will be appended to</param>
    /// <param name="value">Value that will be appended to the string builder</param>
    /// <returns>Whether the value was inside the algorithm's supported range</returns>
    /// <remarks>
    ///   Uses an algorithm that covers the sane range of possible values but will
    ///   fail to render extreme values, NaNs and infinity. In these cases, false
    ///   is returned and the traditional double.ToString() method can be used.
    /// </remarks>
    public static bool Append(StringBuilder builder, float value) {
      return Append(builder, value, int.MaxValue);
    }

    /// <summary>
    ///   Appends a floating point value to a string builder without generating garbage
    /// </summary>
    /// <param name="builder">String builder the value will be appended to</param>
    /// <param name="value">Value that will be appended to the string builder</param>
    /// <param name="decimalPlaces">Maximum number of decimal places to display</param>
    /// <returns>Whether the value was inside the algorithm's supported range</returns>
    /// <remarks>
    ///   Uses an algorithm that covers the sane range of possible values but will
    ///   fail to render extreme values, NaNs and infinity. In these cases, false
    ///   is returned and the traditional double.ToString() method can be used.
    /// </remarks>
    public static bool Append(StringBuilder builder, float value, int decimalPlaces) {
      const int ExponentBits = 0xFF; // Bit mask for the exponent bits
      const int FractionalBitCount = 23; // Number of bits for fractional part
      const int ExponentBias = 127; // Bias subtraced from exponent
      const int NumericBitCount = 31; // Bits without sign

      // You don't need modify these as they're calculated based on
      // the constants assigned above.
      const int FractionalBits = (2 << FractionalBitCount) - 1;
      const int HighestFractionalBit = (1 << FractionalBitCount);
      const int FractionalBitCountPlusOne = FractionalBitCount + 1;

      int intValue = FloatHelper.ReinterpretAsInt(value);
      int exponent = ((intValue >> FractionalBitCount) & ExponentBits) - ExponentBias;
      int mantissa = (intValue & FractionalBits) | HighestFractionalBit;

      int integral;
      int fractional;
      if (exponent >= 0) {
        if (exponent >= FractionalBitCount) {
          if (exponent >= NumericBitCount) {
            return false;
          }
          integral = mantissa << (exponent - FractionalBitCount);
          fractional = 0;
        } else {
          integral = mantissa >> (FractionalBitCount - exponent);
          fractional = (mantissa << (exponent + 1)) & FractionalBits;
        }
      } else {
        if (exponent < -FractionalBitCount) {
          return false;
        }
        integral = 0;
        fractional = (mantissa & FractionalBits) >> -(exponent + 1);
      }

      // Build the integral part      
      if (intValue < 0) {
        builder.Append('-');
      }
      if (integral == 0) {
        builder.Append('0');
      } else {
        recursiveAppend(builder, integral);
      }

      if (decimalPlaces > 0) {
        builder.Append('.');

        // Build the fractional part
        if (fractional == 0) {
          builder.Append('0');
        } else {
          while (fractional != 0) {
            fractional *= 10;
            int digit = (fractional >> FractionalBitCountPlusOne);
            builder.Append(numbers[digit]);
            fractional &= FractionalBits;

            --decimalPlaces;
            if (decimalPlaces == 0) {
              break;
            }
          }
        }
      }

      return true;
    }

    /// <summary>
    ///   Appends a double precision floating point value to a string builder
    ///   without generating garbage
    /// </summary>
    /// <param name="builder">String builder the value will be appended to</param>
    /// <param name="value">Value that will be appended to the string builder</param>
    /// <returns>Whether the value was inside the algorithm's supported range</returns>
    /// <remarks>
    ///   Uses an algorithm that covers the sane range of possible values but will
    ///   fail to render extreme values, NaNs and infinity. In these cases, false
    ///   is returned and the traditional double.ToString() method can be used.
    /// </remarks>
    public static bool Append(StringBuilder builder, double value) {
      return Append(builder, value, int.MaxValue);
    }

    /// <summary>
    ///   Appends a double precision floating point value to a string builder
    ///   without generating garbage
    /// </summary>
    /// <param name="builder">String builder the value will be appended to</param>
    /// <param name="value">Value that will be appended to the string builder</param>
    /// <param name="decimalPlaces">Maximum number of decimal places to display</param>
    /// <returns>Whether the value was inside the algorithm's supported range</returns>
    /// <remarks>
    ///   Uses an algorithm that covers the sane range of possible values but will
    ///   fail to render extreme values, NaNs and infinity. In these cases, false
    ///   is returned and the traditional double.ToString() method can be used.
    /// </remarks>
    public static bool Append(StringBuilder builder, double value, int decimalPlaces) {
      const long ExponentBits = 0x7FF; // Bit mask for the exponent bits
      const int FractionalBitCount = 52; // Number of bits for fractional part
      const int ExponentBias = 1023; // Bias subtraced from exponent
      const int NumericBitCount = 63; // Bits without sign

      // You don't need modify these as they're calculated based on
      // the constants assigned above.
      const long FractionalBits = (2L << FractionalBitCount) - 1;
      const long HighestFractionalBit = (1L << FractionalBitCount);
      const int FractionalBitCountPlusOne = FractionalBitCount + 1;

      long longValue = FloatHelper.ReinterpretAsLong(value);
      long exponent = ((longValue >> FractionalBitCount) & ExponentBits) - ExponentBias;
      long mantissa = (longValue & FractionalBits) | HighestFractionalBit;

      long integral;
      long fractional;
      if (exponent >= 0) {
        if (exponent >= FractionalBitCount) {
          if (exponent >= NumericBitCount) {
            return false;
          }
          integral = mantissa << (int)(exponent - FractionalBitCount);
          fractional = 0;
        } else {
          integral = mantissa >> (int)(FractionalBitCount - exponent);
          fractional = (mantissa << (int)(exponent + 1)) & FractionalBits;
        }
      } else {
        if (exponent < -FractionalBitCount) {
          return false;
        }
        integral = 0;
        fractional = (mantissa & FractionalBits) >> -(int)(exponent + 1);
      }

      // Build the integral part      
      if (longValue < 0) {
        builder.Append('-');
      }
      if (integral == 0) {
        builder.Append('0');
      } else {
        recursiveAppend(builder, integral);
      }

      if (decimalPlaces > 0) {
        builder.Append('.');

        // Build the fractional part
        if (fractional == 0) {
          builder.Append('0');
        } else {
          while (fractional != 0) {
            fractional *= 10;
            long digit = (fractional >> FractionalBitCountPlusOne);
            builder.Append(numbers[digit]);
            fractional &= FractionalBits;

            --decimalPlaces;
            if (decimalPlaces == 0) {
              break;
            }
          }
        }
      }

      return true;
    }

    /// <summary>Recursively appends a number's characters to a string builder</summary>
    /// <param name="builder">String builder the number will be appended to</param>
    /// <param name="remaining">Remaining digits that will be recursively processed</param>
    private static void recursiveAppend(StringBuilder builder, int remaining) {
#if WINDOWS
      int digit;
      int tenth = Math.DivRem(remaining, 10, out digit);
#else
      int digit = remaining % 10;
      int tenth = remaining / 10;
#endif

      if (tenth > 0) {
        recursiveAppend(builder, tenth);
      }

      builder.Append(numbers[digit]);
    }

    /// <summary>Recursively appends a number's characters to a string builder</summary>
    /// <param name="builder">String builder the number will be appended to</param>
    /// <param name="remaining">Remaining digits that will be recursively processed</param>
    private static void recursiveAppend(StringBuilder builder, long remaining) {
#if WINDOWS
      long digit;
      long tenth = Math.DivRem(remaining, 10, out digit);
#else
      long digit = remaining % 10;
      long tenth = remaining / 10;
#endif

      if (tenth > 0) {
        recursiveAppend(builder, tenth);
      }

      builder.Append(numbers[digit]);
    }

  }

} // namespace Nuclex.Support
