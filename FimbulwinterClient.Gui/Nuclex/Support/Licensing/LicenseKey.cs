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
using System.Collections;
using System.IO;
using System.Text;

namespace Nuclex.Support.Licensing {

  /// <summary>Typical license key with 5x5 alphanumerical characters</summary>
  /// <remarks>
  ///   <para>
  ///     This class manages a license key like it is used in Microsoft products.
  ///     Althought it is probably not the exact same technique used by Microsoft,
  ///     the textual representation of the license keys looks identical,
  ///     eg. <code>O809J-RN5TD-IM3CU-4IG1O-O90X9</code>.
  ///   </para>
  ///   <para>
  ///     Available storage space is used efficiently and allows for up to four
  ///     32 bit integers to be stored within the key, that's enough for a full GUID.
  ///     The four integers can be modified directly, for example to store feature
  ///     lists, checksums or other data within the key.
  ///   </para>
  /// </remarks>
  public class LicenseKey {

    /// <summary>Parses the license key contained in a string</summary>
    /// <param name="key">String containing a license key that is to be parsed</param>
    /// <returns>The license key parsed from provided string</returns>
    /// <exception cref="ArgumentException">
    ///   When the provided string is not a license key
    /// </exception>
    public static LicenseKey Parse(string key) {
      key = key.Replace(" ", string.Empty).Replace("-", string.Empty).ToUpper();
      if(key.Length != 25)
        throw new ArgumentException("This is not a license key");

      BitArray bits = new BitArray(128);
      uint sequence;

      // Convert the first 4 sequences of 6 chars into 124 bits
      for(int j = 0; j < 4; j++) {

        sequence =
          (uint)codeTable.IndexOf(key[j * 6 + 5]) * 60466176 +
          (uint)codeTable.IndexOf(key[j * 6 + 4]) * 1679616 +
          (uint)codeTable.IndexOf(key[j * 6 + 3]) * 46656 +
          (uint)codeTable.IndexOf(key[j * 6 + 2]) * 1296 +
          (uint)codeTable.IndexOf(key[j * 6 + 1]) * 36 +
          (uint)codeTable.IndexOf(key[j * 6 + 0]);

        for(int i = 0; i < 31; i++)
          bits[j * 31 + i] = (sequence & powersOfTwo[i, 1]) != 0;

      }

      // Append the remaining character's 4 bits
      sequence = (uint)codeTable.IndexOf(key[24]);
      bits[124] = (sequence & powersOfTwo[4, 1]) != 0;
      bits[125] = (sequence & powersOfTwo[3, 1]) != 0;
      bits[126] = (sequence & powersOfTwo[2, 1]) != 0;
      bits[127] = (sequence & powersOfTwo[1, 1]) != 0;

      // Revert the mangling that was applied to the key when encoding...
      unmangle(bits);

      // ...and we've got our GUID back!
      byte[] guidBytes = new byte[16];
      bits.CopyTo(guidBytes, 0);

      return new LicenseKey(new Guid(guidBytes));
    }

    /// <summary>Initializes a new, empty license key</summary>
    public LicenseKey() : this(Guid.Empty) { }

    /// <summary>Initializes the license key from a GUID</summary>
    /// <param name="source">GUID that is used to create the license key</param>
    public LicenseKey(Guid source) {
      this.guid = source;
    }

    /// <summary>Accesses the four integer values within a license key</summary>
    /// <exception cref="IndexOutOfRangeException">
    ///   When the index lies outside of the key's fields
    /// </exception>
    public int this[int index] {
      get {
        if((index < 0) || (index > 3))
          throw new IndexOutOfRangeException("Index out of range");

        return BitConverter.ToInt32(this.guid.ToByteArray(), index * 4);
      }
      set {
        if((index < 0) || (index > 3))
          throw new IndexOutOfRangeException("Index out of range");

        // Convert the GUID into binary data so we can replace one of its values
        byte[] guidBytes = this.guid.ToByteArray();

        // Overwrite the section at the index specified by the user with the new value
        Array.Copy(
          BitConverter.GetBytes(value), 0, // source and start index
          guidBytes, index * 4, // destination and start index
          4 // length
        );

        // Replacement finished, now we can reconstruct our guid
        this.guid = new Guid(guidBytes);
      }
    }

    /// <summary>Converts the license key into a GUID</summary>
    /// <returns>The GUID created from the license key</returns>
    public Guid ToGuid() {
      return this.guid;
    }

    /// <summary>Converts the license key into a byte array</summary>
    /// <returns>A byte array containing the converted license key</returns>
    public byte[] ToByteArray() {
      return this.guid.ToByteArray();
    }

