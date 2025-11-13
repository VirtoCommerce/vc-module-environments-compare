using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class ComparableAppSettingsProvider(IConfiguration configuration) : IComparableSettingsProvider
{
    public Task<ComparableSettingProviderResult> GetComparableSettingsAsync()
    {
        var result = AbstractTypeFactory<ComparableSettingProviderResult>.TryCreateInstance();
        result.Scope = "AppSettings";

        var connectionStringsGroup = AbstractTypeFactory<ComparableSettingGroup>.TryCreateInstance();
        connectionStringsGroup.Name = "ConnectionStrings";
        result.SettingGroups.Add(connectionStringsGroup);

        var connectionStringSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
        connectionStringSetting.Name = "ConnectionStrings.VirtoCommerce";
        connectionStringSetting.Value = configuration.GetValue<string>("ConnectionStrings.VirtoCommerce");
        connectionStringSetting.IsSecret = true;
        connectionStringsGroup.Settings.Add(connectionStringSetting);

        return Task.FromResult(result);
    }
}
