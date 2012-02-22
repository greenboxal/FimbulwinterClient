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

using Microsoft.Xna.Framework.Input;

using Nuclex.Input;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls {

  /// <summary>User interface element the user can push down</summary>
  public abstract class PressableControl : Control, IFocusable {

    /// <summary>Initializes a new command control</summary>
    public PressableControl() {
      this.Enabled = true;
    }

    /// <summary>Whether the mouse pointer is hovering over the control</summary>
    public bool MouseHovering {
      get { return this.mouseHovering; }
    }

    /// <summary>Whether the pressable control is in the depressed state</summary>
    public virtual bool Depressed {
      get {
        bool mousePressed = (this.mouseHovering && this.pressedDownByMouse);
        return
          mousePressed ||
          this.pressedDownByKeyboard ||
          this.pressedDownByKeyboardShortcut ||
          this.pressedDownByGamepadShortcut;
      }
    }

    /// <summary>Whether the control currently has the input focus</summary>
    public bool HasFocus {
      get {
        return
          (Screen != null) &&
          ReferenceEquals(Screen.FocusedControl, this);
      }
    }

    /// <summary>
    ///   Called when the mouse has entered the control and is now hovering over it
    /// </summary>
    protected override void OnMouseEntered() {
      this.mouseHovering = true;
    }

    /// <summary>
    ///   Called when the mouse has left the control and is no longer hovering over it
    /// </summary>
    protected override void OnMouseLeft() {

      // Intentionally not calling OnActivated() here because the user has moved
      // the mouse away from the command while holding the mouse button down -
      // a common trick under windows to last-second-abort the clicking of a button
      this.mouseHovering = false;

    }

    /// <summary>Called when a mouse button has been pressed down</summary>
    /// <param name="button">Index of the button that has been pressed</param>
    protected override void OnMousePressed(MouseButtons button)
    {
      if(this.Enabled) {
          if (button == MouseButtons.Left)
          {
          this.pressedDownByMouse = true;
        }
      }
    }

    /// <summary>Called when a mouse button has been released again</summary>
    /// <param name="button">Index of the button that has been released</param>
    protected override void OnMouseReleased(MouseButtons button)
    {
        if (button == MouseButtons.Left)
        {
        this.pressedDownByMouse = false;

        // Only trigger the pressed event if the mouse was released over the control.
        // The user can move the mouse cursor away from the control while still holding
        // the mouse button down to do the well-known last-second-abort.
        if(this.mouseHovering && this.Enabled) {

          // If this was the final input device holding down the control, meaning it's
          // not depressed any longer, this counts as a click and we trigger
          // the notification!
          if(!Depressed) {
            OnPressed();
          }

        }
      }
    }

    /// <summary>Called when a button on the gamepad has been pressed</summary>
    /// <param name="button">Button that has been pressed</param>
    /// <returns>
    ///   True if the button press was handled by the control, otherwise false.
    /// </returns>
    protected override bool OnButtonPressed(Buttons button) {
      if(this.ShortcutButton.HasValue) {
        if(button == this.ShortcutButton.Value) {
          this.pressedDownByGamepadShortcut = true;
          return true;
        }
      }

      return false;
    }

    /// <summary>Called when a button on the gamepad has been released</summary>
    /// <param name="button">Button that has been released</param>
    protected override void OnButtonReleased(Buttons button) {
      if(this.ShortcutButton.HasValue) {
        if(this.pressedDownByGamepadShortcut) {
          if(button == this.ShortcutButton.Value) {
            this.pressedDownByGamepadShortcut = false;
            if(!Depressed) {
              OnPressed();
            }
          }
        }
      }
    }

    /// <summary>Called when a key on the keyboard has been pressed down</summary>
    /// <param name="keyCode">Code of the key that was pressed</param>
    /// <returns>
    ///   True if the key press was handled by the control, otherwise false.
    /// </returns>
    protected override bool OnKeyPressed(Keys keyCode) {
      if(this.ShortcutButton.HasValue) {
        if(keyCode == keyFromButton(this.ShortcutButton.Value)) {
          this.pressedDownByKeyboardShortcut = true;
          return true;
        }
      }
      if(HasFocus) {
        if(keyCode == Keys.Space) {
          this.pressedDownByKeyboard = true;
          return true;
        }
      }

      return false;
    }

    /// <summary>Called when a key on the keyboard has been released again</summary>
    /// <param name="keyCode">Code of the key that was released</param>
    protected override void OnKeyReleased(Keys keyCode) {
      if(this.pressedDownByKeyboardShortcut) {
        if(this.ShortcutButton.HasValue) {
          if(keyCode == keyFromButton(this.ShortcutButton.Value)) {
            this.pressedDownByKeyboardShortcut = false;
            if(!Depressed) {
              OnPressed();
            }
          }
        }
      }
      if(this.pressedDownByKeyboard) {
        if(keyCode == Keys.Space) {
          this.pressedDownByKeyboard = false;
          if(!Depressed) {
            OnPressed();
          }
        }
      }
    }

    /// <summary>Called when the control is pressed</summary>
    /// <remarks>
    ///   If you were to implement a button, for example, you could trigger a 'Pressed'
    ///   event here are call a user-provided delegate, depending on your design.
    /// </remarks>
    protected virtual void OnPressed() { }

    /// <summary>Whether the control can currently obtain the input focus</summary>
    bool IFocusable.CanGetFocus {
      get { return this.Enabled; }
    }

    /// <summary>Looks up the equivalent key to the gamepad button</summary>
    /// <param name="button">
    ///   Gamepad button for which the equivalent key on the keyboard will be found
    /// </param>
    /// <returns>The key that is equivalent to the specified gamepad button</returns>
    private static Keys keyFromButton(Buttons button) {
      switch(button) {
        case Buttons.A: { return Keys.A; }
        case Buttons.B: { return Keys.B; }
        case Buttons.Back: { return Keys.Back; }
        case Buttons.LeftShoulder: { return Keys.L; }
        case Buttons.LeftStick: { return Keys.LeftControl; }
        case Buttons.RightShoulder: { return Keys.R; }
        case Buttons.RightStick: { return Keys.RightControl; }
        case Buttons.Start: { return Keys.Enter; }
        case Buttons.X: { return Keys.X; }
        case Buttons.Y: { return Keys.Y; }
        default: { return Keys.None; }
      }
    }

    /// <summary>Whether the user can interact with the choice</summary>
    public bool Enabled;

    /// <summary>Button that can be pressed to activate this command</summary>
    public Buttons? ShortcutButton;

    /// <summary>Whether the command is pressed down using the space key</summary>
    private bool pressedDownByKeyboard;
    /// <summary>Whether the command is pressed down using the keyboard shortcut</summary>
    private bool pressedDownByKeyboardShortcut;
    /// <summary>Whether the command is pressed down using the game pad shortcut</summary>
    private bool pressedDownByGamepadShortcut;
    /// <summary>Whether the command is pressed down using the mouse</summary>
    private bool pressedDownByMouse;
    /// <summary>Whether the mouse is hovering over the command</summary>
    private bool mouseHovering;


  }

} // namespace Nuclex.UserInterface.Controls
