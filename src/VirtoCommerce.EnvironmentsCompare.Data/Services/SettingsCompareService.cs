using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core;
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
    protected const decimal DecimalComparisonEpsilon = 0.00001m;

    public async Task<SettingsComparisonResult> CompareAsync(IList<string> environmentNames, string baseEnvironmentName = null)
    {
        var comparableEnvironmentSettings = await GetComparableEnvironmentsAsync(environmentNames);

        if (baseEnvironmentName == null || !comparableEnvironmentSettings.Any(x => x.EnvironmentName == baseEnvironmentName))
        {
            baseEnvironmentName = comparableEnvironmentSettings.FirstOrDefault()?.EnvironmentName;
        }

        return CompareInternal(comparableEnvironmentSettings, baseEnvironmentName);
    }

    protected virtual async Task<IList<ComparableEnvironmentSettings>> GetComparableEnvironmentsAsync(IList<string> environmentNames)
    {
        var comparableEnvironments = settingsService.ComparableEnvironments.Where(x => environmentNames.Contains(x.Name)).ToList();

        var result = await environmentsCompareClient.GetSettingsAsync(comparableEnvironments);

        if (environmentNames.Contains(ModuleConstants.EnvironmentsCompare.CurrentEnvironmentName))
        {
            var currentEnvironmentSettings = AbstractTypeFactory<ComparableEnvironmentSettings>.TryCreateInstance();
            currentEnvironmentSettings.SettingScopes = await comparableSettingsMasterProvider.GetAllComparableSettingsAsync();
            currentEnvironmentSettings.EnvironmentName = ModuleConstants.EnvironmentsCompare.CurrentEnvironmentName;
            result.Add(currentEnvironmentSettings);
        }

        return result;
    }

    protected virtual SettingsComparisonResult CompareInternal(IList<ComparableEnvironmentSettings> comparableEnvironmentSettings, string baseEnvironmentName)
    {
        var result = AbstractTypeFactory<SettingsComparisonResult>.TryCreateInstance();

        foreach (var environment in comparableEnvironmentSettings)
        {
            var resultEnvironment = AbstractTypeFactory<ComparedEnvironment>.TryCreateInstance();
            resultEnvironment.EnvironmentName = environment.EnvironmentName;
            resultEnvironment.IsCurrent = environment.EnvironmentName == ModuleConstants.EnvironmentsCompare.CurrentEnvironmentName;
            resultEnvironment.IsComparisonBase = environment.EnvironmentName == baseEnvironmentName;
            resultEnvironment.ErrorMessage = environment.ErrorMessage;
            result.ComparedEnvironments.Add(resultEnvironment);
        }

        result.ComparedEnvironments = result.ComparedEnvironments
            .OrderBy(x => x.IsComparisonBase ? 0 : 1)
            .ThenBy(x => x.IsCurrent ? 0 : 1)
            .ToList();

        foreach (var scopeName in comparableEnvironmentSettings
            .SelectMany(x => x.SettingScopes)
            .Select(x => x.ScopeName)
            .Distinct()
            .OrderBy(x => x))
        {
            var resultScope = AbstractTypeFactory<ComparedEnvironmentSettingScope>.TryCreateInstance();
            resultScope.ScopeName = scopeName;
            result.SettingScopes.Add(resultScope);

            foreach (var groupName in comparableEnvironmentSettings
                .SelectMany(x => x.SettingScopes)
                .Where(x => x.ScopeName == scopeName)
                .SelectMany(x => x.SettingGroups)
                .Select(x => x.GroupName)
                .Distinct()
                .OrderBy(x => x))
            {
                var resultGroup = AbstractTypeFactory<ComparedEnvironmentSettingGroup>.TryCreateInstance();
                resultGroup.GroupName = groupName;
                resultScope.SettingGroups.Add(resultGroup);

                foreach (var settingName in comparableEnvironmentSettings
                    .SelectMany(x => x.SettingScopes)
                    .Where(x => x.ScopeName == scopeName)
                    .SelectMany(x => x.SettingGroups)
                    .Where(x => x.GroupName == groupName)
                    .SelectMany(x => x.Settings)
                    .Select(x => x.Name)
                    .Distinct()
                    .OrderBy(x => x))
                {
                    var resultSetting = AbstractTypeFactory<ComparedEnvironmentSetting>.TryCreateInstance();
                    resultSetting.Name = settingName;
                    resultGroup.Settings.Add(resultSetting);

                    var resultSettingBaseValue = AbstractTypeFactory<ComparedEnvironmentSettingValue>.TryCreateInstance();
                    resultSettingBaseValue.EnvironmentName = baseEnvironmentName;
                    resultSetting.ComparedValues.Add(resultSettingBaseValue);

                    var resultSettingBaseValueFindResult = FindSettingValue(comparableEnvironmentSettings, baseEnvironmentName, scopeName, groupName, settingName);
                    resultSettingBaseValue.Value = resultSettingBaseValueFindResult.Value;
                    resultSettingBaseValue.ErrorMessage = resultSettingBaseValueFindResult.ErrorMessage;
                    resultSettingBaseValue.EqualsBaseValue = true;

                    foreach (var comparableEnvironment in result.ComparedEnvironments.Where(x => !x.IsComparisonBase))
                    {
                        var resultSettingComparableValue = AbstractTypeFactory<ComparedEnvironmentSettingValue>.TryCreateInstance();
                        resultSettingComparableValue.EnvironmentName = comparableEnvironment.EnvironmentName;
                        resultSetting.ComparedValues.Add(resultSettingComparableValue);

                        var resultSettingComparableValueFindResult = FindSettingValue(comparableEnvironmentSettings, comparableEnvironment.EnvironmentName, scopeName, groupName, settingName);

                        resultSettingComparableValue.ErrorMessage = resultSettingComparableValueFindResult.ErrorMessage;

                        if (comparableEnvironment.ErrorMessage.IsNullOrEmpty())
                        {
                            resultSettingComparableValue.Value = resultSettingComparableValueFindResult.Value;
                            resultSettingComparableValue.EqualsBaseValue = SettingValuesAreEqual(resultSettingBaseValue, resultSettingComparableValue);
                        }
                    }
                }
            }
        }

        return result;
    }

    protected static (object Value, string ErrorMessage) FindSettingValue(IList<ComparableEnvironmentSettings> comparableEnvironmentSettings, string environmentName, string scopeName, string groupName, string settingName)
    {
        var environmentSettings = comparableEnvironmentSettings.FirstOrDefault(x => x.EnvironmentName == environmentName);
        if (environmentSettings == null)
        {
            return (null, null);
        }

        var scope = environmentSettings.SettingScopes.FirstOrDefault(x => x.ScopeName == scopeName);
        if (scope == null)
        {
            return (null, null);
        }
        if (!scope.ErrorMessage.IsNullOrEmpty())
        {
            return (null, $"Settings scope (provider) error: {environmentSettings.ErrorMessage}");
        }

        var group = scope.SettingGroups.FirstOrDefault(x => x.GroupName == groupName);
        if (group == null)
        {

            return (null, null);
        }

        var setting = group.Settings.FirstOrDefault(x => x.Name == settingName);
        if (setting == null)
        {
            return (null, null);
        }

        return (setting.Value, null);
    }

    protected virtual bool SettingValuesAreEqual(ComparedEnvironmentSettingValue baseValue, ComparedEnvironmentSettingValue comparableValue)
    {
        if (!baseValue.ErrorMessage.IsNullOrEmpty() || !comparableValue.ErrorMessage.IsNullOrEmpty())
        {
            return false;
        }

        if (baseValue.Value == null && comparableValue.Value == null)
        {
            return true;
        }
        else if (baseValue.Value == null || comparableValue.Value == null)
        {
            return false;
        }

        if (IsFloatingPointNumber(baseValue.Value) && IsFloatingPointNumber(comparableValue.Value))
        {
            return Math.Abs(Convert.ToDecimal(baseValue.Value) - Convert.ToDecimal(comparableValue.Value)) < DecimalComparisonEpsilon;
        }

        return baseValue.Value.ToString() == comparableValue.Value.ToString();
    }

    protected static bool IsFloatingPointNumber(object value)
    {
        return value is float || value is double || value is decimal;
    }
}
