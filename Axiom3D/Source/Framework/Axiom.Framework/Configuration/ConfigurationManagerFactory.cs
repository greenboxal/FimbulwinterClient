using System;

namespace Axiom.Framework.Configuration
{
    public class ConfigurationManagerFactory
    {
        public static IConfigurationManager CreateDefault()
        {
            PlatformID platform = Environment.OSVersion.Platform;
            switch (platform)
            {
                case PlatformID.Xbox:
                    return new XBoxConfigurationManager();
#if !(XBOX || XBOX360 || WINDOWS_PHONE)
                case PlatformID.MacOSX:
#endif
                case PlatformID.Unix:
#if SILVERLIGHT && WINDOWS_PHONE
				case PlatformID.NokiaS60:
#endif
                case PlatformID.WinCE:
                    return new XBoxConfigurationManager();
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                default:
#if !(XBOX || XBOX360 || WINDOWS_PHONE || SILVERLIGHT || ANDROID)
                    return new DefaultConfigurationManager();
#else
					return null;
#endif
            }
        }
    }
}