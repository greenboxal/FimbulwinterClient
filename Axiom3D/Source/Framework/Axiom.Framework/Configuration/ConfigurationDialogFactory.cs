using System;
using Axiom.Core;

namespace Axiom.Framework.Configuration
{
    /// <summary>
    /// </summary>
    public abstract class ConfigurationDialogFactory : IConfigurationDialogFactory
    {
        /// <summary>
        ///   Create an instance of the ConfigurationDialog
        /// </summary>
        /// <returns> </returns>
        public abstract IConfigurationDialog CreateConfigurationDialog(Root engine, ResourceGroupManager resourceManager);
    }
}