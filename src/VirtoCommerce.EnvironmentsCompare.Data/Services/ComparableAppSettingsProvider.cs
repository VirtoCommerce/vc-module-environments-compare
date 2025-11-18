using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class ComparableAppSettingsProvider(IConfiguration configuration) : IComparableSettingsProvider
{
    public Task<IList<ComparableSettingScope>> GetComparableSettingsAsync()
    {
        var result = AbstractTypeFactory<ComparableSettingScope>.TryCreateInstance();
        result.ScopeName = "AppSettings";

        var connectionStringsGroup = AbstractTypeFactory<ComparableSettingGroup>.TryCreateInstance();
        connectionStringsGroup.GroupName = "ConnectionStrings";
        result.SettingGroups.Add(connectionStringsGroup);

        var connectionStringSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
        connectionStringSetting.Name = "ConnectionStrings.VirtoCommerce";
        connectionStringSetting.Value = configuration.GetValue<string>("ConnectionStrings:VirtoCommerce");
        connectionStringSetting.IsSecret = true;
        connectionStringsGroup.Settings.Add(connectionStringSetting);

        var virtoCommerceGroup = AbstractTypeFactory<ComparableSettingGroup>.TryCreateInstance();
        virtoCommerceGroup.GroupName = "VirtoCommerce";
        result.SettingGroups.Add(virtoCommerceGroup);

        var licenseActivationUrlSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
        licenseActivationUrlSetting.Name = "VirtoCommerce.LicenseActivationUrl";
        licenseActivationUrlSetting.Value = configuration.GetValue<string>("VirtoCommerce:LicenseActivationUrl");
        licenseActivationUrlSetting.IsSecret = false;
        virtoCommerceGroup.Settings.Add(licenseActivationUrlSetting);

        return Task.FromResult((IList<ComparableSettingScope>)[result]);
    }
}
