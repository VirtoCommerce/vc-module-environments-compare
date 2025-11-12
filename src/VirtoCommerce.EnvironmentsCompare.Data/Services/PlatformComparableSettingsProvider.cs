using System;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class PlatformComparableSettingsProvider(ISettingsManager settingsManager) : IComparableSettingsProvider
{
    public async Task<ComparableSettingsGroup> GetComparableSettingsAsync()
    {
        var result = AbstractTypeFactory<ComparableSettingsGroup>.TryCreateInstance<ComparableSettingsGroup>();

        foreach (var setting in settingsManager.AllRegisteredSettings)
        {
            result.Settings.Add(new ComparableSetting
            {
                Scope = "Platform",
                Name = setting.Name,
                Value = await GetSettingValueAsync(setting),
                IsSecret = setting.ValueType == SettingValueType.SecureString
            });
        }

        return result;
    }

    protected async Task<object> GetSettingValueAsync(SettingDescriptor settingDescriptor)
    {
        if (settingDescriptor.ValueType == SettingValueType.Boolean)
        {
            return await settingsManager.GetValueAsync<bool>(settingDescriptor);
        }

        if (settingDescriptor.ValueType == SettingValueType.Integer || settingDescriptor.ValueType == SettingValueType.PositiveInteger)
        {
            return await settingsManager.GetValueAsync<int>(settingDescriptor);
        }

        if (settingDescriptor.ValueType == SettingValueType.Decimal)
        {
            return await settingsManager.GetValueAsync<decimal>(settingDescriptor);
        }

        if (settingDescriptor.ValueType == SettingValueType.DateTime)
        {
            return await settingsManager.GetValueAsync<DateTime>(settingDescriptor);
        }

        if (settingDescriptor.ValueType == SettingValueType.Json)
        {
            return await settingsManager.GetValueAsync<string>(settingDescriptor);
        }

        if ((settingDescriptor.ValueType == SettingValueType.SecureString || settingDescriptor.ValueType == SettingValueType.ShortText || settingDescriptor.ValueType == SettingValueType.LongText))
        {
            return await settingsManager.GetValueAsync<string>(settingDescriptor);
        }

        return null;//TODO: handle other types
    }
}
