using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparableEnvironmentSettings
{
    public bool IsCurrent { get; set; }

    public string EnvironmentName { get; set; }

    public IList<ComparableSettingScope> SettingScopes { get; set; } = [];

    public string ErrorMessage { get; set; }
}
