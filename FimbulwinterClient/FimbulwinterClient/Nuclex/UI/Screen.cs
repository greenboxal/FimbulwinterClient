#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nuclex.Input;
using Nuclex.Support;
using Nuclex.UserInterface.Input;
using Nuclex.UserInterface.Controls;

namespace Nuclex.UserInterface {

  /// <summary>Manages the controls and their state on a GUI screen</summary>
  /// <remarks>
  ///   This class manages the global state of a distinct user interface. Unlike your
  ///   typical GUI library, the Nuclex.UserInterface library can handle any number of
  ///   simultaneously active user interfaces at the same time, making the library
  ///   suitable for usage on virtual ingame computers and multi-client environments
  ///   such as split-screen games or switchable graphical terminals.
  /// </remarks>
  public class Screen : IInputReceiver {

    /// <summary>Triggered when the control in focus changes</summary>
    public event EventHandler<ControlEventArgs> FocusChanged;

    /// <summary>Initializes a new GUI</summary>
    public Screen() : this(0, 0) { }

    /// <summary>Initializes a new GUI</summary>
    /// <param name="width">Width of the area the GUI can occupy</param>
    /// <param name="height">Height of the area the GUI can occupy</param>
    /// <remarks>
    ///   Width and height should reflect the entire drawable area of your GUI. If you
    ///   want to limit the region which the GUI is allowed to use (eg. to only use the
    ///   safe area of a TV) please resize the desktop control accordingly!
    /// </remarks>
    public Screen(float width, float height) {
      this.Width = width;
      this.Height = height;

      this.heldKeys = new BitArray(maxKeyboardKey + 1);
      this.heldButtons = 0;

      // By default, the desktop control will cover the whole drawing area
      this.desktopControl = new DesktopControl();
      this.desktopControl.Bounds.Size.X.Fraction = 1.0f;
      this.desktopControl.Bounds.Size.Y.Fraction = 1.0f;

      this.desktopControl.SetScreen(this);

      this.focusedControl = new WeakReference<Control>(null);
    }

    /// <summary>Width of the screen in pixels</summary>
    public float Width {
      get { return this.size.X; }
      set { this.size.X = value; }
    }

    /// <summary>Height of the screen in pixels</summary>
    public float Height {
      get { return this.size.Y; }
      set { this.size.Y = value; }
    }

    /// <summary>Control responsible for hosting the GUI's top-level controls</summary>
    public Control Desktop {
      get { return this.desktopControl; }
    }

    /// <summary>Injects a command into the processor</summary>
    /// <param name="command">Input command that will be injected</param>
    public void InjectCommand(Command command) {
      switch(command) {

        // Accept or cancel the current control
        case Command.Accept:
        case Command.Cancel: {
          Control focusedControl = FocusedControl;
          if(focusedControl == null) {
            return; // Also catches when focusedControl is not part of the tree
          }

          // TODO: Should this be propagated down the control tree?
          focusedControl.ProcessCommand(command);

          break;
        }

        // Change focus to another control
        case Command.SelectPrevious:
        case Command.SelectNext: {
          // TODO: Implement focus switching

          break;
        }

        // Control specific. Changes focus if unhandled.
        case Command.Up:
        case Command.Down:
        case Command.Left:
        case Command.Right: {
          Control focusedControl = FocusedControl;
          if(focusedControl == null) {
            return; // Also catches when focusedControl is not part of the tree
          }

          // First send the command to the focused control. If the control handles
          // the command, there's nothing for us to do. Otherwise, use the directional
          // commands for focus switching.
          if(focusedControl.ProcessCommand(command)) {
            return;
          }

          // These will be determined in the following code block
          float nearestDistance = float.NaN;
          Control nearestControl = null;
          {
            // Determine the center of the focused control
            RectangleF parentBounds = focusedControl.Parent.GetAbsoluteBounds();
            RectangleF focusedBounds = focusedControl.Bounds.ToOffset(
              parentBounds.Width, parentBounds.Height
            );

            // Search all siblings of the focused control for the nearest control in the
            // direction the command asks to move into
            Collection<Control> siblings = focusedControl.Parent.Children;
            for(int index = 0; index < siblings.Count; ++index) {
              Control sibling = siblings[index];

              // Only consider this sibling if it's focusable
              if(!ReferenceEquals(sibling, focusedControl) && canControlGetFocus(sibling)) {
                RectangleF siblingBounds = sibling.Bounds.ToOffset(
                  parentBounds.Width, parentBounds.Height
                );

                // Calculate the distance the control has in the direction focus is being
                // changed to. If the control doesn't lie in that direction, NaN will
                // be returned
                float distance = getDirectionalDistance(
                  ref focusedBounds, ref siblingBounds, command
                );
                if(float.IsNaN(nearestDistance) || (distance < nearestDistance)) {
                  nearestControl = sibling;
                  nearestDistance = distance;
                }
              }
            }
          } // beauty scope

          // Search completed, if we found a candidate, change focus to it
          if(nearestDistance != float.NaN) {
            FocusedControl = nearestControl;
          }

          break;
        }

      }
    }

