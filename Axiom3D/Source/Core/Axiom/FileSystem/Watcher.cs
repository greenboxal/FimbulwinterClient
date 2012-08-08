#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id:"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.IO;
using Axiom.Core;
using Axiom.Graphics;

#endregion Namespace Declarations

namespace Axiom.FileSystem
{
    public class Watcher
    {
        #region Fields and Properties

#if !( XBOX || XBOX360 || WINDOWS_PHONE || ANDROID || IOS || SILVERLIGHT)
        private readonly FileSystemWatcher _monitor;
#endif

        #endregion Fields and Properties

        #region Construction and Destruction

        public Watcher(string path, bool recurse)
        {
#if !( XBOX || XBOX360 || WINDOWS_PHONE || ANDROID || IOS || SILVERLIGHT)
            // Initialize FileSystemWatcher
            this._monitor = new FileSystemWatcher();
            this._monitor.Path = path;
            // Watch for changes in LastAccess and LastWrite times, and the renaming of files or directories.
            this._monitor.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Watch all files.
            this._monitor.Filter = "*.*";
            this._monitor.IncludeSubdirectories = recurse;

            // Add event handlers.
            this._monitor.Changed += OnChanged;
            this._monitor.Created += OnChanged;
            this._monitor.Deleted += OnChanged;
            this._monitor.Renamed += OnRenamed;

            // Begin watching.
            this._monitor.EnableRaisingEvents = true;
            LogManager.Instance.Write("File monitor created for {0}.", path);
#endif
        }

        #endregion Construction and Destruction

        #region Methods

#if !( XBOX || XBOX360 || WINDOWS_PHONE || ANDROID || IOS || SILVERLIGHT)
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            LogManager.Instance.Write("File: " + e.FullPath + " " + e.ChangeType);
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            LogManager.Instance.Write("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }
#endif

        #endregion Methods
    }
}