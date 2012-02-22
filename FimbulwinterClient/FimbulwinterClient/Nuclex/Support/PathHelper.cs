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
using System.IO;

namespace Nuclex.Support {

  /// <summary>Utility class for path operations</summary>
  public static class PathHelper {

    /// <summary>Converts an absolute path into a relative one</summary>
    /// <param name="basePath">Base directory the new path should be relative to</param>
    /// <param name="absolutePath">Absolute path that will be made relative</param>
    /// <returns>
    ///   A path relative to the indicated base directory that matches the
    ///   absolute path given.
    /// </returns>
    public static string MakeRelative(string basePath, string absolutePath) {
      string[] baseDirectories = basePath.Split(Path.DirectorySeparatorChar);
      string[] absoluteDirectories = absolutePath.Split(Path.DirectorySeparatorChar);

      // Find the common root path of both paths so we know from which point on
      // the two paths will differ
      int lastCommonRoot = -1;
      int commonLength = Math.Min(baseDirectories.Length, absoluteDirectories.Length);
      for(int index = 0; index < commonLength; ++index) {
        if(absoluteDirectories[index] == baseDirectories[index])
          lastCommonRoot = index;
        else
          break;
      }

      // If the paths don't share a common root, we have to use an absolute path.
      // Should the absolutePath parameter actually be a relative path, this will
      // also trigger the return of the absolutePath as-is.
      if(lastCommonRoot == -1)
        return absolutePath;

      // Calculate the required length for the StringBuilder in order to be slightly
      // more friendly in terms of memory usage.
      int requiredLength = (baseDirectories.Length - (lastCommonRoot + 1)) * 3;
      for(int index = lastCommonRoot + 1; index < absoluteDirectories.Length; ++index)
        requiredLength += absoluteDirectories[index].Length + 1;

      StringBuilder relativePath = new StringBuilder(requiredLength);

      // Go to the common path by adding .. until we're where we want to be
      for(int index = lastCommonRoot + 1; index < baseDirectories.Length; ++index) {
        if(baseDirectories[index].Length > 0) {
          if(relativePath.Length > 0) // We don't want the path to start with a slash
            relativePath.Append(Path.DirectorySeparatorChar);

          relativePath.Append("..");
        }
      }

      // Now that we're in the common root folder, enter the folders that
      // the absolute target path has in addition to the root folder.
      for(int index = lastCommonRoot + 1; index < absoluteDirectories.Length; index++) {
        if(relativePath.Length > 0) // We don't want the path to start with a slash
          relativePath.Append(Path.DirectorySeparatorChar);

        relativePath.Append(absoluteDirectories[index]);
      }

      return relativePath.ToString();
    }

  }

} // namespace Nuclex.Support
