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
using System.Text;

namespace Nuclex.Support.Parsing {

  partial class CommandLine {

    /// <summary>Formats a command line instance into a string</summary>
    internal static class Formatter {

      /// <summary>
      ///   Formats all arguments in the provided command line instance into a string
      /// </summary>
      /// <param name="commandLine">Command line instance that will be formatted</param>
      /// <returns>All arguments in the command line instance as a string</returns>
      public static string FormatCommandLine(CommandLine commandLine) {
        int totalLength = 0;
        for(int index = 0; index < commandLine.arguments.Count; ++index) {
          if(index != 0) {
            ++totalLength; // For spacing between arguments
          }

          totalLength += commandLine.arguments[index].RawLength;
        }

        StringBuilder builder = new StringBuilder(totalLength);
        for(int index = 0; index < commandLine.arguments.Count; ++index) {
          if(index != 0) {
            builder.Append(' ');
          }

          builder.Append(commandLine.arguments[index].Raw);
        }

        return builder.ToString();
      }

    }

  }

} // namespace Nuclex.Support.Parsing
