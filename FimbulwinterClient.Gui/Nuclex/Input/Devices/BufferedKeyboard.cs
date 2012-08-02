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

  /// <summary>Keyboard that buffers key presses until Update() is called</summary>
  public abstract class BufferedKeyboard : IKeyboard {

    #region enum EventType

    /// <summary>Types of event the buffered keyboard can queue</summary>
    private enum EventType {
      /// <summary>A key has been pressed</summary>
      KeyPress,
      /// <summary>A key has been released</summary>
      KeyRelease,
      /// <summary>A character has been entered</summary>
      Character,
      /// <summary>A snapshot was taken at this point in time</summary>
      Snapshot
    }

    #endregion // enum EventType

    #region struct KeyboardEvent

    /// <summary>Stores the properties of a keyboard event</summary>
    [StructLayout(LayoutKind.Explicit)]
    private struct KeyboardEvent {

      /// <summary>Creates a new keyboard event for a key press</summary>
      /// <param name="key">Key that has been pressed</param>
      /// <returns>The new keyboard event</returns>
      public static KeyboardEvent KeyPress(Keys key) {
        var keyboardEvent = new KeyboardEvent();
        keyboardEvent.EventType = EventType.KeyPress;
        keyboardEvent.Key = key;
        return keyboardEvent;
      }

      /// <summary>Creates a new keyboard event for a key release</summary>
      /// <param name="key">Key that has been released</param>
      /// <returns>The new keyboard event</returns>
      public static KeyboardEvent KeyRelease(Keys key) {
        var keyboardEvent = new KeyboardEvent();
        keyboardEvent.EventType = EventType.KeyRelease;
        keyboardEvent.Key = key;
        return keyboardEvent;
      }

      /// <summary>Creates a new keyboard event for an entered character</summary>
      /// <param name="character">Character that has been entered</param>
      /// <returns>The new keyboard event</returns>
      public static KeyboardEvent CharacterEntry(char character) {
        var keyboardEvent = new KeyboardEvent();
        keyboardEvent.EventType = EventType.Character;
        keyboardEvent.Character = character;
        return keyboardEvent;
      }

      /// <summary>Creates a new keyboard event for a snapshot point</summary>
      /// <returns>The new keyboard event</returns>
      public static KeyboardEvent Snapshot() {
        var keyboardEvent = new KeyboardEvent();
        keyboardEvent.EventType = EventType.Snapshot;
        return keyboardEvent;
      }

      /// <summary>Type of the event</summary>
      [FieldOffset(0)]
      public EventType EventType;
      /// <summary>The key that was pressed or released</summary>
      [FieldOffset(4)]
      public Keys Key;
      /// <summary>The character that was entered</summary>
      [FieldOffset(4)]
      public char Character;

    }

    #endregion // struct KeyboardEvent

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

    /// <summary>Initialize a new buffered keyboard device</summary>
    internal BufferedKeyboard() {
      this.queuedEvents = new Queue<KeyboardEvent>();
      this.current = new KeyboardState();
    }

    /// <summary>Retrieves the current state of the keyboard</summary>
    /// <returns>The current state of the keyboard</returns>
    public KeyboardState GetState() {
      return this.current;
    }

    /// <summary>Whether the input device is connected to the system</summary>
    public abstract bool IsAttached { get; }

    /// <summary>Human-readable name of the input device</summary>
    public abstract string Name { get; }

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
      while (this.queuedEvents.Count > 0) {
        KeyboardEvent nextEvent = this.queuedEvents.Dequeue();
        switch (nextEvent.EventType) {
          case EventType.KeyRelease: {
            KeyboardStateHelper.RemovePressedKey(ref this.current, (int)nextEvent.Key);
            OnKeyReleased(nextEvent.Key);
            break;
          }
          case EventType.KeyPress: {
            KeyboardStateHelper.AddPressedKey(ref this.current, (int)nextEvent.Key);
            OnKeyPressed(nextEvent.Key);
            break;
          }
          case EventType.Character: {
            OnCharacterEntered(nextEvent.Character);
            break;
          }
          case EventType.Snapshot: {
            return;
          }
        }
      }
    }

    /// <summary>Takes a snapshot of the current state of the input device</summary>
    /// <remarks>
    ///   This snapshot will be queued until the user calls the Update() method,
    ///   where the next polled snapshot will be taken from the queue and provided
    ///   to the user.
    /// </remarks>
    public void TakeSnapshot() {
      this.queuedEvents.Enqueue(KeyboardEvent.Snapshot());
    }

    /// <summary>Records a key press in the event queue</summary>
    /// <param name="key">Key that has been pressed</param>
    protected void OnKeyPressed(Keys key) {
      if (KeyPressed != null) {
        KeyPressed(key);
      }
    }

    /// <summary>Records a key release in the event queue</summary>
    /// <param name="key">Key that has been released</param>
    protected void OnKeyReleased(Keys key) {
      if (KeyReleased != null) {
        KeyReleased(key);
      }
    }

    /// <summary>Records a character in the event queue</summary>
    /// <param name="character">Character that has been entered</param>
    protected void OnCharacterEntered(char character) {
      if (CharacterEntered != null) {
        CharacterEntered(character);
      }
    }

    /// <summary>Records a key press in the event queue</summary>
    /// <param name="key">Key that has been pressed</param>
    protected void BufferKeyPress(Keys key) {
      this.queuedEvents.Enqueue(KeyboardEvent.KeyPress(key));
    }

    /// <summary>Records a key release in the event queue</summary>
    /// <param name="key">Key that has been released</param>
    protected void BufferKeyRelease(Keys key) {
      this.queuedEvents.Enqueue(KeyboardEvent.KeyRelease(key));
    }

    /// <summary>Records a character in the event queue</summary>
    /// <param name="character">Character that has been entered</param>
    protected void BufferCharacterEntry(char character) {
      this.queuedEvents.Enqueue(KeyboardEvent.CharacterEntry(character));
    }

    /// <summary>Keyboard events waiting to be processed</summary>
    private Queue<KeyboardEvent> queuedEvents;

    /// <summary>Current state of the keyboard</summary>
    private KeyboardState current;

  }

} // namespace Nuclex.Input.Devices

#endif // WINDOWS
