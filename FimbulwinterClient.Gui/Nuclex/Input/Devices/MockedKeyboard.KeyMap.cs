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

  partial class MockedKeyboard {

    /// <summary>Maps characters to the keys enumeration</summary>
    private static readonly Keys[] keyMap = new Keys[] {
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 0-3
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 4-7
      Keys.None,      Keys.Tab,      Keys.None,     Keys.None,    // 8-11
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 12-15
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 16-19
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 20-23
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 24-27
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 28-31
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 32-35
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 36-39
      Keys.None,      Keys.None,     Keys.Multiply, Keys.Add,     // 40-43
      Keys.Separator, Keys.Subtract, Keys.Decimal,  Keys.Divide,  // 44-47
      Keys.D0,        Keys.D1,       Keys.D2,       Keys.D3,      // 48-51
      Keys.D4,        Keys.D5,       Keys.D6,       Keys.D7,      // 52-55
      Keys.D8,        Keys.D9,       Keys.None,     Keys.None,    // 56-59
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 60-63
      Keys.None,      Keys.A,        Keys.B,        Keys.C,       // 64-67
      Keys.D,         Keys.E,        Keys.F,        Keys.G,       // 68-71
      Keys.H,         Keys.I,        Keys.J,        Keys.K,       // 72-75
      Keys.L,         Keys.M,        Keys.N,        Keys.O,       // 76-79
      Keys.P,         Keys.Q,        Keys.R,        Keys.S,       // 80-83
      Keys.T,         Keys.U,        Keys.V,        Keys.W,       // 84-87
      Keys.X,         Keys.Y,        Keys.Z,        Keys.None,    // 88-91
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 92-95
      Keys.None,      Keys.A,        Keys.B,        Keys.C,       // 96-99
      Keys.D,         Keys.E,        Keys.F,        Keys.G,       // 100-103
      Keys.H,         Keys.I,        Keys.J,        Keys.K,       // 104-107
      Keys.L,         Keys.M,        Keys.N,        Keys.O,       // 108-111
      Keys.P,         Keys.Q,        Keys.R,        Keys.S,       // 112-115
      Keys.T,         Keys.U,        Keys.V,        Keys.W,       // 115-119
      Keys.X,         Keys.Y,        Keys.Z,        Keys.None,    // 120-123
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 124-127
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 128-131
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 132-135
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 136-139
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 140-143
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 144-147
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 148-151
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 152-155
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 156-159
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 160-163
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 164-167
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 168-171
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 172-175
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 176-179
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 180-183
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 184-187
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 188-191
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 192-195
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 196-199
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 200-103
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 204-207
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 208-211
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 212-215
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 216-219
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 220-223
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 224-227
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 228-231
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 232-235
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 236-239
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 240-243
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 244-247
      Keys.None,      Keys.None,     Keys.None,     Keys.None,    // 248-251
      Keys.None,      Keys.None,     Keys.None,     Keys.None     // 252-255
    };

  }

} // namespace Nuclex.Input.Devices
