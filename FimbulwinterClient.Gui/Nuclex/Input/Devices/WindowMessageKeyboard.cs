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
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework.Input;

#if WINDOWS

namespace Nuclex.Input.Devices {

  /// <summary>Interfaces with a PC keyboard via window messages</summary>
  internal class WindowMessageKeyboard : BufferedKeyboard, IDisposable {

    /// <summary>Initialize a new window message-based keyboard device</summary>
    /// <param name="messageSource">Source the window messages are obtained from</param>
    internal WindowMessageKeyboard(IKeyboardMessageSource messageSource) {
      this.bufferKeyPressDelegate = new KeyboardKeyEventDelegate(BufferKeyPress);
      this.bufferKeyReleaseDelegate = new KeyboardKeyEventDelegate(BufferKeyRelease);
      this.bufferCharacterEntryDelegate = new KeyboardCharacterEventDelegate(
        BufferCharacterEntry
      );

      this.messageSource = messageSource;
      this.messageSource.KeyPressed += this.bufferKeyPressDelegate;
      this.messageSource.KeyReleased += this.bufferKeyReleaseDelegate;
      this.messageSource.CharacterEntered += this.bufferCharacterEntryDelegate;
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if (this.messageSource != null) {
        this.messageSource.CharacterEntered -= this.bufferCharacterEntryDelegate;
        this.messageSource.KeyReleased -= this.bufferKeyReleaseDelegate;
        this.messageSource.KeyPressed -= this.bufferKeyPressDelegate;

        this.messageSource = null;
      }
    }

    /// <summary>Whether the input device is connected to the system</summary>
    public override bool IsAttached {
      get { return true; }
    }

    /// <summary>Human-readable name of the input device</summary>
    public override string Name {
      get { return "PC Keyboard"; }
    }

    /// <summary>Delegate for the keyPressed() method</summary>
    private KeyboardKeyEventDelegate bufferKeyPressDelegate;
    /// <summary>Delegate for the keyReleased() method</summary>
    private KeyboardKeyEventDelegate bufferKeyReleaseDelegate;
    /// <summary>Delegate for the characterEntered() method</summary>
    private KeyboardCharacterEventDelegate bufferCharacterEntryDelegate;

    /// <summary>Window message source the instance is currently subscribed to</summary>
    private IKeyboardMessageSource messageSource;

  }

} // namespace Nuclex.Input.Devices

#endif // WINDOWS
