#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: EmbeddedArchive.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.FileSystem
{
    /// <summary>
    /// </summary>
    public class EmbeddedArchive : FileSystemArchive
    {
        #region Fields and Properties

        private readonly Assembly assembly;
        private readonly List<string> resources;

        public override bool IsMonitorable
        {
            get { return false; }
        }

        #endregion Fields and Properties

        #region Utility Methods

        protected override bool DirectoryExists(string directory)
        {
            return (from res in this.resources
                    where res.StartsWith(directory)
                    select res).Any();
        }

        protected override void findFiles(string pattern, bool recursive, List<string> simpleList,
                                          FileInfoList detailList,
                                          string currentDir)
        {
            if (pattern == "")
            {
                pattern = "*";
            }
            if (currentDir == "")
            {
                currentDir = _basePath;
            }

            string[] files = getFilesRecursively(currentDir, pattern);

            foreach (string file in files)
            {
                if (simpleList != null)
                {
                    simpleList.Add(file);
                }

                if (detailList != null)
                {
                    detailList.Add(new FileInfo
                                       {
                                           Archive = this,
                                           Filename = file,
                                           Basename = file.Substring(currentDir.Length),
                                           Path = currentDir,
                                           CompressedSize = 0,
                                           UncompressedSize = 0,
                                           ModifiedTime = DateTime.Now
                                       });
                }
            }
        }

        protected override string[] getFiles(string dir, string pattern, bool recurse)
        {
            IEnumerable<string> files = !pattern.Contains("*") && Exists(dir + pattern)
                                            ? new[]
                                                  {
                                                      pattern
                                                  }
                                            : from res in this.resources
                                              where res.StartsWith(dir)
                                              select res;

            if (pattern == "*")
            {
                return files.ToArray();
            }

            pattern = pattern.Substring(pattern.LastIndexOf('*') + 1);

            return (from file in files
                    where file.EndsWith(pattern)
                    select file).ToArray<string>();
        }

        protected override string[] getFilesRecursively(string dir, string pattern)
        {
            return getFiles(dir, pattern, true);
        }

        #endregion Utility Methods

        #region Constructors and Destructors

        public EmbeddedArchive(string name, string archType)
            : base(name.Split('/')[0], archType)
        {
            string named = Name + ",";

            this.assembly = (from a in AssemblyEx.Neighbors()
                             where a.FullName.StartsWith(named)
                             select a).First();
            Name = name.Replace('/', '.');
            this.resources = (from resource in this.assembly.GetManifestResourceNames()
                              //where resource.StartsWith(Name)
                              select resource).ToList();
            this.resources.Sort();
        }

        #endregion Constructors and Destructors

        #region Archive Implementation

        public override bool IsCaseSensitive
        {
            get { return true; }
        }

        public override void Load()
        {
            _basePath = Name + ".";
            IsReadOnly = true;
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
            return
                this.assembly.GetManifestResourceStream(
                    this.resources[this.resources.BinarySearch(_basePath + filename)]);
        }

        public override bool Exists(string fileName)
        {
            return this.resources.BinarySearch(_basePath + fileName) >= 0;
        }

        #endregion Archive Implementation
    }

    /// <summary>
    ///   Specialization of IArchiveFactory for Embedded files.
    /// </summary>
    public class EmbeddedArchiveFactory : ArchiveFactory
    {
        private const string _type = "Embedded";

        #region ArchiveFactory Implementation

        public override string Type
        {
            get { return _type; }
        }

        public override Archive CreateInstance(string name)
        {
            return new EmbeddedArchive(name, _type);
        }

        public override void DestroyInstance(ref Archive obj)
        {
            obj.Dispose();
        }

        #endregion ArchiveFactory Implementation
    };
}