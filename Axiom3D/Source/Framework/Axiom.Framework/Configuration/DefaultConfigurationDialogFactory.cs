using Axiom.Core;

namespace Axiom.Framework.Configuration
{
    internal class DefaultConfigurationDialogFactory : ConfigurationDialogFactory
    {
        #region Overrides of ConfigurationDialogFactory

        /// <summary>
        ///   Create an instance of the ConfigurationDialog
        /// </summary>
        /// <returns> </returns>
        public override IConfigurationDialog CreateConfigurationDialog(Root engine, ResourceGroupManager resourceManager)
        {
            return new WinFormConfigurationDialog(engine, resourceManager);
        }

        #endregion
    }
}