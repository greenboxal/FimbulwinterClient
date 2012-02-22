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

  /// <summary>Helper routines for working with strings</summary>
  public static class StringHelper {

    /// <summary>
    ///   Searches for the first occurence of a character other than the characters
    ///   listed in the <paramref name="anyNotOf" /> parameter
    /// </summary>
    /// <param name="haystack">String that will be scanned in</param>
    /// <param name="anyNotOf">Characters to not look for in the scanned string</param>
    /// <returns>
    ///   The index of the first occurence of a character not in the
    ///   <paramref name="anyNotOf" /> array or -1 if all characters in the string were
    ///   present in the <paramref name="anyNotOf" /> array.
    /// </returns>
    public static int IndexNotOfAny(this string haystack, char[] anyNotOf) {
      return IndexNotOfAny(haystack, anyNotOf, 0, haystack.Length);
    }

    /// <summary>
    ///   Searches for the first occurence of a character other than the characters
    ///   listed in the <paramref name="anyNotOf" /> parameter
    /// </summary>
    /// <param name="haystack">String that will be scanned in</param>
    /// <param name="anyNotOf">Characters to not look for in the scanned string</param>
    /// <param name="startIndex">
    ///   Index of the character in the haystack at which to start scanning
    /// </param>
    /// <returns>
    ///   The index of the first occurence of a character not in the
    ///   <paramref name="anyNotOf" /> array or -1 if all characters in the string were
    ///   present in the <paramref name="anyNotOf" /> array.
    /// </returns>
    public static int IndexNotOfAny(this string haystack, char[] anyNotOf, int startIndex) {
      return IndexNotOfAny(haystack, anyNotOf, startIndex, haystack.Length - startIndex);
    }

    /// <summary>
    ///   Searches for the first occurence of a character other than the characters
    ///   listed in the <paramref name="anyNotOf" /> parameter
    /// </summary>
    /// <param name="haystack">String that will be scanned in</param>
    /// <param name="anyNotOf">Characters to not look for in the scanned string</param>
    /// <param name="startIndex">
    ///   Index of the character in the haystack at which to start scanning
    /// </param>
    /// <param name="count">Number of characters in the haystack to scan</param>
    /// <returns>
    ///   The index of the first occurence of a character not in the
    ///   <paramref name="anyNotOf" /> array or -1 if all characters in the string were
    ///   present in the <paramref name="anyNotOf" /> array.
    /// </returns>
    public static int IndexNotOfAny(
      this string haystack, char[] anyNotOf, int startIndex, int count
    ) {
      int anyLength = anyNotOf.Length;

      count += startIndex;
      while(startIndex < count) {
        char character = haystack[startIndex];

        int index = Array.IndexOf<char>(anyNotOf, character, 0, anyLength);
        if(index == -1) {
          return startIndex;
        }

        ++startIndex;
      }

      return -1;
    }

    /// <summary>
    ///   Searches backwards for the first occurence of a character other than the
    ///   characters listed in the <paramref name="anyNotOf" /> parameter
    /// </summary>
    /// <param name="haystack">String that will be scanned in</param>
    /// <param name="anyNotOf">Characters to not look for in the scanned string</param>
    /// <returns>
    ///   The index of the first occurence of a character not in the
    ///   <paramref name="anyNotOf" /> array or -1 if all characters in the string were
    ///   present in the <paramref name="anyNotOf" /> array.
    /// </returns>
    public static int LastIndexNotOfAny(this string haystack, char[] anyNotOf) {
      return LastIndexNotOfAny(haystack, anyNotOf, haystack.Length - 1, haystack.Length);
    }

    /// <summary>
    ///   Searches backwards for the first occurence of a character other than the
    ///   characters listed in the <paramref name="anyNotOf" /> parameter
    /// </summary>
    /// <param name="haystack">String that will be scanned in</param>
    /// <param name="anyNotOf">Characters to not look for in the scanned string</param>
    /// <param name="startIndex">
    ///   Index of the character in the haystack at which to start scanning
    /// </param>
    /// <returns>
    ///   The index of the first occurence of a character not in the
    ///   <paramref name="anyNotOf" /> array or -1 if all characters in the string were
    ///   present in the <paramref name="anyNotOf" /> array.
    /// </returns>
    public static int LastIndexNotOfAny(this string haystack, char[] anyNotOf, int startIndex) {
      return LastIndexNotOfAny(haystack, anyNotOf, startIndex, startIndex + 1);
    }

    /// <summary>
    ///   Searches backwards for the first occurence of a character other than the
    ///   characters listed in the <paramref name="anyNotOf" /> parameter
    /// </summary>
    /// <param name="haystack">String that will be scanned in</param>
    /// <param name="anyNotOf">Characters to not look for in the scanned string</param>
    /// <param name="startIndex">
    ///   Index of the character in the haystack at which to start scanning
    /// </param>
    /// <param name="count">Number of characters in the haystack to scan</param>
    /// <returns>
    ///   The index of the first occurence of a character not in the
    ///   <paramref name="anyNotOf" /> array or -1 if all characters in the string were
    ///   present in the <paramref name="anyNotOf" /> array.
    /// </returns>
    public static int LastIndexNotOfAny(
      this string haystack, char[] anyNotOf, int startIndex, int count
    ) {
      int anyLength = anyNotOf.Length;

      count = startIndex - count;
      while(startIndex > count) {
        char character = haystack[startIndex];

        int index = Array.IndexOf<char>(anyNotOf, character, 0, anyLength);
        if(index == -1) {
          return startIndex;
        }

        --startIndex;
      }

      return -1;
    }

  }

} // namespace Nuclex.Support

