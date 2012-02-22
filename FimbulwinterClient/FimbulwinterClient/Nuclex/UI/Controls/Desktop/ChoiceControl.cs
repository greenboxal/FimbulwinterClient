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

namespace Nuclex.UserInterface.Controls.Desktop {

  /// <summary>Control displaying an exclusive choice the user can select</summary>
  /// <remarks>
  ///   The choice control is equivalent to a radio button - if more than one
  ///   choice control is on a dialog, only one can be selected at a time.
  ///   To have several choice groups on a dialog, use panels to group them.
  /// </remarks>
  public class ChoiceControl : PressableControl {

    /// <summary>Will be triggered when the choice is changed</summary>
    public event EventHandler Changed;

    /// <summary>Called when the button is pressed</summary>
    protected override void OnPressed() {
      if(!this.Selected) {
        this.Selected = true;

        // Unselect all sibling choice controls in the same container
        unselectSiblings();

        OnChanged();
      }
    }

    /// <summary>Triggers the changed event</summary>
    protected virtual void OnChanged() {
      if(Changed != null) {
        Changed(this, EventArgs.Empty);
      }
    }

    /// <summary>Disables all sibling choices on the same level</summary>
    private void unselectSiblings() {

      // Disable any other choices in the same frame
      if(Parent != null) {
        Collection<Control> siblings = Parent.Children;
        for(int index = 0; index < siblings.Count; ++index) {
          ChoiceControl control = siblings[index] as ChoiceControl;
          if((control != null) && (control != this) && (control.Selected)) {
            control.Selected = false;
            control.OnChanged();
          }
        }
      }

    }

    /// <summary>Text that will be shown on the button</summary>
    public string Text;

    /// <summary>Whether the choice is currently selected</summary>
    public bool Selected;

  }

} // namespace Nuclex.UserInterface.Controls.Desktop
