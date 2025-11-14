using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparableEnvironmentSettings
{
    public string EnvironmentName { get; set; }

    public IList<ComparableSettingScope> SettingScopes { get; set; } = new List<ComparableSettingScope>();

    public string ErrorMessage { get; set; }
}
