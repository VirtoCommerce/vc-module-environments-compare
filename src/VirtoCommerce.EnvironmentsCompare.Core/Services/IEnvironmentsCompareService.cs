using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core.Models;

namespace VirtoCommerce.EnvironmentsCompare.Core.Services;

public interface IEnvironmentsCompareService
{
    Task<SettingsComparisonResult> CompareAsync(IList<string> environmentNames, string baseEnvironmentName = null, bool showAll = false);

    Task<IList<ComparableEnvironmentSettings>> GetComparableEnvironmentsAsync(IList<string> environmentNames);
}
