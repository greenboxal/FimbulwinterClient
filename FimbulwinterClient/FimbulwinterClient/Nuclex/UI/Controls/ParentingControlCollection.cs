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
using System.Collections.ObjectModel;

using Nuclex.Support.Collections;

namespace Nuclex.UserInterface.Controls {

  /// <summary>Collection of GUI controls</summary>
  /// <remarks>
  ///   This class is for internal use only. Do not expose it to the user. If it was
  ///   exposed, the user might decide to use it for storing his own controls, causing
  ///   exceptions because the collection tries to parent the controls which are already
  ///   belonging to another collection.
  /// </remarks>
  internal class ParentingControlCollection : Collection<Control> {

    /// <summary>Initializes a new parenting control collection</summary>
    /// <param name="parent">Parent control to assign to all children</param>
    public ParentingControlCollection(Control parent) {
      this.parent = parent;
    }

    /// <summary>Clears all elements from the collection</summary>
    protected override void ClearItems() {
      for(int index = 0; index < base.Count; ++index)
        unassignParent(base[index]);

      base.ClearItems();
    }

    /// <summary>Inserts a new element into the collection</summary>
    /// <param name="index">Index at which to insert the element</param>
    /// <param name="item">Item to be inserted</param>
    protected override void InsertItem(int index, Control item) {
      ensureIntegrity(item);

      base.InsertItem(index, item);
      assignParent(item);
    }

    /// <summary>Removes an element from the collection</summary>
    /// <param name="index">Index of the element to remove</param>
    protected override void RemoveItem(int index) {
      unassignParent(base[index]);
      base.RemoveItem(index);
    }

    /// <summary>Takes over a new element that is directly assigned</summary>
    /// <param name="index">Index of the element that was assigned</param>
    /// <param name="item">New item</param>
    protected override void SetItem(int index, Control item) {
      ensureIntegrity(item);

      unassignParent(base[index]);
      assignParent(item);
    }

    /// <summary>Switches the control to a specific GUI</summary>
    /// <param name="screen">Screen that owns the control from now on</param>
    internal void SetScreen(Screen screen) {
      this.screen = screen;

      for(int index = 0; index < base.Count; ++index)
        base[index].SetScreen(screen);
    }

    /// <summary>
    ///   Checks whether the provided name is already taken by a control
    /// </summary>
    /// <param name="name">Id that will be checked</param>
    /// <returns>True if the id is already taken, false otherwise</returns>
    internal bool IsNameTaken(string name) {

      // Empty names are an exception and will not be checked for duplicates.
      if(name == null)
        return false;

      // Look for any controls with the provided name. This is a stupid sequential
      // search, but given the typical number of controls in a Gui and the fact
      // that this operation usually only happens once, there's no point in adding
      // the overhead of managing a synchronized look-up dictionary here.
      for(int index = 0; index < base.Count; ++index)
        if(base[index].Name == name)
          return true;

      // If we reach this point, no control is using the specified name.
      return false;

    }

#if false
    /// <summary>Moves the specified control to the end of the list</summary>
    /// <param name="controlIndex">
    ///   Index of the control that will be moved to the end of the list
    /// </param>
    internal void MoveToEnd(int controlIndex) {
      Control control = base[controlIndex];

      // We explicitely circumvent the additional logic for adding and removing items
      // in this collection since we're only relocating an item. Removal and readdition
      // have no risk of causing an exception in a normal collection, otherwise the
      // rollback attempt would be futile anyway since it would mean to repeat exactly
      // what has caused failed: adding an item ;)
      base.RemoveAt(controlIndex);
      base.Add(control);
    }
#endif

    /// <summary>Moves the specified control to the start of the list</summary>
    /// <param name="controlIndex">
    ///   Index of the control that will be moved to the start of the list
    /// </param>
    internal void MoveToStart(int controlIndex) {
      Control control = base[controlIndex];

      // We explicitely circumvent the additional logic for adding and removing items
      // in this collection since we're only relocating an item. Removal and readdition
      // have no risk of causing an exception in a normal collection, otherwise the
      // rollback attempt would be futile anyway since it would mean to repeat exactly
      // what has caused failed: adding an item ;)
      base.RemoveAt(controlIndex);
      base.Insert(0, control);
    }

    /// <summary>Ensures the integrity of the parent/child relationships</summary>
    /// <param name="proposedChild">Control that is to become one of our childs</param>
    private void ensureIntegrity(Control proposedChild) {

      // The item must not have a parent (otherwise, by being added to this collection,
      // it would either be contained twice in the same collection or have two parents).
      if(!ReferenceEquals(proposedChild.Parent, null))
        throw new InvalidOperationException("Control already is the child of another control");

      // The item must not become its own parent. I cannot imagine this ever happenning
      // unless someone deliberately tried to crash the GUI library :)
      if(ReferenceEquals(this.parent, proposedChild))
        throw new InvalidOperationException("Attempt to instate control as its own parent");

      // The item also must not be any of our parent's parents (and so on). Otherwise,
      // a stack overflow is likely to occur.
      if(isParent(proposedChild))
        throw new InvalidOperationException(
          "Attempt to instate one of the control's parents as its child"
        );

      // We also do not allow a child control to have the same id as an existing
      // control (with the exception of an empty name)
      if(IsNameTaken(proposedChild.Name))
        throw new DuplicateNameException(
          "The name of the added control has already been taken by another child"
        );

    }

    /// <summary>
    ///   Determines whether the provided control is a parent of this control.
    /// </summary>
    /// <param name="control">Control to check for parentage</param>
    /// <returns>True if the control is one of our parents, otherwise false</returns>
    /// <remarks>
    ///   This method takes into account all ancestors up to the tree's root.
    /// </remarks>
    private bool isParent(Control control) {
      Control parent = this.parent;
      while(parent != null) {

        // Does one of our parents happen to be the control we are checking?
        if(ReferenceEquals(parent, control))
          return true;

        // Walk upwards in the tree until we reach the top
        parent = parent.Parent;

      }

      // The control is not one of our parents in the tree all the way up
      // to the tree's root.
      return false;
    }

    /// <summary>Gives up the parentage on the item provided</summary>
    /// <param name="item">Item to be unparented</param>
    private void unassignParent(Control item) {
      item.SetParent(null);
    }

    /// <summary>Sets up the parentage on the specified item</summary>
    /// <param name="item">Item to be parented</param>
    private void assignParent(Control item) {
      item.SetParent(this.parent);
    }

    /// <summary>GUI this control is currently assigned to. Can be null.</summary>
    private Screen screen;
    /// <summary>Parent control to assign to all controls in this collection.</summary>
    private Control parent;

  }

} // namespace Nuclex.UserInterface.Controls