    /// <summary>Called when a key on the keyboard has been pressed down</summary>
    /// <param name="keyCode">Code of the key that was pressed</param>
    public void InjectKeyPress(Keys keyCode) {
      bool repetition = this.heldKeys.Get((int)keyCode);

      // If a control is activated, it will receive any input notifications
      if(this.activatedControl != null) {
        // The desktop control might still reject a key press if the activated
        // control was closed by the previous key press and thus, while the screen
        // has an activated control, the desktop control no longer has.
        if(this.activatedControl.ProcessKeyPress(keyCode, repetition)) {
          if(!repetition) {
            ++this.heldKeyCount;
            this.heldKeys.Set((int)keyCode, true);
          }
        }
        return;
      }

      // No control is activated, try the focused control before searching
      // the entire tree for a responder.
      Control focusedControl = this.focusedControl.Target;
      if(focusedControl != null) {
        if(focusedControl.ProcessKeyPress(keyCode, false)) {
          this.activatedControl = focusedControl;
          if(!repetition) {
            ++this.heldKeyCount;
            this.heldKeys.Set((int)keyCode, true);
          }
          return;
        }
      }

      // Focused control didn't process the notification, now let the desktop
      // control traverse the entire control tree is earch for a handler.
      if(this.desktopControl.ProcessKeyPress(keyCode, false)) {
        this.activatedControl = this.desktopControl;
        if(!repetition) {
          ++this.heldKeyCount;
          this.heldKeys.Set((int)keyCode, true);
        }
      } else {
        switch(keyCode) {
          case Keys.Up: { InjectCommand(Command.Up); break; }
          case Keys.Down: { InjectCommand(Command.Down); break; }
          case Keys.Left: { InjectCommand(Command.Left); break; }
          case Keys.Right: { InjectCommand(Command.Right); break; }
          case Keys.Enter: { InjectCommand(Command.Accept); break; }
          case Keys.Escape: { InjectCommand(Command.Cancel); break; }
        }
      }
    }

    /// <summary>Called when a key on the keyboard has been released again</summary>
    /// <param name="keyCode">Code of the key that was released</param>
    public void InjectKeyRelease(Keys keyCode) {
      if(!this.heldKeys.Get((int)keyCode)) {
        return;
      }
      --this.heldKeyCount;
      this.heldKeys.Set((int)keyCode, false);

      // If a control signed responsible for the earlier key press, it will now
      // receive the release notification.
      if(this.activatedControl != null) {
        this.activatedControl.ProcessKeyRelease(keyCode);
      }

      // Reset the activated control if the user has released all buttons on all
      // input devices.
      if(!anyKeysOrButtonsPressed) {
        this.activatedControl = null;
      }
    }

    /// <summary>Handle user text input by a physical or virtual keyboard</summary>
    /// <param name="character">Character that has been entered</param>
    public void InjectCharacter(char character) {

      // Send the text to the currently focused control in the GUI
      Control focusedControl = this.focusedControl.Target;
      IWritable writable = focusedControl as IWritable;
      if(writable != null) {
        writable.OnCharacterEntered(character);
      }

    }

