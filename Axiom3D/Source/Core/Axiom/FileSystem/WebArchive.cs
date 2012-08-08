#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: WebArchive.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.FileSystem
{
    /// <summary>
    /// </summary>
    public class WebArchive : FileSystemArchive
    {
        #region Fields and Properties

        /// <summary>
        ///   Is this archive capable of being monitored for additions, changes and deletions
        /// </summary>
        public override bool IsMonitorable
        {
            get { return false; }
        }

        #endregion Fields and Properties

        #region Utility Methods

        protected override bool DirectoryExists(string directory)
        {
            return true;
        }

        protected override string[] getFiles(string dir, string pattern, bool recurse)
        {
            List<string> searchResults = new List<string>();
            string[] files = !pattern.Contains("*") && Exists(dir + "/" + pattern)
                                 ? new[]
                                       {
                                           pattern
                                       }
                                 : new string[0]; //Directory.EnumerateFiles( dir );

            foreach (string file in files)
            {
                string ext = Path.GetExtension(file);

                if (pattern == "*" || pattern.Contains(ext))
                {
                    searchResults.Add(file);
                }
            }

            return searchResults.ToArray();
        }

        #endregion Utility Methods

        #region Constructors and Destructors

        public WebArchive(string name, string archType)
            : base(name, archType)
        {
        }

        #endregion Constructors and Destructors

        #region Archive Implementation

        public override bool IsCaseSensitive
        {
            get { return true; }
        }

        public override void Load()
        {
            _basePath = Name + "/";
            IsReadOnly = true;
            SafeDirectoryChange(_basePath, () => IsReadOnly = true);
        }

        public override Stream Create(string filename, bool overwrite)
        {
            throw new AxiomException("Cannot create a file in a read-only archive.");
        }

        public override Stream Open(string filename, bool readOnly)
        {
            if (!readOnly)
            {
                throw new AxiomException("Cannot create a file in a read-only archive.");
            }
            Stream result = null;
            AutoResetEvent wait = new AutoResetEvent(false);
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += (s, o) =>
                                        {
                                            if (o.Error == null)
                                            {
                                                result = o.Result;
                                            }
                                            wait.Set();
                                        };
            wc.OpenReadAsync(new Uri(_basePath + filename, UriKind.RelativeOrAbsolute));
            wait.WaitOne();
            return result;
        }

        public override bool Exists(string fileName)
        {
            return Open(fileName, true) != null;
        }

        #endregion Archive Implementation
    }

    /// <summary>
    ///   Specialization of IArchiveFactory for Web files.
    /// </summary>
    public class WebArchiveFactory : ArchiveFactory
    {
        private const string _type = "Web";

        #region ArchiveFactory Implementation

        public override string Type
        {
            get { return _type; }
        }

        public override Archive CreateInstance(string name)
        {
            return new WebArchive(name, _type);
        }

        public override void DestroyInstance(ref Archive obj)
        {
            obj.Dispose();
        }

        #endregion ArchiveFactory Implementation
    };
}