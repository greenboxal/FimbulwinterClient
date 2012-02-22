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

#if WINDOWS

namespace Nuclex.Input.Devices {

  /// <summary>Code-controllable keyboard for unit testing</summary>
  public partial class MockedKeyboard : BufferedKeyboard {

    /// <summary>Initializes a new mocked keyboard</summary>
    public MockedKeyboard() { }

    /// <summary>Whether the input device is connected to the system</summary>
    public override bool IsAttached {
      get { return this.isAttached; }
    }

    /// <summary>Human-readable name of the input device</summary>
    public override string Name {
      get { return "Mocked keyboard"; }
    }

    /// <summary>Types the specified character on the keyboard</summary>
    /// <param name="character">Character that will be typed</param>
    public void Enter(char character) {
      BufferCharacterEntry(character);
    }

    /// <summary>Types the specified text on the keyboard</summary>
    /// <param name="text">Text that will be typed</param>
    public void Type(string text) {
      for (int index = 0; index < text.Length; ++index) {
        char character = text[index];
        bool isUpper = char.IsUpper(character);

        if (isUpper) {
          BufferKeyPress(Keys.LeftShift);
        }

        Keys key;
        if (((int)character > 0) && ((int)character < 256)) {
          key = keyMap[(int)character];
        } else {
          key = Keys.None;
        }

        BufferKeyPress(key);
        BufferCharacterEntry(character);
        BufferKeyRelease(key);

        if (isUpper) {
          BufferKeyRelease(Keys.LeftShift);
        }
      }
    }

    /// <summary>Presses the specified key on the keyboard</summary>
    /// <param name="key">Key that will be pressed</param>
    public void Press(Keys key) {
      BufferKeyPress(key);
    }

    /// <summary>Releases the specified key on the keyboard</summary>
    /// <param name="key">Key that will be released</param>
    public void Release(Keys key) {
      BufferKeyRelease(key);
    }

    /// <summary>Attaches (connects) the game pad</summary>
    public void Attach() {
      this.isAttached = true;
    }

    /// <summary>Detaches (disconnects) the game pad</summary>
    public void Detach() {
      this.isAttached = false;
    }

    /// <summary>Whether the game pad is attached</summary>
    private bool isAttached;

  }

} // namespace Nuclex.Input.Devices

#endif // WINDOWS
