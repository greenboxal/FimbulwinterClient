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

  /// <summary>Interfaces with an XBox 360 chat pad via XNA (XINPUT)</summary>
  internal partial class XnaKeyboard : IKeyboard {

    /// <summary>Fired when a key has been pressed</summary>
    public event KeyDelegate KeyPressed;

    /// <summary>Fired when a key has been released</summary>
    public event KeyDelegate KeyReleased;

    /// <summary>Fired when the user has entered a character</summary>
    /// <remarks>
    ///   This provides the complete, translated character the user has entered.
    ///   Handling of international keyboard layouts, shift key, accents and
    ///   other special cases is done by Windows according to the current users'
    ///   country and selected keyboard layout.
    /// </remarks>
    public event CharacterDelegate CharacterEntered;

    /// <summary>Initializes a new XNA-based keyboard device</summary>
    /// <param name="playerIndex">Index of the player whose chat pad will be queried</param>
    /// <param name="gamePad">Game pad the chat pad is attached to</param>
    public XnaKeyboard(PlayerIndex playerIndex, IGamePad gamePad) {
      this.playerIndex = playerIndex;
      this.gamePad = gamePad;
      this.states = new Queue<KeyboardState>();
      this.current = new KeyboardState();
    }

    /// <summary>Retrieves the current state of the keyboard</summary>
    /// <returns>The current state of the keyboard</returns>
    public KeyboardState GetState() {
      return this.current;
    }

    /// <summary>Whether the input device is connected to the system</summary>
    public bool IsAttached {
      get { return this.gamePad.IsAttached; }
    }

    /// <summary>Human-readable name of the input device</summary>
    public string Name {
      get { return "XBox 360 chat pad"; }
    }

    /// <summary>Updates the state of the input device</summary>
    /// <remarks>
    ///   <para>
    ///     If this method is called with no snapshots in the queue, it will take
    ///     an immediate snapshot and make it the current state. This way, you
    ///     can use the input devices without caring for the snapshot system if
    ///     you wish.
    ///   </para>
    ///   <para>
    ///     If this method is called while one or more snapshots are waiting in
    ///     the queue, this method takes the next snapshot from the queue and makes
    ///     it the current state.
    ///   </para>
    /// </remarks>
    public void Update() {
      // According to http://msdn.microsoft.com/en-us/library/bb975640.aspx chat pads
      // are only supported on the Xbox 360. Contrary to the documentation, I have
      // observed the GetState(PlayerIndex) method returning the state of my
      // real keyboard instead. This caused my GUI to register entered characters
      // twice (once from WM_CHAR, once from the supposed chat pad which wasn't)!
#if XBOX360
      KeyboardState previous = this.current;

      if (this.states.Count == 0) {
        this.current = queryKeyboardState();
      } else {
        this.current = this.states.Dequeue();
      }

      generateEvents(ref previous, ref this.current);
#endif
    }

    /// <summary>Takes a snapshot of the current state of the input device</summary>
    /// <remarks>
    ///   This snapshot will be queued until the user calls the Update() method,
    ///   where the next polled snapshot will be taken from the queue and provided
    ///   to the user.
    /// </remarks>
    public void TakeSnapshot() {
      this.states.Enqueue(queryKeyboardState());
    }

    /// <summary>Fires the KeyPressed event</summary>
    /// <param name="key">Key to report as having been pressed</param>
    protected void OnKeyPressed(Keys key) {
      if (KeyPressed != null) {
        KeyPressed(key);
      }
    }

    /// <summary>Fires the KeyReleased event</summary>
    /// <param name="key">Key to report as having been releaed</param>
    protected void OnKeyReleased(Keys key) {
      if (KeyReleased != null) {
        KeyReleased(key);
      }
    }

    /// <summary>Fires the CharacterEntered event</summary>
    /// <param name="character">Character to report as having been entered</param>
    protected void OnCharacterEntered(char character) {
      if (CharacterEntered != null) {
        CharacterEntered(character);
      }
    }

    /// <summary>Returns all entries in the XNA Keys enumeration</summary>
    /// <returns>All entries in the keys enumeration</returns>
    private static Keys[] getAllValidKeys() {
#if XBOX360 || WINDOWS_PHONE
      FieldInfo[] fieldInfos = typeof(Keys).GetFields(
        BindingFlags.Public | BindingFlags.Static
      );

      // Create an array to hold the enumeration values and copy them over from
      // the fields we just retrieved
      var values = new Keys[fieldInfos.Length];
      for (int index = 0; index < fieldInfos.Length; ++index) {
        values[index] = (Keys)fieldInfos[index].GetValue(null);
      }
      
      return values;
#else
      return (Keys[])Enum.GetValues(typeof(Keys));
#endif
    }

    /// <summary>Updates the immediate (non-buffered) state of the keyboard</summary>
    /// <remarks>
    ///   Only called when the game is not using the Update() and TakeSnapshot() methods
    ///   to buffer input.
    /// </remarks>
    private KeyboardState queryKeyboardState() {
      if(this.gamePad.IsAttached) {
        return Keyboard.GetState(this.playerIndex);
      } else {
        return new KeyboardState();
      }
    }

#if false
    /// <summary>Updates the immediate (non-buffered) state of the keyboard</summary>
    /// <remarks>
    ///   Only called when the game is not using the Update() and TakeSnapshot() methods
    ///   to buffer input.
    /// </remarks>
    private void updateImmediateState() {
      KeyboardState previous = this.current;
      this.current = Keyboard.GetState(this.playerIndex);
      generateEvents(ref previous, ref this.current);
    }
#endif

    /// <summary>Generates events for the differences between two states</summary>
    /// <param name="previous">Previous state the keyboard reported</param>
    /// <param name="current">Current state reported by the keyboard</param>
    private void generateEvents(ref KeyboardState previous, ref KeyboardState current) {

      // No subscribers? Don't waste time!
      if ((KeyPressed == null) && (KeyReleased == null) && (CharacterEntered == null)) {
        return;
      }

      // Check all keys for changes between the two provided states
      for (int keyIndex = 0; keyIndex < validKeys.Length; ++keyIndex) {
        Keys key = validKeys[keyIndex];

        KeyState previousState = previous[key];
        KeyState currentState = current[key];

        // If this key changed state, report it
        if (previousState != currentState) {
          if (currentState == KeyState.Down) {
            OnKeyPressed(key);
            generateCharacterEvent(key);
          } else {
            OnKeyReleased(key);
          }
        }
      }

    }

    /// <summary>Generates the character entered event for the chat pad</summary>
    /// <param name="key">Key that has been pressed on the chat pad</param>
    private void generateCharacterEvent(Keys key) {
      char character = characterMap[(int)key];
      if (character == '\0') {
        return;
      }

      bool isShiftPressed =
        this.current.IsKeyDown(Keys.LeftShift) ||
        this.current.IsKeyDown(Keys.RightShift);

      if (isShiftPressed) {
        OnCharacterEntered(char.ToUpper(character));
      } else {
        OnCharacterEntered(character);
      }
    }

    /// <summary>Contains all keys listed in the Keys enumeration</summary>
    private static readonly Keys[] validKeys = getAllValidKeys();

    /// <summary>Index of the player this device represents</summary>
    private PlayerIndex playerIndex;

    /// <summary>Game pad the chat pad is attached to</summary>
    private IGamePad gamePad;

    /// <summary>Snapshots of the keyboard state waiting to be processed</summary>
    private Queue<KeyboardState> states;

    /// <summary>Currently published keyboard state</summary>
    private KeyboardState current;

  }

} // namespace Nuclex.Input.Devices
