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
using System.Text;

namespace Nuclex.Input {

  /// <summary>Provides helper methods for collections</summary>
  internal static class CollectionHelper {

    /// <summary>Returns an item from a list if the index exists</summary>
    /// <typeparam name="ItemType">Type of the item that will be returned</typeparam>
    /// <param name="list">List the item will be taken from</param>
    /// <param name="index">Index from which the item will be taken</param>
    /// <returns>The item if the index existed, otherwise a default item</returns>
    public static ItemType GetIfExists<ItemType>(IList<ItemType> list, int index) {
      if (list.Count > index) {
        return list[index];
      } else {
        return default(ItemType);
      }
    }

    /// <summary>Disposes all items in a list</summary>
    /// <typeparam name="ItemType">Type of item that will be disposed</typeparam>
    /// <param name="list">List containing the items that will be disposed</param>
    public static void DisposeItems<ItemType>(IList<ItemType> list) {
      for (int index = list.Count - 1; index >= 0; --index) {
        IDisposable disposable = list[index] as IDisposable;
        if (disposable != null) {
          disposable.Dispose();
        }
      }
    }

  }

} // namespace Nuclex.Input
