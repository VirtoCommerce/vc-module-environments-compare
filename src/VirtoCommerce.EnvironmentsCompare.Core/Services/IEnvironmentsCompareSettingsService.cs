using System.Collections.Generic;
using VirtoCommerce.EnvironmentsCompare.Core.Models;

namespace VirtoCommerce.EnvironmentsCompare.Core.Services;

public interface IEnvironmentsCompareSettingsService
{
    IList<ComparableEnvironment> ComparableEnvironments { get; }
}
