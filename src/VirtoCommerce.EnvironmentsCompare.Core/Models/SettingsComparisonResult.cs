using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class SettingsComparisonResult
{
    public IList<ComparedEnvironmentSettingScope> SettingScopes { get; set; } = new List<ComparedEnvironmentSettingScope>();
}
