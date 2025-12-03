using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class ComparableAppSettingsProvider(IConfiguration configuration) : IComparableSettingsProvider
{
    private const string OptionsSectionName = "EnvironmentsCompare:WhiteList";

    protected virtual IList<string> VisibleSectionKeys
    {
        get
        {
            var defaults = new List<string>
            {
                "DatabaseProvider",
                "ConnectionStrings",
                "SqlServer",
                "Serilog",
                "FrontendSecurity",
                "VirtoCommerce",
                "Auth",
                "Assets",
                "Notifications",
                "IdentityOptions",
                "ExternalModules",
                "Search",
                "Content",
                "Authorization",
                "SecurityHeaders",
                "AzureAd",
                "Caching",
                "Crud",
                "PushNotifications",
                "LoginPageUI",
                "DefaultMainMenuState",
            };

            return MergeWithConfiguration(defaults, GetInclude("SectionKeys"), GetExclude("SectionKeys"));
        }
    }

    protected virtual IList<string> PublicSettingKeys
    {
        get
        {
            var defaults = new List<string>
            {
                "Assets:AzureBlobStorage:CdnUrl",
                "Assets:FileSystem:PublicUrl",
                "Assets:FileSystem:RootPath",
                "Assets:Provider",
                "Auth:Audience",
                "Auth:Authority",
                "Auth:PrivateKeyPath",
                "Auth:PublicCertPath",
                "Authorization:AccessTokenLifeTime",
                "Authorization:AllowApiAccessForCustomers",
                "Authorization:LimitedCookiePermissions",
                "Authorization:RefreshTokenLifeTime",
                "Authorization:ReturnPasswordHash",
                "AzureAd:ApplicationId",
                "AzureAd:AuthenticationCaption",
                "AzureAd:AuthenticationType",
                "AzureAd:AzureAdInstance",
                "AzureAd:DefaultUserType",
                "AzureAd:Enabled",
                "AzureAd:TenantId",
                "AzureAd:UsePreferredUsername",
                "Caching:CacheEnabled",
                "Caching:CacheSlidingExpiration",
                "Caching:Redis:ChannelName",
                "Content:AzureBlobStorage:CdnUrl",
                "Content:AzureBlobStorage:RootPath",
                "Content:FileSystem:PublicUrl",
                "Content:FileSystem:RootPath",
                "Content:Provider",
                "Crud:MaxResultWindow",
                "DatabaseProvider",
                "DefaultMainMenuState",
                "DefaultMainMenuState:items:0:isFavorite",
                "DefaultMainMenuState:items:0:order",
                "DefaultMainMenuState:items:0:path",
                "DefaultMainMenuState:items:1:isFavorite",
                "DefaultMainMenuState:items:1:order",
                "DefaultMainMenuState:items:1:path",
                "DefaultMainMenuState:items:2:isFavorite",
                "DefaultMainMenuState:items:2:order",
                "DefaultMainMenuState:items:2:path",
                "DefaultMainMenuState:items:3:isFavorite",
                "DefaultMainMenuState:items:3:order",
                "DefaultMainMenuState:items:3:path",
                "DefaultMainMenuState:items:4:isFavorite",
                "DefaultMainMenuState:items:4:order",
                "DefaultMainMenuState:items:4:path",
                "DefaultMainMenuState:items:5:isFavorite",
                "DefaultMainMenuState:items:5:order",
                "DefaultMainMenuState:items:5:path",
                "DefaultMainMenuState:items:6:isFavorite",
                "DefaultMainMenuState:items:6:order",
                "DefaultMainMenuState:items:6:path",
                "DefaultMainMenuState:items:7:isFavorite",
                "DefaultMainMenuState:items:7:order",
                "DefaultMainMenuState:items:7:path",
                "DefaultMainMenuState:items:8:isFavorite",
                "DefaultMainMenuState:items:8:order",
                "DefaultMainMenuState:items:8:path",
                "ExternalModules:AuthorizationToken",
                "ExternalModules:AutoInstallModuleBundles:0",
                "ExternalModules:IncludePrerelease",
                "ExternalModules:ModulesManifestUrl",
                "FrontendSecurity:OrganizationMaintainerRole",
                "IdentityOptions:Lockout:DefaultLockoutTimeSpan",
                "IdentityOptions:Password:RepeatedResetPasswordTimeLimit",
                "IdentityOptions:Password:RequireDigit",
                "IdentityOptions:Password:RequiredLength",
                "IdentityOptions:Password:RequireNonAlphanumeric",
                "IdentityOptions:User:MaxPasswordAge",
                "IdentityOptions:User:RemindPasswordExpiryInDay",
                "IdentityOptions:User:RequireUniqueEmail",
                "LoginPageUI:BackgroundUrl",
                "LoginPageUI:PatternUrl",
                "LoginPageUI:Preset",
                "LoginPageUI:Presets:0:BackgroundUrl",
                "LoginPageUI:Presets:0:Name",
                "LoginPageUI:Presets:0:PatternUrl",
                "LoginPageUI:Presets:1:BackgroundUrl",
                "LoginPageUI:Presets:1:Name",
                "LoginPageUI:Presets:1:PatternUrl",
                "Notifications:DefaultSender",
                "Notifications:Gateway",
                "Notifications:SendGrid:ApiKey",
                "Notifications:Smtp:ForceSslTls",
                "Notifications:Smtp:Login",
                "Notifications:Smtp:Port",
                "Notifications:Smtp:SmtpServer",
                "PushNotifications:ForceWebSockets",
                "PushNotifications:HubUrl",
                "PushNotifications:RedisBackplane:ChannelName",
                "PushNotifications:ScalabilityMode",
                "Search:AzureSearch:Key",
                "Search:AzureSearch:SearchServiceName",
                "Search:ContentFullTextSearchEnabled",
                "Search:ElasticSearch:EnableHttpCompression",
                "Search:ElasticSearch:Key",
                "Search:ElasticSearch:Server",
                "Search:ElasticSearch:User",
                "Search:Lucene:Path",
                "Search:OrderFullTextSearchEnabled",
                "Search:PickupLocationFullTextSearchEnabled",
                "Search:Provider",
                "Search:Scope",
                "SecurityHeaders",
                "SecurityHeaders:FrameAncestors",
                "SecurityHeaders:FrameOptions",
                "Serilog:Enrich:0",
                "Serilog:MinimumLevel:Default",
                "Serilog:MinimumLevel:Override:Microsoft",
                "Serilog:MinimumLevel:Override:Microsoft.Hosting.Lifetime",
                "Serilog:MinimumLevel:Override:System",
                "Serilog:MinimumLevel:Override:VirtoCommerce.Platform.Modules",
                "Serilog:MinimumLevel:Override:VirtoCommerce.Platform.Web.Startup",
                "Serilog:Using:0",
                "Serilog:Using:1",
                "Serilog:WriteTo:0",
                "Serilog:WriteTo:1",
                "VirtoCommerce:AllowInsecureHttp",
                "VirtoCommerce:ApplicationInsights:EnableSqlCommandTextInstrumentation",
                "VirtoCommerce:ApplicationInsights:IgnoreSqlTelemetryOptions:QueryIgnoreSubstrings:0",
                "VirtoCommerce:ApplicationInsights:IgnoreSqlTelemetryOptions:QueryIgnoreSubstrings:1",
                "VirtoCommerce:ApplicationInsights:IgnoreSqlTelemetryOptions:QueryIgnoreSubstrings:2",
                "VirtoCommerce:ApplicationInsights:SamplingOptions:Adaptive:EvaluationInterval",
                "VirtoCommerce:ApplicationInsights:SamplingOptions:Adaptive:InitialSamplingPercentage",
                "VirtoCommerce:ApplicationInsights:SamplingOptions:Adaptive:MaxSamplingPercentage",
                "VirtoCommerce:ApplicationInsights:SamplingOptions:Adaptive:MaxTelemetryItemsPerSecond",
                "VirtoCommerce:ApplicationInsights:SamplingOptions:Adaptive:MinSamplingPercentage",
                "VirtoCommerce:ApplicationInsights:SamplingOptions:Adaptive:MovingAverageRatio",
                "VirtoCommerce:ApplicationInsights:SamplingOptions:Adaptive:SamplingPercentageDecreaseTimeout",
                "VirtoCommerce:ApplicationInsights:SamplingOptions:Adaptive:SamplingPercentageIncreaseTimeout",
                "VirtoCommerce:ApplicationInsights:SamplingOptions:Fixed:SamplingPercentage",
                "VirtoCommerce:ApplicationInsights:SamplingOptions:IncludedTypes",
                "VirtoCommerce:ApplicationInsights:SamplingOptions:Processor",
                "VirtoCommerce:DiscoveryPath",
                "VirtoCommerce:GraphQL:ForbiddenAuthenticationTypes:0",
                "VirtoCommerce:GraphQLPlayground:Enable",
                "VirtoCommerce:Hangfire:AutomaticRetryCount",
                "VirtoCommerce:Hangfire:JobStorageType",
                "VirtoCommerce:Hangfire:MySqlStorageOptions:InvisibilityTimeout",
                "VirtoCommerce:Hangfire:MySqlStorageOptions:QueuePollInterval",
                "VirtoCommerce:Hangfire:PostgreSqlStorageOptions:DisableGlobalLocks",
                "VirtoCommerce:Hangfire:PostgreSqlStorageOptions:InvisibilityTimeout",
                "VirtoCommerce:Hangfire:PostgreSqlStorageOptions:QueuePollInterval",
                "VirtoCommerce:Hangfire:PostgreSqlStorageOptions:UsePageLocksOnDequeue",
                "VirtoCommerce:Hangfire:PostgreSqlStorageOptions:UseRecommendedIsolationLevel",
                "VirtoCommerce:Hangfire:SqlServerStorageOptions:CommandBatchMaxTimeout",
                "VirtoCommerce:Hangfire:SqlServerStorageOptions:DisableGlobalLocks",
                "VirtoCommerce:Hangfire:SqlServerStorageOptions:EnableHeavyMigrations",
                "VirtoCommerce:Hangfire:SqlServerStorageOptions:InactiveStateExpirationTimeout",
                "VirtoCommerce:Hangfire:SqlServerStorageOptions:QueuePollInterval",
                "VirtoCommerce:Hangfire:SqlServerStorageOptions:SlidingInvisibilityTimeout",
                "VirtoCommerce:Hangfire:SqlServerStorageOptions:TryAutoDetectSchemaDependentOptions",
                "VirtoCommerce:Hangfire:SqlServerStorageOptions:UseIgnoreDupKeyOption",
                "VirtoCommerce:Hangfire:SqlServerStorageOptions:UsePageLocksOnDequeue",
                "VirtoCommerce:Hangfire:SqlServerStorageOptions:UseRecommendedIsolationLevel",
                "VirtoCommerce:Hangfire:UseHangfireServer",
                "VirtoCommerce:LicenseActivationUrl",
                "VirtoCommerce:PlatformUI:Enable",
                "VirtoCommerce:SampleDataUrl",
                "VirtoCommerce:Stores:DefaultStore",
                "VirtoCommerce:Swagger:Enable",
                "VirtoCommerce:UseResponseCompression"
            };

            return MergeWithConfiguration(defaults, GetInclude("SettingKeys"), GetExclude("SettingKeys"));
        }
    }

    public Task<IList<ComparableSettingScope>> GetComparableSettingsAsync()
    {
        var result = AbstractTypeFactory<ComparableSettingScope>.TryCreateInstance();
        result.ScopeName = "AppSettings";

        foreach (var section in configuration.GetChildren().Where(x => VisibleSectionKeys.Contains(x.Key, StringComparer.OrdinalIgnoreCase)))
        {
            var sectionValues = new Dictionary<string, string>();
            EnumerateSectionRecursive(section, section.Key, sectionValues);

            if (sectionValues.Count == 0)
            {
                continue;
            }

            var resultGroup = AbstractTypeFactory<ComparableSettingGroup>.TryCreateInstance();
            resultGroup.GroupName = section.Key;
            result.SettingGroups.Add(resultGroup);

            foreach (var key in sectionValues.Select(x => x.Key))
            {
                var resultSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
                resultSetting.Name = key;
                resultSetting.Value = sectionValues[key];
                resultSetting.IsSecret = !PublicSettingKeys.Contains(key, StringComparer.OrdinalIgnoreCase);
                resultGroup.Settings.Add(resultSetting);
            }
        }

        return Task.FromResult((IList<ComparableSettingScope>)[result]);
    }

    private static void EnumerateSectionRecursive(IConfigurationSection section, string parentPath, Dictionary<string, string> allValues)
    {
        foreach (var child in section.GetChildren())
        {
            var currentPath = parentPath.IsNullOrEmpty() ? child.Key : $"{parentPath}:{child.Key}";

            if (child.Value != null)
            {
                allValues[currentPath] = child.Value;
            }
            else
            {
                EnumerateSectionRecursive(child, currentPath, allValues);
            }
        }
    }

    private IList<string> MergeWithConfiguration(IList<string> defaults, IEnumerable<string> include, IEnumerable<string> exclude)
    {
        // Start from defaults
        var result = new List<string>(defaults);

        // Apply includes (deduplicated, case-insensitive)
        if (!include.IsNullOrEmpty())
        {
            foreach (var key in include.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                if (!result.Contains(key, StringComparer.OrdinalIgnoreCase))
                {
                    result.Add(key);
                }
            }
        }

        // Apply excludes (case-insensitive)
        if (!exclude.IsNullOrEmpty())
        {
            result = result
                .Where(k => !exclude.Contains(k, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        return result;
    }

    private IEnumerable<string> GetInclude(string keyGroupName)
    {
        return configuration
            .GetSection($"{OptionsSectionName}:{keyGroupName}:Include")
            .Get<string[]>();
    }

    private IEnumerable<string> GetExclude(string keyGroupName)
    {
        return configuration
            .GetSection($"{OptionsSectionName}:{keyGroupName}:Exclude")
            .Get<string[]>();
    }
}
