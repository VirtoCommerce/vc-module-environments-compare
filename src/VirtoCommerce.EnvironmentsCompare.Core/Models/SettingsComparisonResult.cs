using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class SettingsComparisonResult
{
    public IList<ComparedEnvironment> ComparedEnvironments { get; set; } = [];

    public IList<ComparedEnvironmentSettingScope> SettingScopes { get; set; } = [];
}
