using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class SettingsComparisonResult
{
    public IList<ComparedEnvironmentSettingScope> Settings { get; set; }
}
