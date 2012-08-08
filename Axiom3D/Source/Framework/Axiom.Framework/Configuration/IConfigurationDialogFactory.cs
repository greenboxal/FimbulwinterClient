using Axiom.Core;

namespace Axiom.Framework.Configuration
{
    public interface IConfigurationDialogFactory
    {
        IConfigurationDialog CreateConfigurationDialog(Root engine, ResourceGroupManager resourceManager);
    }
}