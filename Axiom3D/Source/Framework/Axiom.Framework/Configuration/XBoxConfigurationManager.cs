using System;

namespace Axiom.Framework.Configuration
{
    internal class XBoxConfigurationManager : ConfigurationManagerBase
    {
        #region Fields and Properties

        public static string DefaultLogFileName = "axiom.log";

        #endregion Fields and Properties

        #region Construction and Destruction

        /// <summary>
        /// </summary>
        public XBoxConfigurationManager()
            : base(DefaultLogFileName)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="configurationFile"> </param>
        public XBoxConfigurationManager(string configurationFile)
            : base(configurationFile)
        {
        }

        #endregion Construction and Destruction

        #region ConfigurationManagerBase Implementation

        /// <summary>
        /// </summary>
        /// <param name="engine"> </param>
        /// <returns> </returns>
        public override bool RestoreConfiguration(Core.Root engine)
        {
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="engine"> </param>
        public override void SaveConfiguration(Core.Root engine)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="engine"> </param>
        /// <param name="defaultRenderer"> </param>
        public override void SaveConfiguration(Core.Root engine, string defaultRenderer)
        {
        }


        /// <summary>
        /// </summary>
        /// <param name="engine"> </param>
        /// <returns> </returns>
        public override bool ShowConfigDialog(Core.Root engine)
        {
            return true;
        }

        #endregion ConfigurationManagerBase Implementation
    }
}