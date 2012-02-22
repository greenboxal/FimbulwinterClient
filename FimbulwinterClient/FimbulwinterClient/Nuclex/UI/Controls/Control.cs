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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nuclex.Input;
using Nuclex.Support.Collections;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls {

  /// <summary>Represents an element in the user interface</summary>
  /// <remarks>
  ///   <para>
  ///     Controls are always arranged in a tree where each control except the one at
  ///     the root of the tree has exactly one owner (the one at the root has no owner).
  ///     The design actively prevents you from assigning a control as child to
  ///     multiple parents.
  ///   </para>
  ///   <para>
  ///     The controls in the Nuclex.UserInterface library are fully independent of
  ///     their graphical representation. That means you can construct a dialog
  ///     without even having a graphics device in place, that you can move your
  ///     dialogs between different graphics devices and that you do not have to
  ///     even think about graphics device resets and similar trouble.
  ///   </para>
  /// </remarks>
  public partial class Control {

    /// <summary>Initializes a new control</summary>
    public Control() : this(false) { }

    /// <summary>Initializes a new control</summary>
    /// <param name="affectsOrdering">
    ///   Whether the control comes to the top of the hierarchy when clicked
    /// </param>
    /// <remarks>
    ///   <para>
    ///     The <paramref name="affectsOrdering" /> parameter should be set for windows
    ///     and other free-floating panels which exist in parallel and which the user
    ///     might want to put on top of their siblings by clicking them. If the user
    ///     clicks on a child control of such a panel/window control, the panel/window
    ///     control will also be moved into the foreground.
    ///   </para>
    ///   <para>
    ///     It should not be set for normal controls which usually have no overlap,
    ///     like buttons. Otherwise, a button placed on the desktop could overdraw a
    ///     window when the button is clicked. The behavior would be well-defined and
    ///     controlled, but the user probably doesn't expect this ;-)
    ///   </para>
    /// </remarks>
    protected Control(bool affectsOrdering) {
      this.affectsOrdering = affectsOrdering;

      this.children = new ParentingControlCollection(this);
    }

    /// <summary>Children of the control</summary>
    public Collection<Control> Children {
      get { return this.children; }
    }

    /// <summary>
    ///   True if clicking the control or its children moves the control into
    ///   the foreground of the drawing hierarchy
    /// </summary>
    public bool AffectsOrdering {
      get { return this.affectsOrdering; }
    }

    /// <summary>Parent control this control is contained in</summary>
    /// <remarks>
    ///   Can be null, but this is only the case for free-floating controls that have
    ///   not been added into a Gui. The only control that really keeps this field
    ///   set to null whilst the Gui is active is the root control in the Gui class.
    /// </remarks>
    public Control Parent {
      get { return this.parent; }
    }

    /// <summary>Name that can be used to uniquely identify the control</summary>
    /// <remarks>
    ///   This name acts as an unique identifier for a control. It primarily serves
    ///   as a means to programmatically identify the control and as a debugging aid.
    ///   Duplicate names are not allowed and will result in an exception being
    ///   thrown, the only exception is when the control's name is set to null.
    /// </remarks>
    public string Name {
      get { return this.name; }
      set {

        // Don't do anything if we're given the same name we already have. This
        // is not a pure performance optimization, it also prevents the control
        // from reporting an name collision with itself in this special case :)
        if(value != this.name) {

          // Look for name collisions with our siblings
          Control parent = Parent;
          if(parent != null)
            if(parent.children.IsNameTaken(value))
              throw new DuplicateNameException("Another control is already using this name");

          // Everything seems to be ok, accept the new name
          this.name = value;

        }
      }
    }

    /// <summary>Moves the control into the foreground</summary>
    public void BringToFront() {

      // Doing nothing if we don't have a parent is okay since in that case,
      // we're the root and we're the frontmost control in any case. If the user
      // calls BringToFront() on a control before he integrates it into the GUI
      // tree, this is expected behavior and only logical.
      Control control = this;
      while(!ReferenceEquals(control.parent, null)) {
        ParentingControlCollection siblings = control.parent.children;
        siblings.MoveToStart(siblings.IndexOf(control));

        control = control.parent;
      }

    }

    /// <summary>
    ///   Obtains the absolute boundaries of the control in screen coordinates
    /// </summary>
    /// <returns>The control's absolute screen coordinate boundaries</returns>
    /// <remarks>
    ///   This method resolves the unified coordinates into absolute screen coordinates
    ///   that can be used to do hit-testing and rendering. The control is required to
    ///   be part of a GUI hierarchy that is assigned to a screen for this to work
    ///   since otherwise, there's no absolute coordinate frame into which the
    ///   unified coordinates could be resolved.
    /// </remarks>
    public RectangleF GetAbsoluteBounds() {

      // Is this the topmost control in the hierarchy (the desktop control)?
      if(ReferenceEquals(this.parent, null)) {

        // Make sure the control is attached to a screen, otherwise, it's a free
        // control not living in any GUI hierarchy and thus, does not have
        // absolute bounds yet.
        if(ReferenceEquals(this.screen, null)) {
          throw new InvalidOperationException(
            "Obtaining absolute bounds requires the control to be part of a screen"
          );
        }

        // Transform the unified coordinate bounds into absolute pixel coordinates
        // for the screen's dimensions
        return this.Bounds.ToOffset(this.screen.Width, this.screen.Height);

      } else { // Control is the child of another control

        // Recursively determine the bounds of the parent control until we end up
        // at the desktop control (or not, if this is a free living hierarchy, in
        // which case the exception above will be triggered as soon as the top of
        // the hierarchy is reached)
        RectangleF parentBounds = this.parent.GetAbsoluteBounds();

        // Determine the controls absolute position based on the absolute
        // dimensions and position of the parent control
        RectangleF controlBounds = this.Bounds.ToOffset(
          parentBounds.Width, parentBounds.Height
        );
        controlBounds.Offset(parentBounds.X, parentBounds.Y);

        // Done, controlBounds now contains the absolute screen coordinates of
        // the control's boundaries.
        return controlBounds;

      }

    }

    /// <summary>Called when an input command was sent to the control</summary>
    /// <param name="command">Input command that has been triggered</param>
    /// <returns>Whether the command has been processed by the control</returns>
    protected virtual bool OnCommand(Command command) { return false; }

    /// <summary>Called when a button on the gamepad has been pressed</summary>
    /// <param name="button">Button that has been pressed</param>
    /// <returns>
    ///   True if the button press was handled by the control, otherwise false.
    /// </returns>
    /// <remarks>
    ///   If the control indicates that it didn't handle the key press, it will not
    ///   receive the associated key release notification.
    /// </remarks>
    protected virtual bool OnButtonPressed(Buttons button) { return false; }

    /// <summary>Called when a button on the gamepad has been released</summary>
    /// <param name="button">Button that has been released</param>
    protected virtual void OnButtonReleased(Buttons button) { }

    /// <summary>Called when the mouse position is updated</summary>
    /// <param name="x">X coordinate of the mouse cursor on the control</param>
    /// <param name="y">Y coordinate of the mouse cursor on the control</param>
    protected virtual void OnMouseMoved(float x, float y) { }

    /// <summary>Called when a mouse button has been pressed down</summary>
    /// <param name="button">Index of the button that has been pressed</param>
    /// <returns>Whether the control has processed the mouse press</returns>
    /// <remarks>
    ///   If this method states that a mouse press is processed by returning
    ///   true, that means the control did something with it and the mouse press
    ///   should not be acted upon by any other listener.
    /// </remarks>
    protected virtual void OnMousePressed(MouseButtons button) { }

    /// <summary>Called when a mouse button has been released again</summary>
    /// <param name="button">Index of the button that has been released</param>
    protected virtual void OnMouseReleased(MouseButtons button) { }

    /// <summary>
    ///   Called when the mouse has left the control and is no longer hovering over it
    /// </summary>
    protected virtual void OnMouseLeft() { }

    /// <summary>
    ///   Called when the mouse has entered the control and is now hovering over it
    /// </summary>
    protected virtual void OnMouseEntered() { }

    /// <summary>Called when the mouse wheel has been rotated</summary>
    /// <param name="ticks">Number of ticks that the mouse wheel has been rotated</param>
    protected virtual void OnMouseWheel(float ticks) { }

    /// <summary>Called when a key on the keyboard has been pressed down</summary>
    /// <param name="keyCode">Code of the key that was pressed</param>
    /// <returns>
    ///   True if the key press was handled by the control, otherwise false.
    /// </returns>
    /// <remarks>
    ///   If the control indicates that it didn't handle the key press, it will not
    ///   receive the associated key release notification. This means that if you
    ///   return false from this method, you should under no circumstances do anything
    ///   with the information - you will not know when the key is released again
    ///   and another control might pick it up, causing a second key response.
    /// </remarks>
    protected virtual bool OnKeyPressed(Keys keyCode) { return false; }

    /// <summary>Called when a key on the keyboard has been released again</summary>
    /// <param name="keyCode">Code of the key that was released</param>
    protected virtual void OnKeyReleased(Keys keyCode) { }

    /// <summary>GUI instance this control belongs to. Can be null.</summary>
    internal Screen Screen {
      get { return this.screen; }
    }

    /// <summary>Called when a command was sent to the control</summary>
    /// <param name="command">Command to be injected</param>
    /// <returns>Whether the command has been processed</returns>
    internal bool ProcessCommand(Command command) {

      switch(command) {

        // These are not supported on the control level
        case Command.SelectPrevious:
        case Command.SelectNext: {
          return false;
        }

        // These can be handled by user code if he so wishes
        case Command.Up:
        case Command.Down:
        case Command.Left:
        case Command.Right:
        case Command.Accept:
        case Command.Cancel: {
          return OnCommand(command);
        }

        // Value not contained in enumation - should not be happening!
        default: {
          throw new ArgumentException("Invalid command", "command");
        }
      }
    }

    /// <summary>Assigns a new parent to the control</summary>
    /// <param name="parent">New parent to assign to the control</param>
    internal void SetParent(Control parent) {
      this.parent = parent;

      // Have we been assigned to a parent?
      if(this.parent != null) {

        // If this ownership change transferred us to a different gui, we will
        // have to migrate our visual and also the visuals of all our children.
        if(!ReferenceEquals(this.screen, parent.screen))
          SetScreen(parent.screen);

      } else { // No parent, we're now officially an orphan ;)

        // Orphans don't have screens!
        SetScreen(null);

      }
    }

    /// <summary>Assigns a new GUI to the control</summary>
    /// <param name="gui">New GUI to assign to the control</param>
    internal void SetScreen(Screen gui) {
      this.screen = gui;

      this.children.SetScreen(gui);
    }
    
    /// <summary>Control the mouse is currently over</summary>
    internal protected Control MouseOverControl {
      get { return this.mouseOverControl; }
    }
    
    /// <summary>Control that currently captured incoming input</summary>
    internal protected Control ActivatedControl {
      get { return this.activatedControl; }
    }

    /// <summary>Location and extents of the control</summary>
    public UniRectangle Bounds;

    /// <summary>Control this control is contained in</summary>
    private Control parent;
    /// <summary>GUI instance this control has been added to. Can be null.</summary>
    private Screen screen;
    /// <summary>Name of the control instance (for programmatic identification)</summary>
    private string name;
    /// <summary>Whether this control can obtain the input focus</summary>
    private bool affectsOrdering;
    /// <summary>Child controls belonging to this control</summary>
    /// <remarks>
    ///   Child controls are any controls that belong to this control. They don't
    ///   neccessarily need to be situated in this control's client area, but
    ///   their positioning will be relative to the parent's location.
    /// </remarks>
    private ParentingControlCollection children;

  }

} // namespace Nuclex.UserInterface.Controls
