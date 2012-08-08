using Axiom.Core;

namespace Axiom.Framework.Configuration
{
    /// <summary>
    /// </summary>
    public abstract class ConfigurationManagerBase : IConfigurationManager
    {
        #region Fields and Properties

        /// <summary>
        /// </summary>
        protected string ConfigurationFile { get; set; }

        #endregion Fields and Properties

        #region Construction and Destruction

        /// <summary>
        /// </summary>
        /// <param name="configurationFilename"> </param>
        protected ConfigurationManagerBase(string configurationFilename)
        {
            ConfigurationFile = configurationFilename;
        }

        #endregion Construction and Destruction

        #region IConfigurationManager Implementation

        /// <summary>
        /// </summary>
        public virtual string LogFilename { get; protected set; }

        /// <summary>
        /// </summary>
        /// <param name="engine"> </param>
        public abstract bool RestoreConfiguration(Root engine);


        /// <summary>
        /// </summary>
        /// <param name="engine"> </param>
        public abstract void SaveConfiguration(Root engine);

        /// <summary>
        /// </summary>
        /// <param name="engine"> </param>
        /// <param name="defaultRenderer"> </param>
        public abstract void SaveConfiguration(Root engine, string defaultRenderer);

        /// <summary>
        /// </summary>
        public abstract bool ShowConfigDialog(Root engine);

        #endregion IConfigurationManager Implementation
    }
}