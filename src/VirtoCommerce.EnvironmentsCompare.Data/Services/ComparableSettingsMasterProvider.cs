using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class ComparableSettingsMasterProvider(IEnumerable<IComparableSettingsProvider> comparableSettingsProviders) : IComparableSettingsMasterProvider
{
    public async Task<IList<ComparableSettingsGroup>> GetAllComparableSettingsAsync()
    {
        var result = new List<ComparableSettingsGroup>();

        foreach (var provider in comparableSettingsProviders)
        {
            result.Add(await GetSettingsFromProviderAsync(provider));
        }

        foreach (var settingsGroup in result)
        {
            HideSecretSettings(settingsGroup);
        }

        return result;
    }

    protected async Task<ComparableSettingsGroup> GetSettingsFromProviderAsync(IComparableSettingsProvider comparableSettingsProvider)
    {
        ComparableSettingsGroup result;

        try
        {
            result = await comparableSettingsProvider.GetComparableSettingsAsync();

            if (result == null)
            {
                throw new InvalidOperationException("The provider returned null settings group");
            }
        }
        catch (Exception ex)
        {
            result = AbstractTypeFactory<ComparableSettingsGroup>.TryCreateInstance<ComparableSettingsGroup>();
            result.ErrorMessage = ex.Message;
        }

        result.ProviderName = comparableSettingsProvider.GetType().FullName;

        return result;
    }

    protected virtual void HideSecretSettings(ComparableSettingsGroup settingsGroup)
    {
        foreach (var setting in settingsGroup.Settings.Where(x => x.IsSecret))
        {
            setting.Value = setting.Value?.GetSHA1Hash();
        }
    }
}
