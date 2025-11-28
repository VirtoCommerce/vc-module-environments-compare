using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.EnvironmentsCompare.Core;

public static class ModuleConstants
{
    public static class Security
    {
        public static class Permissions
        {
            public const string Access = "environments-compare:access";

            public static string[] AllPermissions { get; } = [Access];
        }
    }

    public static class Settings
    {
        public static class General
        {
            public static SettingDescriptor EnvironmentsCompareEnabled { get; } = new()
            {
                Name = "EnvironmentsCompare.Enabled",
                GroupName = "EnvironmentsCompare|General",
                ValueType = SettingValueType.Boolean,
                DefaultValue = false,
            };

            public static IEnumerable<SettingDescriptor> AllGeneralSettings
            {
                get
                {
                    yield return EnvironmentsCompareEnabled;
                }
            }
        }

        public static IEnumerable<SettingDescriptor> AllSettings
        {
            get
            {
                return General.AllGeneralSettings;
            }
        }
    }

    public static class EnvironmentsCompare
    {
        public const string CurrentEnvironmentName = "Current";

        public const string ApiAuthorizationKeyHeaderName = "api_key";
        public const string ApiEnvironmentsCompareRoute = "api/environments-compare-external";
    }
}
