using System.Collections.Generic;
using VirtoCommerce.EnvironmentsCompare.Core.Models;

namespace VirtoCommerce.EnvironmentsCompare.Core.Services;

public interface IEnvironmentsCompareSettingsService
{
    string SelfApiKey { get; }
    IList<ComparableEnvironment> ComparableEnvironments { get; }
}