    /// <summary>Called when a button on the gamepad has been pressed</summary>
    /// <param name="button">Button that has been pressed</param>
    public void InjectButtonPress(Buttons button) {
      Buttons newHeldButtons = this.heldButtons | button;
      if(newHeldButtons == this.heldButtons) {
        return;
      }
      this.heldButtons = newHeldButtons;

      // If a control is activated, it will receive any input notifications
      if(this.activatedControl != null) {
        this.activatedControl.ProcessButtonPress(button);
        return;
      }

      // No control is activated, try the focused control before searching
      // the entire tree for a responder.
      Control focusedControl = this.focusedControl.Target;
      if(focusedControl != null) {
        if(focusedControl.ProcessButtonPress(button)) {
          this.activatedControl = focusedControl;
          return;
        }
      }

      // Focused control didn't process the notification, now let the desktop
      // control traverse the entire control tree is earch for a handler.
      if(this.desktopControl.ProcessButtonPress(button)) {
        this.activatedControl = this.desktopControl;
      }
    }

    /// <summary>Called when a button on the gamepad has been released</summary>
    /// <param name="button">Button that has been released</param>
    public void InjectButtonRelease(Buttons button) {
      if((this.heldButtons & button) == 0) {
        return;
      }
      this.heldButtons &= ~button;

      // If a control signed responsible for the earlier button press, it will now
      // receive the release notification.
      if(this.activatedControl != null) {
        this.activatedControl.ProcessButtonRelease(button);
      }

      // Reset the activated control if the user has released all buttons on all
      // input devices.
      if(!anyKeysOrButtonsPressed) {
        this.activatedControl = null;
      }
    }

    /// <summary>Injects a mouse position update into the GUI</summary>
    /// <param name="x">X coordinate of the mouse cursor within the screen</param>
    /// <param name="y">Y coordinate of the mouse cursor within the screen</param>
    public void InjectMouseMove(float x, float y) {
      this.desktopControl.ProcessMouseMove(this.size.X, this.size.Y, x, y);
    }

    /// <summary>Called when a mouse button has been pressed down</summary>
    /// <param name="button">Index of the button that has been pressed</param>
    public void InjectMousePress(MouseButtons button)
    {
      this.heldMouseButtons |= button;

      // If a control is activated, it will receive any input notifications
      if(this.activatedControl != null) {
        this.activatedControl.ProcessMousePress(button);
        return;
      }

      // No control was activated, so the desktop control becomes activated and
      // is responsible for routing the input to the control under the mouse.
      if(this.desktopControl.ProcessMousePress(button)) {
        this.activatedControl = this.desktopControl;
      }
    }

    /// <summary>Called when a mouse button has been released again</summary>
    /// <param name="button">Index of the button that has been released</param>
    public void InjectMouseRelease(MouseButtons button)
    {
      this.heldMouseButtons &= ~button;

      // If a control signed responsible for the earlier mouse press, it will now
      // receive the release notification.
      if(this.activatedControl != null) {
        this.activatedControl.ProcessMouseRelease(button);
      }

      // Reset the activated control if the user has released all buttons on all
      // input devices.
      if(!anyKeysOrButtonsPressed) {
        this.activatedControl = null;
      }
    }

    /// <summary>Called when the mouse wheel has been rotated</summary>
    /// <param name="ticks">Number of ticks that the mouse wheel has been rotated</param>
    public void InjectMouseWheel(float ticks) {
      if(this.activatedControl != null) {
        this.activatedControl.ProcessMouseWheel(ticks);
      } else {
        this.desktopControl.ProcessMouseWheel(ticks);
      }
    }

    /// <summary>Triggers the FocusChanged event</summary>
    /// <param name="focusedControl">Control that has gotten the input focus</param>
    private void onFocusChanged(Control focusedControl) {
      if(FocusChanged != null) {
        FocusChanged(this, new ControlEventArgs(focusedControl));
      }
    }

