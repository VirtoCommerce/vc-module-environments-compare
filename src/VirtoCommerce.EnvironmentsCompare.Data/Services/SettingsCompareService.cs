using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core.Services;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class SettingsCompareService(IEnvironmentsCompareSettingsService settingsService, IEnvironmentsCompareClient environmentsCompareClient) : ISettingsCompareService
{
    public async Task CompareAsync(IList<string> environmentNames, string mainEnvironment = null)
    {
        var comparableEnvironments = settingsService.ComparableEnvironments.Where(x => environmentNames.Contains(x.Name)).ToList();
        var comparableEnvironmentSettings = await environmentsCompareClient.GetSettingsAsync(comparableEnvironments);

        //TODO: implement comparison logic
    }
}
