#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: ArchiveManager.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.FileSystem
{
    /// <summary>
    ///   This class manages the available ArchiveFactory plugins.
    /// </summary>
    /// <ogre name="ArchiveManager">
    ///   <file name="OgreArchiveManager.h" revision="1.8.2.1" lastUpdated="5/18/2006" lastUpdatedBy="Borrillis" />
    ///   <file name="OgreArchiveManager.cpp" revision="1.14.2.1" lastUpdated="5/18/2006" lastUpdatedBy="Borrillis" />
    /// </ogre>
    public sealed class ArchiveManager : Singleton<ArchiveManager>
    {
        #region Fields and Properties

        /// <summary>
        ///   The list of factories
        /// </summary>
        private readonly Dictionary<string, ArchiveFactory> _factories = new Dictionary<string, ArchiveFactory>();

        private readonly Dictionary<string, Archive> _archives = new Dictionary<string, Archive>();

        #endregion

        #region Constructor

        #endregion Constructor

        #region Methods

        /// <summary>
        ///   Opens an archive for file reading.
        /// </summary>
        /// <remarks>
        ///   The archives are created using class factories within
        ///   extension libraries.
        /// </remarks>
        /// <param name="filename"> The filename that will be opened </param>
        /// <param name="archiveType"> The library that contains the data-handling code </param>
        /// <returns> If the function succeeds, a valid pointer to an Archive object is returned.
        ///   <para />
        ///   If the function fails, an exception is thrown. </returns>
        public Archive Load(string filename, string archiveType)
        {
            Archive arch = null;
            if (!this._archives.TryGetValue(filename, out arch))
            {
                // Search factories
                ArchiveFactory fac = null;
                if (!this._factories.TryGetValue(archiveType, out fac))
                {
                    throw new AxiomException("Cannot find an archive factory to deal with archive of type {0}",
                                             archiveType);
                }

                arch = fac.CreateInstance(filename);
                arch.Load();
                this._archives.Add(filename, arch);
            }
            return arch;
        }

        #region Unload Method

        /// <summary>
        ///   Unloads an archive.
        /// </summary>
        /// <remarks>
        ///   You must ensure that this archive is not being used before removing it.
        /// </remarks>
        /// <param name="arch"> The Archive to unload </param>
        public void Unload(Archive arch)
        {
            Unload(arch.Name);
        }

        /// <summary>
        ///   Unloads an archive.
        /// </summary>
        /// <remarks>
        ///   You must ensure that this archive is not being used before removing it.
        /// </remarks>
        /// <param name="filename"> The Archive to unload </param>
        public void Unload(string filename)
        {
            Archive arch = this._archives[filename];

            if (arch != null)
            {
                arch.Unload();

                ArchiveFactory fac = this._factories[arch.Type];
                if (fac == null)
                {
                    throw new AxiomException("Cannot find an archive factory to deal with archive of type {0}",
                                             arch.Type);
                }
                this._archives.Remove(arch.Name);
                fac.DestroyInstance(ref arch);
            }
        }

        #endregion Unload Method

        /// <summary>
        ///   Add an archive factory to the list
        /// </summary>
        /// <param name="factory"> The factory itself </param>
        public void AddArchiveFactory(ArchiveFactory factory)
        {
            if (this._factories.ContainsKey(factory.Type))
            {
                throw new AxiomException("Attempted to add the {0} factory to ArchiveManager more than once.",
                                         factory.Type);
            }

            this._factories.Add(factory.Type, factory);
            LogManager.Instance.Write("ArchiveFactory for archive type {0} registered.", factory.Type);
        }

        #endregion Methods

        #region Singleton<ArchiveManager> Implementation

        /// <summary>
        ///   Called when the engine is shutting down.
        /// </summary>
        protected override void dispose(bool disposeManagedResources)
        {
            if (!isDisposed)
            {
                if (disposeManagedResources)
                {
                    // Unload & delete resources in turn
                    foreach (KeyValuePair<string, Archive> arch in this._archives)
                    {
                        // Unload
                        arch.Value.Unload();

                        // Find factory to destroy
                        ArchiveFactory fac = this._factories[arch.Value.Type];
                        if (fac == null)
                        {
                            // Factory not found
                            throw new AxiomException("Cannot find an archive factory to deal with archive of type {0}",
                                                     arch.Value.Type);
                        }
                        Archive tmp = arch.Value;
                        fac.DestroyInstance(ref tmp);
                    }

                    // Empty the list
                    this._archives.Clear();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            base.dispose(disposeManagedResources);
        }

        #endregion Singleton<ArchiveManager> Implementation
    }
}