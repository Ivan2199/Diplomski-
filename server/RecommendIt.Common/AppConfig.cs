using System;
using System.Configuration;

namespace GeoTagMap.Common
{
    public static class AppConfig
    {
        public static Guid GetGuid(string stringGuid)
        {
            if (Guid.TryParse(ConfigurationManager.AppSettings[stringGuid], out Guid resultGuid))
            {
                return resultGuid;
            }

            throw new ConfigurationErrorsException("Invalid Guid configuration value.");
        }
    }
}