using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class SettingsCompareService(
    IEnvironmentsCompareSettingsService settingsService,
    IComparableSettingsMasterProvider comparableSettingsMasterProvider,
    IEnvironmentsCompareClient environmentsCompareClient)
    : ISettingsCompareService
{
    public const string CurrentEnvironmentName = "Current";

    public async Task<SettingsComparisonResult> CompareAsync(IList<string> environmentNames, string baseEnvironmentName = null)
    {
        var comparableEnvironmentSettings = await GetComparableEnvironmentsAsync(environmentNames);

        if (baseEnvironmentName == null)
        {
            baseEnvironmentName = comparableEnvironmentSettings.FirstOrDefault()?.EnvironmentName;
        }

        return CompareInternal(comparableEnvironmentSettings, environmentNames, baseEnvironmentName);
    }

    protected virtual async Task<IList<ComparableEnvironmentSettings>> GetComparableEnvironmentsAsync(IList<string> environmentNames)
    {
        var comparableEnvironments = settingsService.ComparableEnvironments.Where(x => environmentNames.Contains(x.Name)).ToList();

        var result = await environmentsCompareClient.GetSettingsAsync(comparableEnvironments);

        if (environmentNames.Contains(CurrentEnvironmentName))
        {
            var currentEnvironmentSettings = AbstractTypeFactory<ComparableEnvironmentSettings>.TryCreateInstance();
            currentEnvironmentSettings.SettingScopes = await comparableSettingsMasterProvider.GetAllComparableSettingsAsync();
            currentEnvironmentSettings.EnvironmentName = CurrentEnvironmentName;
            result.Add(currentEnvironmentSettings);
        }

        return result;
    }

    protected virtual SettingsComparisonResult CompareInternal(IList<ComparableEnvironmentSettings> comparableEnvironmentSettings, IList<string> environmentNames, string baseEnvironmentName)
    {
        var result = AbstractTypeFactory<SettingsComparisonResult>.TryCreateInstance();

        foreach (var scopeName in comparableEnvironmentSettings
            .SelectMany(x => x.SettingScopes)
            .Select(x => x.ScopeName)
            .Distinct())
        {
            var resultScope = AbstractTypeFactory<ComparedEnvironmentSettingScope>.TryCreateInstance();
            result.SettingScopes.Add(resultScope);

            foreach (var groupName in comparableEnvironmentSettings
                .SelectMany(x => x.SettingScopes)
                .Where(x => x.ScopeName == scopeName)
                .SelectMany(x => x.SettingGroups)
                .Select(x => x.GroupName)
                .Distinct())
            {
                var resultGroup = AbstractTypeFactory<ComparedEnvironmentSettingGroup>.TryCreateInstance();
                resultScope.SettingGroups.Add(resultGroup);

                foreach (var settingName in comparableEnvironmentSettings
                    .SelectMany(x => x.SettingScopes)
                    .Where(x => x.ScopeName == scopeName)
                    .SelectMany(x => x.SettingGroups)
                    .Where(x => x.GroupName == groupName)
                    .SelectMany(x => x.Settings)
                    .Select(x => x.Name)
                    .Distinct())
                {
                    var resultSetting = AbstractTypeFactory<ComparedEnvironmentSetting>.TryCreateInstance();
                    resultSetting.Name = settingName;

                    var resultSettingBaseValue = AbstractTypeFactory<ComparedEnvironmentSettingValue>.TryCreateInstance();
                    resultSettingBaseValue.EnvironmentName = baseEnvironmentName;
                    resultSettingBaseValue.Value = FindSettingValue(comparableEnvironmentSettings, baseEnvironmentName, scopeName, groupName, settingName);
                    resultSetting.ComparedValues.Add(resultSettingBaseValue);

                    foreach (var comparableEnvironmentName in environmentNames.Where(x => x != baseEnvironmentName))
                    {
                        var resultSettingComparableValue = AbstractTypeFactory<ComparedEnvironmentSettingValue>.TryCreateInstance();
                        resultSettingComparableValue.EnvironmentName = comparableEnvironmentName;
                        resultSettingComparableValue.Value = FindSettingValue(comparableEnvironmentSettings, comparableEnvironmentName, scopeName, groupName, settingName);
                        resultSettingComparableValue.EqualsBaseValue = SettingValuesAreEqual(resultSettingComparableValue.Value, resultSettingBaseValue.Value);
                        resultSetting.ComparedValues.Add(resultSettingComparableValue);
                    }
                }
            }
        }

        return result;
    }

    protected object FindSettingValue(IList<ComparableEnvironmentSettings> comparableEnvironmentSettings, string environmentName, string scopeName, string groupName, string settingName)
    {
        var environmentSettings = comparableEnvironmentSettings.FirstOrDefault(x => x.EnvironmentName == environmentName);
        if (environmentSettings == null)
        {
            return null;
        }

        var scope = environmentSettings.SettingScopes.FirstOrDefault(x => x.ScopeName == scopeName);
        if (scope == null)
        {
            return null;
        }

        var group = scope.SettingGroups.FirstOrDefault(x => x.GroupName == groupName);
        if (group == null)
        {
            return null;
        }

        var setting = group.Settings.FirstOrDefault(x => x.Name == settingName);
        if (setting == null)
        {
            return null;
        }

        return setting.Value;
    }

    protected bool SettingValuesAreEqual(object value1, object value2)
    {
        return value1 == value2;
    }
}