    /// <summary>Converts the license key to a string</summary>
    /// <returns>A string containing the converted license key</returns>
    public override string ToString() {
      StringBuilder resultBuilder = new StringBuilder();

      // Build a bit array from the input data
      BitArray bits = new BitArray(this.guid.ToByteArray());
      mangle(bits);

      int sequence = 0;

      // Build 4 sequences of 6 characters from the first 124 bits
      for(int i = 0; i < 4; ++i) {

        // We take the next 31 bits from the buffer
        for(int j = 0; j < 31; ++j)
          sequence |= (int)powersOfTwo[j, bits[i * 31 + j] ? 1 : 0];

        // Using 31 bits, a number up to 2.147.483.648 can be represented,
        // while 6 alpha-numerical characters allow for 2.176.782.336 possible values,
        // which means we can fit 31 bits into every 6 alpha-numerical characters.
        for(int j = 0; j < 6; ++j) {
          resultBuilder.Append(codeTable[sequence % 36]);
          sequence /= 36;
        }

      }

      // Use the remaining 4 bits to build the final character
      resultBuilder.Append(
        codeTable[
          (int)(
            powersOfTwo[4, bits[124] ? 1 : 0] |
            powersOfTwo[3, bits[125] ? 1 : 0] |
            powersOfTwo[2, bits[126] ? 1 : 0] |
            powersOfTwo[1, bits[127] ? 1 : 0] |
            powersOfTwo[0, 1] // One bit remains unused :)
          )
        ]
      );

      // Now build a nice, readable string from the decoded characters
      resultBuilder.Insert(5, keyDelimiter, 0, 1);
      resultBuilder.Insert(11, keyDelimiter, 0, 1);
      resultBuilder.Insert(17, keyDelimiter, 0, 1);
      resultBuilder.Insert(23, keyDelimiter, 0, 1);
      return resultBuilder.ToString();
    }

    /// <summary>Mangles a bit array</summary>
    /// <param name="bits">Bit array that will be mangled</param>
    private static void mangle(BitArray bits) {
      BitArray temp = new BitArray(bits);

      for(int i = 0; i < temp.Length; ++i) {
        bits[i] = temp[shuffle[i]];

        if((i & 1) != 0)
          bits[i] = !bits[i];
      }
    }

    /// <summary>Unmangles a bit array</summary>
    /// <param name="bits">Bit array that will be unmangled</param>
    private static void unmangle(BitArray bits) {
      BitArray temp = new BitArray(bits);

      for(int i = 0; i < temp.Length; ++i) {
        if((i & 1) != 0)
          temp[i] = !temp[i];

        bits[shuffle[i]] = temp[i];
      }
    }

    /// <summary>Character used to delimit each 5 digit group in a license key</summary>
    /// <remarks>
    ///   Required to be a char array because the .NET Compact Framework only provides
    ///   an overload for char[] in the StringBuilder.Insert() method.
    /// </remarks>
    private static char[] keyDelimiter = new char[] { '-' };

    /// <summary>Table with the individual characters in a key</summary>
    private static readonly string codeTable =
      "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>Helper array containing the precalculated powers of two</summary>
    private static readonly uint[,] powersOfTwo = new uint[32, 2] {
      { 0,         1 }, { 0,         2 }, { 0,          4 }, { 0,          8 },
      { 0,        16 }, { 0,        32 }, { 0,         64 }, { 0,        128 },
      { 0,       256 }, { 0,       512 }, { 0,       1024 }, { 0,       2048 },
      { 0,      4096 }, { 0,      8192 }, { 0,      16384 }, { 0,      32768 },
      { 0,     65536 }, { 0,    131072 }, { 0,     262144 }, { 0,     524288 },
      { 0,   1048576 }, { 0,   2097152 }, { 0,    4194304 }, { 0,    8388608 },
      { 0,  16777216 }, { 0,  33554432 }, { 0,   67108864 }, { 0,  134217728 },
      { 0, 268435456 }, { 0, 536870912 }, { 0, 1073741824 }, { 0, 2147483648 }
    };

    /// <summary>Index list for rotating the bit arrays</summary>
    private static readonly byte[] shuffle = new byte[128] {
       99,  47,  19, 104,  40,  71,  35,  82,  88,   2, 117, 118, 105,  42,  84,  48,
       33,  54,  43,  27,  78,  53,  61,  50, 109,  87,  69,  66,  25,  76,  45,  14,
       92,  16, 123,  98,  95,  37,  34,   8,   1,  49,  20,  90,  15,  97,  22, 108,
        5,  32, 120, 106, 122,  70,  67,  55,  46,  89, 100,   0,  26,  94, 121,   7,
       56,  59, 103,  79, 107,  36, 125, 119, 126,  44,  18,  93,  75, 116,  31,   9,
       73, 113,   3,  41, 124,  60,  77,  91,  28, 114,  65,  12,  39, 127,  72,  17,
      112,  21,  96, 111,  83, 101,  85,  80,  23,  68,  57,  13,   4,  10,  51,  63,
       11,  30, 115, 102,  86,  81,  74, 110,  62,  38,  29,  64,  52,   6,  24,  58
    };

    /// <summary>GUID in which the key is stored</summary>
    private Guid guid;

  }

} // namespace Nuclex.Support.Licensing
