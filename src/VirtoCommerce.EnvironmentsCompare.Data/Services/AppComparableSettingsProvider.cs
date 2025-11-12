using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class AppComparableSettingsProvider(IConfiguration configuration) : IComparableSettingsProvider
{
    public Task<ComparableSettingsGroup> GetComparableSettingsAsync()
    {
        var result = AbstractTypeFactory<ComparableSettingsGroup>.TryCreateInstance<ComparableSettingsGroup>();

        var connectionStringValue = configuration.GetValue<string>("ConnectionStrings.VirtoCommerce");
        result.Settings.Add(new ComparableSetting()
        {
            Scope = "Environment",
            Name = "ConnectionStrings.VirtoCommerce",
            Value = connectionStringValue,
            IsSecret = true,
        });

        return Task.FromResult(result);
    }
}
