using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Core;

namespace Axiom.Framework.Configuration
{
    /// <summary>
    ///   Provides basic interface for loading and storing engine configuration
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// </summary>
        string LogFilename { get; }

        /// <summary>
        /// </summary>
        /// <param name="engine"> </param>
        bool RestoreConfiguration(Root engine);

        /// <summary>
        /// </summary>
        /// <param name="engine"> </param>
        void SaveConfiguration(Root engine);

        /// <summary>
        /// </summary>
        /// <param name="engine"> </param>
        /// <param name="defaultRenderer"> </param>
        void SaveConfiguration(Root engine, string defaultRenderer);

        /// <summary>
        /// </summary>
        /// <param name="mRoot"> </param>
        /// <returns> </returns>
        bool ShowConfigDialog(Root mRoot);
    }
}