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

using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Nuclex.Input.Devices {

  partial class XnaKeyboard {

    /// <summary>Maps the keys enumeration to characters</summary>
    private static readonly char[] characterMap = new char[] {
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 0-7
      '\0', '\t', '\0', '\0', '\0', '\0', '\0', '\0', // 8-15
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 16-23
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 24-31
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 32-39
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 40-47
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 48-55
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 56-63
      '\0', 'a',  'b',  'c',  'd',  'e',  'f',  'g',  // 64-71
      'h',  'i',  'j',  'k',  'l',  'm',  'n',  'o',  // 72-79
      'p',  'q',  'r',  's',  't',  'u',  'v',  'w',  // 80-87
      'x',  'y',  'z',  '\0', '\0', '\0', '\0', '\0', // 88-95
      '0',  '1',  '2',  '3',  '4',  '5',  '6',  '7',  // 96-103
      '8',  '9',  '*',  '+',  ',',  '-',  '.',  '/',  // 104-111
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 112-119
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 120-127
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 128-135
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 136-143
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 144-151
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 152-159
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 160-167
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 168-175
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 176-183
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 184-191
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 192-199
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 200-207
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 208-215
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 216-223
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 224-231
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 232-239
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', // 240-247
      '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0'  // 248-255
    };

  }

} // namespace Nuclex.Input.Devices
