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
using System.IO;
using System.Text;

using Nuclex.Support.Collections;

namespace Nuclex.Support.Parsing {

  /// <summary>Parses and stores an application's command line parameters</summary>
  /// <remarks>
  ///   <para>
  ///     At the time of the creation of this component, there are already several command
  ///     line parsing libraries out there. Most of them, however, do way too much at once
  ///     or at the very least rely on huge, untested clutters of classes and methods to
  ///     arrive at their results.
  ///   </para>
  ///   <para>
  ///     This command line parser does nothing more than represent the command line to
  ///     the application through a convenient interface. It parses a command line and
  ///     extracts the arguments, but doesn't interpret them and or check them for validity.
  ///   </para>
  ///   <para>
  ///     This design promotes simplicity and makes is an ideal building block to create
  ///     actual command line interpreters that connect the parameters to program
  ///     instructions and or fill structures in code.
  ///   </para>
  ///   <para>
  ///     Terminology
  ///     <list type="table">
  ///       <item>
  ///         <term>Command line</term>
  ///         <description>
  ///           The entire command line either as a string or as
  ///           an already parsed data structure
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <term>Argument</term>
  ///         <description>
  ///           Either an option or a loose value (see below) being specified on
  ///           the command line
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <term>Option</term>
  ///         <description>
  ///           Can be specified on the command line and typically alters the behavior
  ///           of the application or changes a setting. For example, '--normalize' or
  ///           '/safemode'.
  ///         </description>
  ///       </item>
  ///       <item>
  ///         <term>Value</term>
  ///         <description>
  ///           Can either sit loosely in the command line (eg. 'update' or 'textfile.txt')
  ///           or as assignment to an option (eg. '--width=1280' or '/overwrite:always')
  ///         </description>
  ///       </item>
  ///     </list>
  ///   </para>
  /// </remarks>
  public partial class CommandLine {

    /// <summary>
    ///   Whether the command line should use Windows mode by default
    /// </summary>
    public static readonly bool WindowsModeDefault =
      (Path.DirectorySeparatorChar == '\\');

    /// <summary>Initializes a new command line</summary>
    public CommandLine() :
      this(new List<Argument>(), WindowsModeDefault) { }

    /// <summary>Initializes a new command line</summary>
    /// <param name="windowsMode">Whether the / character initiates an argument</param>
    public CommandLine(bool windowsMode) :
      this(new List<Argument>(), windowsMode) { }

    /// <summary>Initializes a new command line</summary>
    /// <param name="argumentList">List containing the parsed arguments</param>
    private CommandLine(IList<Argument> argumentList) :
      this(argumentList, WindowsModeDefault) { }

    /// <summary>Initializes a new command line</summary>
    /// <param name="argumentList">List containing the parsed arguments</param>
    /// <param name="windowsMode">Whether the / character initiates an argument</param>
    private CommandLine(IList<Argument> argumentList, bool windowsMode) {
      this.arguments = argumentList;
      this.windowsMode = windowsMode;
    }

    /// <summary>Parses the command line arguments from the provided string</summary>
    /// <param name="commandLineString">String containing the command line arguments</param>
    /// <returns>The parsed command line</returns>
    /// <remarks>
    ///   You should always pass Environment.CommandLine to this method to avoid
    ///   some problems with the built-in command line tokenizer in .NET
    ///   (which splits '--test"hello world"/v' into '--testhello world/v')
    /// </remarks>
    public static CommandLine Parse(string commandLineString) {
      bool windowsMode = (Path.DirectorySeparatorChar != '/');
      return Parse(commandLineString, windowsMode);
    }

    /// <summary>Parses the command line arguments from the provided string</summary>
    /// <param name="commandLineString">String containing the command line arguments</param>
    /// <param name="windowsMode">Whether the / character initiates an argument</param>
    /// <returns>The parsed command line</returns>
    /// <remarks>
    ///   You should always pass Environment.CommandLine to this methods to avoid
    ///   some problems with the built-in command line tokenizer in .NET
    ///   (which splits '--test"hello world"/v' into '--testhello world/v')
    /// </remarks>
    public static CommandLine Parse(string commandLineString, bool windowsMode) {
      return new CommandLine(
        Parser.Parse(commandLineString, windowsMode)
      );
    }

    /// <summary>Returns whether an argument with the specified name exists</summary>
    /// <param name="name">Name of the argument whose existence will be checked</param>
    /// <returns>True if an argument with the specified name exists</returns>
    public bool HasArgument(string name) {
      return (indexOfArgument(name) != -1);
    }

