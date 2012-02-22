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

namespace Nuclex.Support.Parsing {

  partial class CommandLine {

    /// <summary>Argument being specified on an application's command line</summary>
    public class Argument {

      /// <summary>Initializes a new option with only a name</summary>
      /// <param name="raw">
      ///   String segment with the entire argument as it was given on the command line
      /// </param>
      /// <param name="nameStart">Absolute index the argument name starts at</param>
      /// <param name="nameLength">Number of characters in the option name</param>
      /// <returns>The newly created option</returns>
      internal static Argument OptionOnly(
        StringSegment raw,
        int nameStart, int nameLength
      ) {
        return new Argument(raw, nameStart, nameLength, -1, -1);
      }

      /// <summary>Initializes a new argument with only a value</summary>
      /// <param name="raw">
      ///   String segment with the entire argument as it was given on the command line
      /// </param>
      /// <param name="valueStart">Absolute index the value starts at</param>
      /// <param name="valueLength">Number of characters in the value</param>
      /// <returns>The newly created option</returns>
      internal static Argument ValueOnly(
        StringSegment raw,
        int valueStart, int valueLength
      ) {
        return new Argument(raw, -1, -1, valueStart, valueLength);
      }

      /// <summary>Creates a new option with a name and an assigned value</summary>
      /// <param name="raw">
      ///   String segment containing the entire option as it was given on the command line
      /// </param>
      /// <param name="nameStart">Absolute index the option name starts at</param>
      /// <param name="nameLength">Number of characters in the option name</param>
      /// <param name="valueStart">Absolute index the value starts at</param>
      /// <param name="valueLength">Number of characters in the value</param>
      /// <returns>The newly created option</returns>
      internal Argument(
        StringSegment raw,
        int nameStart, int nameLength,
        int valueStart, int valueLength
      ) {
        this.raw = raw;
        this.nameStart = nameStart;
        this.nameLength = nameLength;
        this.valueStart = valueStart;
        this.valueLength = valueLength;
      }

      /// <summary>Contains the raw string the command line argument was parsed from</summary>
      public string Raw {
        get { return this.raw.ToString(); }
      }

      /// <summary>Characters used to initiate this option</summary>
      public string Initiator {
        get {
          if(this.nameStart == -1) {
            return null;
          } else {
            return this.raw.Text.Substring(
              this.raw.Offset, this.nameStart - this.raw.Offset
            );
          }
        }
      }

      /// <summary>Name of the command line option</summary>
      public string Name {
        get {
          if(this.nameStart == -1) {
            return null;
          } else {
            return this.raw.Text.Substring(this.nameStart, this.nameLength);
          }
        }
      }

      /// <summary>Characters used to associate a value to this option</summary>
      public string Associator {
        get {
          if(this.nameStart == -1) {
            return null;
          } else {
            int associatorStart = this.nameStart + this.nameLength;

            if(this.valueStart == -1) {
              int characterCount = (this.raw.Offset + this.raw.Count) - associatorStart;
              if(characterCount == 0) {
                return null;
              }
            } else if(this.valueStart == associatorStart) {
              return null;
            }

            return this.raw.Text.Substring(associatorStart, 1);
          }
        }
      }

      /// <summary>Name of the command line option</summary>
      public string Value {
        get {
          if(this.valueStart == -1) {
            return null;
          } else {
            return this.raw.Text.Substring(this.valueStart, this.valueLength);
          }
        }
      }

      /// <summary>The raw length of the command line argument</summary>
      internal int RawLength {
        get { return this.raw.Count; }
      }

      /// <summary>
      ///   Contains the entire option as it was specified on the command line
      /// </summary>
      private StringSegment raw;

      /// <summary>Absolute index in the raw string the option name starts at</summary>
      private int nameStart;
      /// <summary>Number of characters in the option name</summary>
      private int nameLength;
      /// <summary>Absolute index in the raw string the value starts at</summary>
      private int valueStart;
      /// <summary>Number of characters in the value</summary>
      private int valueLength;

    }

  }

} // namespace Nuclex.Support.Parsing
