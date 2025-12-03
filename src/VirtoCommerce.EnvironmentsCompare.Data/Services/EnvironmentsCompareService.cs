using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class EnvironmentsCompareService(
    IEnvironmentsCompareSettingsService settingsService,
    IComparableSettingsMasterProvider comparableSettingsMasterProvider,
    IEnvironmentsCompareClient environmentsCompareClient)
    : IEnvironmentsCompareService
{
    protected const decimal DecimalComparisonEpsilon = 0.00001m;

    public async Task<SettingsComparisonResult> CompareAsync(IList<string> environmentNames, string baseEnvironmentName = null, bool showAll = false)
    {
        var comparableEnvironmentSettings = await GetComparableEnvironmentsAsync(environmentNames);

        if (baseEnvironmentName == null || !comparableEnvironmentSettings.Any(x => x.EnvironmentName == baseEnvironmentName))
        {
            baseEnvironmentName = comparableEnvironmentSettings.FirstOrDefault()?.EnvironmentName;
        }

        var result = CompareEnvironmentSettings(comparableEnvironmentSettings, baseEnvironmentName);

        if (!showAll)
        {
            RemoveSettingsWithEqualValues(result);
        }

        return result;
    }

    public virtual async Task<IList<ComparableEnvironmentSettings>> GetComparableEnvironmentsAsync(IList<string> environmentNames)
    {
        var comparableEnvironments = settingsService.ComparableEnvironments.Where(x => environmentNames.Contains(x.Name)).ToList();

        var result = await environmentsCompareClient.GetSettingsAsync(comparableEnvironments);

        if (environmentNames.Contains(ModuleConstants.EnvironmentsCompare.CurrentEnvironmentName))
        {
            var currentEnvironmentSettings = AbstractTypeFactory<ComparableEnvironmentSettings>.TryCreateInstance();
            currentEnvironmentSettings.IsCurrent = true;
            currentEnvironmentSettings.SettingScopes = await comparableSettingsMasterProvider.GetAllComparableSettingsAsync();
            currentEnvironmentSettings.EnvironmentName = ModuleConstants.EnvironmentsCompare.CurrentEnvironmentName;
            result.Add(currentEnvironmentSettings);
        }

        return result;
    }

    protected virtual SettingsComparisonResult CompareEnvironmentSettings(IList<ComparableEnvironmentSettings> comparableEnvironmentSettings, string baseEnvironmentName)
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

        result.ComparedEnvironments = [.. result.ComparedEnvironments
            .OrderBy(x => x.IsComparisonBase ? 0 : 1)
            .ThenBy(x => x.IsCurrent ? 0 : 1)];

        foreach (var scopeName in comparableEnvironmentSettings
            .SelectMany(x => x.SettingScopes)
            .Select(x => x.ScopeName)
            .Distinct()
            .Order())
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
                .Order())
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
                    .Order())
                {
                    var resultSetting = CompareEnvironmentSetting(result.ComparedEnvironments, comparableEnvironmentSettings, baseEnvironmentName, scopeName, groupName, settingName);

                    resultGroup.Settings.Add(resultSetting);
                }
            }
        }

        return result;
    }

    protected virtual ComparedEnvironmentSetting CompareEnvironmentSetting(
        IList<ComparedEnvironment> comparedEnvironments,
        IList<ComparableEnvironmentSettings> comparableEnvironmentSettings,
        string baseEnvironmentName,
        string scopeName,
        string groupName,
        string settingName)
    {
        var result = AbstractTypeFactory<ComparedEnvironmentSetting>.TryCreateInstance();
        result.Name = settingName;

        var resultSettingBaseValue = AbstractTypeFactory<ComparedEnvironmentSettingValue>.TryCreateInstance();
        resultSettingBaseValue.EnvironmentName = baseEnvironmentName;
        result.ComparedValues.Add(resultSettingBaseValue);

        var resultSettingBaseValueFindResult = FindSettingValue(comparableEnvironmentSettings, baseEnvironmentName, scopeName, groupName, settingName);
        resultSettingBaseValue.Value = resultSettingBaseValueFindResult.Value;
        resultSettingBaseValue.ErrorMessage = resultSettingBaseValueFindResult.ErrorMessage;
        resultSettingBaseValue.EqualsBaseValue = resultSettingBaseValueFindResult.Found;

        foreach (var comparableEnvironment in comparedEnvironments.Where(x => !x.IsComparisonBase))
        {
            var resultSettingComparableValue = AbstractTypeFactory<ComparedEnvironmentSettingValue>.TryCreateInstance();
            resultSettingComparableValue.EnvironmentName = comparableEnvironment.EnvironmentName;
            result.ComparedValues.Add(resultSettingComparableValue);

            if (comparableEnvironment.ErrorMessage.IsNullOrEmpty())
            {
                var resultSettingComparableValueFindResult = FindSettingValue(comparableEnvironmentSettings, comparableEnvironment.EnvironmentName, scopeName, groupName, settingName);
                resultSettingComparableValue.Value = resultSettingComparableValueFindResult.Value;
                resultSettingComparableValue.ErrorMessage = resultSettingComparableValueFindResult.ErrorMessage;
                resultSettingComparableValue.EqualsBaseValue = resultSettingBaseValueFindResult.Found && resultSettingComparableValueFindResult.Found && SettingValuesAreEqual(resultSettingBaseValue, resultSettingComparableValue);
            }
        }

        return result;
    }

    protected static (object Value, bool Found, string ErrorMessage) FindSettingValue(IList<ComparableEnvironmentSettings> comparableEnvironmentSettings, string environmentName, string scopeName, string groupName, string settingName)
    {
        var environmentSettings = comparableEnvironmentSettings.FirstOrDefault(x => x.EnvironmentName == environmentName);
        if (environmentSettings == null)
        {
            return (null, false, null);
        }

        var scope = environmentSettings.SettingScopes.FirstOrDefault(x => x.ScopeName == scopeName);
        if (scope == null)
        {
            return (null, false, "Setting scope not found");
        }
        else if (!scope.ErrorMessage.IsNullOrEmpty())
        {
            return (null, true, $"Setting scope (provider) error: {environmentSettings.ErrorMessage}");
        }

        var group = scope.SettingGroups.FirstOrDefault(x => x.GroupName == groupName);
        if (group == null)
        {

            return (null, false, "Setting group not found");
        }

        var setting = group.Settings.FirstOrDefault(x => x.Name == settingName);
        if (setting == null)
        {
            return (null, false, "Setting not found");
        }

        return (setting.Value, true, null);
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

        return IsFloatingPointNumber(baseValue.Value) && IsFloatingPointNumber(comparableValue.Value)
            ? Math.Abs(Convert.ToDecimal(baseValue.Value, CultureInfo.InvariantCulture) - Convert.ToDecimal(comparableValue.Value, CultureInfo.InvariantCulture)) < DecimalComparisonEpsilon
            : baseValue.Value.ToString() == comparableValue.Value.ToString();
    }

    protected static bool IsFloatingPointNumber(object value)
    {
        return value is float or double or decimal;
    }

    protected virtual void RemoveSettingsWithEqualValues(SettingsComparisonResult comparisonResult)
    {
        foreach (var scope in comparisonResult.SettingScopes.ToList())
        {
            foreach (var group in scope.SettingGroups.ToList())
            {
                foreach (var setting in group.Settings.ToList())
                {
                    RemoveSettingsWithEqualValues(group, setting);
                }

                if (!group.Settings.Any())
                {
                    scope.SettingGroups.Remove(group);
                }
            }

            if (!scope.SettingGroups.Any())
            {
                comparisonResult.SettingScopes.Remove(scope);
            }
        }
    }

    protected virtual void RemoveSettingsWithEqualValues(ComparedEnvironmentSettingGroup group, ComparedEnvironmentSetting setting)
    {
        var allValuesEqual = !setting.ComparedValues.Any(x => x.EqualsBaseValue == false);
        if (allValuesEqual)
        {
            group.Settings.Remove(setting);
        }
    }
}
