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
    public async Task<IList<ComparableSettingProviderResult>> GetAllComparableSettingsAsync()
    {
        var result = new List<ComparableSettingProviderResult>();

        foreach (var provider in comparableSettingsProviders)
        {
            result.Add(await GetSettingsFromProviderAsync(provider));
        }

        HideSecretSettings(result.SelectMany(x => x.SettingGroups).SelectMany(x => x.Settings));

        return result;
    }

    protected async Task<ComparableSettingProviderResult> GetSettingsFromProviderAsync(IComparableSettingsProvider comparableSettingsProvider)
    {
        ComparableSettingProviderResult result;

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
            result = AbstractTypeFactory<ComparableSettingProviderResult>.TryCreateInstance();
            result.ErrorMessage = ex.Message;
        }

        result.ProviderName = comparableSettingsProvider.GetType().FullName;

        return result;
    }

    protected virtual void HideSecretSettings(IEnumerable<ComparableSetting> settings)
    {
        foreach (var setting in settings.Where(x => x.IsSecret))
        {
            setting.Value = $"HASH: {setting.Value?.GetSHA1Hash()}";
        }
    }
}