    /// <summary>Whether the GUI has currently captured the input devices</summary>
    /// <remarks>
    ///   <para>
    ///     When you mix GUIs and gameplay (for example, in a strategy game where the GUI
    ///     manages the build menu and the remainder of the screen belongs to the game),
    ///     it is important to keep control of who currently owns the input devices.
    ///   </para>
    ///   <para>
    ///     Assume the player is drawing a selection rectangle around some units using
    ///     the mouse. He will press the mouse button outside any GUI elements, keep
    ///     holding it down and possibly drag over the GUI. Until the player lets go
    ///     of the mouse button, input exclusively belongs to the game. The same goes
    ///     vice versa, of course.
    ///   </para>
    ///   <para>
    ///     This property tells whether the GUI currently thinks that all input belongs
    ///     to it. If it is true, the game should not process any input. The GUI will
    ///     implement the input model as described here and respect the game's ownership
    ///     of the input devices if a mouse button is pressed outside of the GUI. To
    ///     correctly handle input device ownership, send all input to the GUI
    ///     regardless of this property's value, then check this property and if it
    ///     returns false let your game process the input.
    ///   </para>
    /// </remarks>
    public bool IsInputCaptured {
      get { return this.desktopControl.IsInputCaptured; }
    }

    /// <summary>True if the mouse is currently hovering over any GUI elements</summary>
    /// <remarks>
    ///   Useful if you mix gameplay with a GUI and use different mouse cursors
    ///   depending on the location of the mouse. As long as input is not captured
    ///   (see <see cref="IsInputCaptured" />) you can use this property to know
    ///   whether you should use the standard GUI mouse cursor or let your game
    ///   decide which cursor to use.
    /// </remarks>
    public bool IsMouseOverGui {
      get { return this.desktopControl.IsMouseOverGui; }
    }

    /// <summary>Child control that currently has the input focus</summary>
    public Control FocusedControl {
      get {
        Control current = this.focusedControl.Target;
        if((current != null) && ReferenceEquals(current.Screen, this)) {
          return current;
        } else {
          return null;
        }
      }
      set {
        Control current = this.focusedControl.Target;
        if(!ReferenceEquals(value, current)) {
          this.focusedControl.Target = value;
          onFocusChanged(value);
        }
      }
    }

    /// <summary>
    ///   Whether any keys, mouse buttons or game pad buttons are beind held pressed
    /// </summary>
    private bool anyKeysOrButtonsPressed {
      get {
        return
          (this.heldMouseButtons != 0) ||
          (this.heldKeyCount > 0) ||
          (this.heldButtons != 0);
      }
    }

