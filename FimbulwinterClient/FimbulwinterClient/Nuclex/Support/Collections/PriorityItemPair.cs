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
using System.Text;

namespace Nuclex.Support.Collections {

  /// <summary>An pair of a priority and an item</summary>
  public struct PriorityItemPair<PriorityType, ItemType> {

    /// <summary>Initializes a new priority / item pair</summary>
    /// <param name="priority">Priority of the item in the pair</param>
    /// <param name="item">Item to be stored in the pair</param>
    public PriorityItemPair(PriorityType priority, ItemType item) {
      this.Priority = priority;
      this.Item = item;
    }

    /// <summary>Priority assigned to this priority / item pair</summary>
    public PriorityType Priority;
    /// <summary>Item contained in this priority / item pair</summary>
    public ItemType Item;

    /// <summary>Converts the priority / item pair into a string</summary>
    /// <returns>A string describing the priority / item pair</returns>
    public override string ToString() {
      int length = 4;

      // Convert the priority value into a string or use the empty string
      // constant if the ToString() overload returns null
      string priorityString = this.Priority.ToString();
      if(priorityString != null)
        length += priorityString.Length;
      else
        priorityString = string.Empty;

      // Convert the item value into a string or use the empty string
      // constant if the ToString() overload returns null
      string itemString = this.Item.ToString();
      if(itemString != null)
        length += itemString.Length;
      else
        itemString = string.Empty;

      // Concatenate priority and item into a single string
      StringBuilder builder = new StringBuilder(length);
      builder.Append('[');
      builder.Append(priorityString);
      builder.Append(", ");
      builder.Append(itemString);
      builder.Append(']');
      return builder.ToString();
    }

  }

} // namespace Nuclex.Support.Collections
