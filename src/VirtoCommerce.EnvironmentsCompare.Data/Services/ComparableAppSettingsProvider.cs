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

        var connectionStringsGroup = new ComparableSettingGroup();
        connectionStringsGroup.Name = "ConnectionStrings";

        result.SettingGroups.Add(connectionStringsGroup);

        var connectionStringValue = configuration.GetValue<string>("ConnectionStrings.VirtoCommerce");
        connectionStringsGroup.Settings.Add(new ComparableSetting()
        {
            Name = "ConnectionStrings.VirtoCommerce",
            Value = connectionStringValue,
            IsSecret = true,
        });

        return Task.FromResult(result);
    }
}
