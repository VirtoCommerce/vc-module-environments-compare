using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class ComparablePlatformSettingsProvider(ISettingsManager settingsManager) : IComparableSettingsProvider
{
    public async Task<IList<ComparableSettingScope>> GetComparableSettingsAsync()
    {
        var result = AbstractTypeFactory<ComparableSettingScope>.TryCreateInstance();
        result.ScopeName = "PlatformSettings";

        foreach (var group in settingsManager.AllRegisteredSettings
            .GroupBy(x => x.GroupName))
        {
            var resultGroup = AbstractTypeFactory<ComparableSettingGroup>.TryCreateInstance();
            resultGroup.GroupName = group.Key.IsNullOrEmpty() ? "Without group" : group.Key;
            result.SettingGroups.Add(resultGroup);

            foreach (var setting in group)
            {
                var resultSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
                resultSetting.Name = setting.Name;
                resultSetting.Value = await GetSettingValueAsync(setting);
                resultSetting.IsSecret = IsSettingSecret(setting);
                resultGroup.Settings.Add(resultSetting);
            }
        }

        return [result];
    }

    protected async Task<object> GetSettingValueAsync(SettingDescriptor settingDescriptor)
    {
        if (settingDescriptor.ValueType is SettingValueType.Boolean)
        {
            return await settingsManager.GetValueAsync<bool>(settingDescriptor);
        }

        if (settingDescriptor.ValueType is SettingValueType.Integer or SettingValueType.PositiveInteger)
        {
            return await settingsManager.GetValueAsync<int>(settingDescriptor);
        }

        if (settingDescriptor.ValueType is SettingValueType.Decimal)
        {
            return await settingsManager.GetValueAsync<decimal>(settingDescriptor);
        }

        if (settingDescriptor.ValueType is SettingValueType.DateTime)
        {
            return await settingsManager.GetValueAsync<DateTime>(settingDescriptor);
        }

        if (settingDescriptor.ValueType is SettingValueType.Json)
        {
            return await settingsManager.GetValueAsync<string>(settingDescriptor);
        }

        if (settingDescriptor.ValueType is SettingValueType.SecureString or SettingValueType.ShortText or SettingValueType.LongText)
        {
            return await settingsManager.GetValueAsync<string>(settingDescriptor);
        }

        return null;
    }

    protected virtual bool IsSettingSecret(SettingDescriptor setting)
    {
        return setting.ValueType == SettingValueType.SecureString;
    }
}
