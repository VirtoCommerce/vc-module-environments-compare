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
                resultSetting.Value = await settingsManager.GetValueAsync<object>(setting);
                resultSetting.IsSecret = IsSettingSecret(setting);
                resultGroup.Settings.Add(resultSetting);
            }
        }

        return [result];
    }

    protected virtual bool IsSettingSecret(SettingDescriptor setting)
    {
        return setting.ValueType == SettingValueType.SecureString;
    }
}
