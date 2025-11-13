using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class ComparablePlatformSettingsProvider(ISettingsManager settingsManager) : IComparableSettingsProvider
{
    public async Task<ComparableSettingProviderResult> GetComparableSettingsAsync()
    {
        var result = AbstractTypeFactory<ComparableSettingProviderResult>.TryCreateInstance();
        result.Scope = "PlatformSettings";

        foreach (var group in settingsManager.AllRegisteredSettings
            .Where(x => !x.GroupName.IsNullOrEmpty())
            .GroupBy(x => x.GroupName))
        {
            var resultGroup = new ComparableSettingGroup();
            resultGroup.Name = group.Key;

            result.SettingGroups.Add(resultGroup);

            foreach (var setting in group)
            {
                resultGroup.Settings.Add(new ComparableSetting
                {
                    Name = setting.Name,
                    Value = await GetSettingValueAsync(setting),
                    IsSecret = IsSettingSecret(setting)
                });
            }
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

    protected virtual bool IsSettingSecret(SettingDescriptor setting)
    {
        return setting.ValueType == SettingValueType.SecureString || setting.Name.EqualsIgnoreCase("Shipping.Bopis.GoogleMaps.ApiKey");//TODO: implement some flag in SettingDescriptor
    }
}