    /// <summary>Adds a value to the command line</summary>
    /// <param name="value">Value that will be added</param>
    public void AddValue(string value) {
      int valueLength = (value != null) ? value.Length : 0;

      if(requiresQuotes(value)) {
        StringBuilder builder = new StringBuilder(valueLength + 2);
        builder.Append('"');
        builder.Append(value);
        builder.Append('"');

        this.arguments.Add(
          Argument.ValueOnly(
            new StringSegment(builder.ToString(), 0, valueLength + 2),
            1,
            valueLength
          )
        );
      } else {
        this.arguments.Add(
          Argument.ValueOnly(new StringSegment(value), 0, valueLength)
        );
      }
    }

    /// <summary>Adds an option to the command line</summary>
    /// <param name="name">Name of the option that will be added</param>
    public void AddOption(string name) {
      AddOption("-", name);
    }

    /// <summary>Adds an option to the command line</summary>
    /// <param name="initiator">Initiator that will be used to start the option</param>
    /// <param name="name">Name of the option that will be added</param>
    public void AddOption(string initiator, string name) {
      StringBuilder builder = new StringBuilder(initiator.Length + name.Length);
      builder.Append(initiator);
      builder.Append(name);

      this.arguments.Add(
        Argument.OptionOnly(
          new StringSegment(builder.ToString()),
          initiator.Length,
          name.Length
        )
      );
    }

    /// <summary>Adds an option with an assignment to the command line</summary>
    /// <param name="name">Name of the option that will be added</param>
    /// <param name="value">Value that will be assigned to the option</param>
    public void AddAssignment(string name, string value) {
      AddAssignment("-", name, value);
    }

    /// <summary>Adds an option with an assignment to the command line</summary>
    /// <param name="initiator">Initiator that will be used to start the option</param>
    /// <param name="name">Name of the option that will be added</param>
    /// <param name="value">Value that will be assigned to the option</param>
    public void AddAssignment(string initiator, string name, string value) {
      bool valueContainsSpaces = containsWhitespace(value);
      StringBuilder builder = new StringBuilder(
        initiator.Length + name.Length + 1 + value.Length + (valueContainsSpaces ? 2 : 0)
      );
      builder.Append(initiator);
      builder.Append(name);
      builder.Append('=');
      if(valueContainsSpaces) {
        builder.Append('"');
        builder.Append(value);
        builder.Append('"');
      } else {
        builder.Append(value);
      }

      this.arguments.Add(
        new Argument(
          new StringSegment(builder.ToString()),
          initiator.Length,
          name.Length,
          initiator.Length + name.Length + 1 + (valueContainsSpaces ? 1 : 0),
          value.Length
        )
      );
    }

    /// <summary>Returns a string that contains the entire command line</summary>
    /// <returns>The entire command line as a single string</returns>
    public override string ToString() {
      return Formatter.FormatCommandLine(this);
    }

    /// <summary>Retrieves the index of the argument with the specified name</summary>
    /// <param name="name">Name of the argument whose index will be returned</param>
    /// <returns>
    ///   The index of the indicated argument of -1 if no argument with that name exists
    /// </returns>
    private int indexOfArgument(string name) {
      for(int index = 0; index < this.arguments.Count; ++index) {
        if(this.arguments[index].Name == name) {
          return index;
        }
      }

      return -1;
    }

    /// <summary>Options that were specified on the command line</summary>
    public IList<Argument> Arguments {
      get { return this.arguments; }
    }

    /// <summary>
    ///   Determines whether the string requires quotes to survive the command line
    /// </summary>
    /// <param name="value">Value that will be checked for requiring quotes</param>
    /// <returns>True if the value requires quotes to survive the command line</returns>
    private bool requiresQuotes(string value) {

      // If the value is empty, it needs quotes to become visible as an argument
      // (versus being intepreted as spacing between other arguments)
      if(string.IsNullOrEmpty(value)) {
        return true;
      }

      // Any whitespace characters force us to use quotes, so does a minus sign
      // at the beginning of the value (otherwise, it would become an option argument)
      bool requiresQuotes =
        containsWhitespace(value) ||
        (value[0] == '-');

      // On windows, option arguments can also be starten with the forward slash
      // character, so we require quotes as well if the value starts with one
      if(this.windowsMode) {
        requiresQuotes |= (value[0] == '/');
      }

      return requiresQuotes;

    }

    /// <summary>
    ///   Determines whether the string contains any whitespace characters
    /// </summary>
    /// <param name="value">String that will be scanned for whitespace characters</param>
    /// <returns>True if the provided string contains whitespace characters</returns>
    private static bool containsWhitespace(string value) {
      return
        (value.IndexOf(' ') != -1) ||
        (value.IndexOf('\t') != -1);
    }

    /// <summary>Options that were specified on the command line</summary>
    private IList<Argument> arguments;
    /// <summary>Whether the / character initiates an argument</summary>
    private bool windowsMode;

  }

} // namespace Nuclex.Support.Parsing
