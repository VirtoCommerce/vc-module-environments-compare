using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparableSettingProviderResult
{
    /// <summary>
    /// Scope of the settings, e.g. "Platform", "AppSettings"
    /// </summary>
    public string Scope { get; set; }

    public string ProviderName { get; set; }

    public string ErrorMessage { get; set; }

    public IList<ComparableSettingGroup> SettingGroups { get; set; } = new List<ComparableSettingGroup>();
}