    /// <summary>
    ///   Determines the distance of one rectangle to the other, also taking direction
    ///   into account
    /// </summary>
    /// <param name="ownBounds">Boundaries of the base rectangle</param>
    /// <param name="otherBounds">Boundaries of the other rectangle</param>
    /// <param name="direction">Direction into which distance will be determined</param>
    /// <returns>
    ///   The direction of the other rectangle of NaN if it didn't lie in that direction
    /// </returns>
    private static float getDirectionalDistance(
      ref RectangleF ownBounds, ref RectangleF otherBounds, Command direction
    ) {
      float closestPointX, closestPointY;
      float distance;

      bool isVertical =
        (direction == Command.Up) ||
        (direction == Command.Down);

      if(isVertical) {
        float ownCenterX = ownBounds.X + (ownBounds.Width / 2.0f);

        // Take an imaginary line through the other control's center, perpendicular
        // to the specified direction. Then locate the closest point on that line
        // to our own center.
        closestPointX = Math.Min(Math.Max(ownCenterX, otherBounds.Left), otherBounds.Right);
        closestPointY = otherBounds.Y + (otherBounds.Height / 2.0f);

        // Find out whether we need to check the diagonal quadrant boundary
        bool leavesLeft = (closestPointX < ownBounds.Left);
        bool leavesRight = (closestPointX > ownBounds.Right);

        // 
        float sideY;
        if(direction == Command.Up) {
          sideY = ownBounds.Top;
          if((closestPointY > sideY) && (leavesLeft || leavesRight)) {
            return float.NaN;
          }
          distance = sideY - closestPointY;
        } else {
          sideY = ownBounds.Bottom;
          if((closestPointY < sideY) && (leavesLeft || leavesRight)) {
            return float.NaN;
          }
          distance = closestPointY - sideY;
        }

        float distanceY = Math.Abs(sideY - closestPointY);
        if(leavesLeft) {
          float distanceX = Math.Abs(ownBounds.Left - closestPointX);
          if(distanceX > distanceY) {
            return float.NaN;
          }
        } else if(leavesRight) {
          float distanceX = Math.Abs(closestPointX - ownBounds.Right);
          if(distanceX > distanceY) {
            return float.NaN;
          }
        }
      } else {
        float ownCenterY = ownBounds.Y + (ownBounds.Height / 2.0f);

        // Take an imaginary line through the other control's center, perpendicular
        // to the specified direction. Then locate the closest point on that line
        // to our own center.
        closestPointX = otherBounds.X + (otherBounds.Width / 2.0f);
        closestPointY = Math.Min(Math.Max(ownCenterY, otherBounds.Top), otherBounds.Bottom);

        // Find out whether we need to check the diagonal quadrant boundary
        bool leavesTop = (closestPointY < ownBounds.Top);
        bool leavesBottom = (closestPointY > ownBounds.Bottom);

        float sideX;
        if(direction == Command.Left) {
          sideX = ownBounds.Left;
          if((closestPointX > sideX) && (leavesTop || leavesBottom)) {
            return float.NaN;
          }
          distance = sideX - closestPointX;
        } else {
          sideX = ownBounds.Right;
          if((closestPointX < sideX) && (leavesTop || leavesBottom)) {
            return float.NaN;
          }
          distance = closestPointX - sideX;
        }

        float distanceX = Math.Abs(sideX - closestPointX);
        if(leavesTop) {
          float distanceY = Math.Abs(ownBounds.Top - closestPointY);
          if(distanceY > distanceX) {
            return float.NaN;
          }
        } else if(leavesBottom) {
          float distanceY = Math.Abs(closestPointY - ownBounds.Bottom);
          if(distanceY > distanceX) {
            return float.NaN;
          }
        }
      }

      return (distance < 0.0f) ? float.NaN : distance;
    }

    /// <summary>Determines whether a control can obtain the input focus</summary>
    /// <param name="control">Control that will be checked for focusability</param>
    /// <returns>True if the specified control can obtain the input focus</returns>
    private static bool canControlGetFocus(Control control) {
      IFocusable focusableControl = control as IFocusable;
      if(focusableControl != null) {
        return focusableControl.CanGetFocus;
      } else {
        return false;
      }
    }

    /// <summary>Highest value in the Keys enumeration</summary>
    private static readonly int maxKeyboardKey =
      (int)EnumHelper.GetHighestValue<Keys>();

    /// <summary>Size of the GUI area in world units or pixels</summary>
    private Vector2 size;
    /// <summary>Control responsible for hosting the GUI's top-level controls</summary>
    private DesktopControl desktopControl;
    /// <summary>Child that currently has the input focus</summary>
    /// <remarks>
    ///   If this field is non-null, all keyboard input sent to the Gui is handed
    ///   over to the focused control. Otherwise, keyboard input is discarded.
    /// </remarks>
    private WeakReference<Control> focusedControl;
    /// <summary>Control the user has activated through one of the input devices</summary>
    private Control activatedControl;

    /// <summary>Number of keys being held down on the keyboard</summary>
    private int heldKeyCount;
    /// <summary>Keys on the keyboard the user is currently holding down</summary>
    BitArray heldKeys;
    /// <summary>Buttons on the game pad the user is currently holding down</summary>
    Buttons heldButtons;
    /// <summary>Mouse buttons currently being held down</summary>
    private MouseButtons heldMouseButtons;

  }

} // namespace Nuclex.UserInterface
