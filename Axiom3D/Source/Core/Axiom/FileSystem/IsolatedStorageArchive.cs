#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: IsolatedStorageArchive.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.FileSystem
{
    public static class IsolatedStorageExtensionMethods
    {
#if !NET_40
		public static bool DirectoryExists( this IsolatedStorageFile isolatedStorage, string directory )
		{
			return isolatedStorage.GetDirectoryNames( directory ).Length != 0;
		}

		public static bool FileExists( this IsolatedStorageFile isolatedStorage, string fileName )
		{
			return File.Exists( RootDirectoryGet( isolatedStorage ) + fileName );
		}

		public static FileStream CreateFile( this IsolatedStorageFile isolatedStorage, string fileName )
		{
			return File.Create( RootDirectoryGet( isolatedStorage ) + fileName );
		}

		public static FileStream OpenFile( this IsolatedStorageFile isolatedStorage, string fileName, FileMode mode,
		                                   FileAccess access )
		{
			return File.Open( RootDirectoryGet( isolatedStorage ) + fileName, mode, access );
		}

		private static readonly Class<IsolatedStorage>.Getter<String> RootDirectoryGet =
			Class<IsolatedStorage>.FieldGet<String>( "m_RootDir" );
#endif
    }

    /// <summary>
    /// </summary>
    public class IsolatedStorageArchive : FileSystemArchive
    {
        #region Fields and Properties

        private readonly IsolatedStorageFile isolatedStorage;

        #endregion Fields and Properties

        #region Utility Methods

        protected override bool DirectoryExists(string directory)
        {
            return this.isolatedStorage.DirectoryExists(directory);
        }

        protected override string[] getFiles(string dir, string pattern, bool recurse)
        {
            List<string> searchResults = new List<string>();
            string[] folders = this.isolatedStorage.GetDirectoryNames(dir);
            string[] files = this.isolatedStorage.GetFileNames(dir);

            if (recurse)
            {
                foreach (string folder in folders)
                {
                    searchResults.AddRange(getFilesRecursively(dir, pattern));
                }
            }
            else
            {
                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file);

                    if (pattern == "*" || pattern.Contains(ext))
                    {
                        searchResults.Add(file);
                    }
                }
            }
            return searchResults.ToArray();
        }

        protected override string[] getFilesRecursively(string dir, string pattern)
        {
            List<string> searchResults = new List<string>();
            string[] folders = this.isolatedStorage.GetDirectoryNames(dir);
            string[] files = this.isolatedStorage.GetFileNames(dir);

            foreach (string folder in folders)
            {
                searchResults.AddRange(getFilesRecursively(
                    dir + Path.GetFileName(folder) + Path.DirectorySeparatorChar, pattern));
            }

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

        public IsolatedStorageArchive(string name, string archType)
            : base(name, archType)
        {
            this.isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
        }

        #endregion Constructors and Destructors

        #region Archive Implementation

        public override void Load()
        {
            _basePath = Name + "/";
            IsReadOnly = false;

            SafeDirectoryChange(_basePath, () =>
                                               {
                                                   try
                                                   {
                                                       this.isolatedStorage.CreateFile(_basePath + @"__testWrite.Axiom");
                                                       this.isolatedStorage.DeleteFile(_basePath + @"__testWrite.Axiom");
                                                   }
                                                   catch (Exception)
                                                   {
                                                       IsReadOnly = true;
                                                   }
                                               });
        }

        public override Stream Create(string filename, bool overwrite)
        {
            if (IsReadOnly)
            {
                throw new AxiomException("Cannot create a file in a read-only archive.");
            }

            string fullPath = _basePath + Path.DirectorySeparatorChar + filename;
            bool exists = this.isolatedStorage.FileExists(fullPath);
            if (!exists || overwrite)
            {
                try
                {
                    return this.isolatedStorage.CreateFile(fullPath);
                }
                catch (Exception ex)
                {
                    throw new AxiomException("Failed to open file : " + filename, ex);
                }
            }
            return Open(fullPath, false);
        }

        public override Stream Open(string filename, bool readOnly)
        {
            Stream strm = null;

            SafeDirectoryChange(_basePath, () =>
                                               {
                                                   if (this.isolatedStorage.FileExists(_basePath + filename))
                                                   {
                                                       strm = this.isolatedStorage.OpenFile(_basePath + filename,
                                                                                            FileMode.Open,
                                                                                            readOnly
                                                                                                ? FileAccess.Read
                                                                                                : FileAccess.ReadWrite);
                                                   }
                                               });
            return strm;
        }

        public override bool Exists(string fileName)
        {
            return this.isolatedStorage.FileExists(_basePath + fileName);
        }

        #endregion Archive Implementation
    }

    /// <summary>
    ///   Specialization of IArchiveFactory for IsolatedStorage files.
    /// </summary>
    public class IsolatedStorageArchiveFactory : ArchiveFactory
    {
        private const string _type = "IsolatedStorage";

        #region ArchiveFactory Implementation

        public override string Type
        {
            get { return _type; }
        }

        public override Archive CreateInstance(string name)
        {
            return new IsolatedStorageArchive(name, _type);
        }

        public override void DestroyInstance(ref Archive obj)
        {
            obj.Dispose();
        }

        #endregion ArchiveFactory Implementation
    };
}