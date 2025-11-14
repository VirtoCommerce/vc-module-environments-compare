using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparableSettingScope
{
    /// <summary>
    /// Scope of the settings, e.g. "Platform", "AppSettings"
    /// </summary>
    public string ScopeName { get; set; }

    public string ProviderName { get; set; }

    public string ErrorMessage { get; set; }

    public IList<ComparableSettingGroup> SettingGroups { get; set; } = new List<ComparableSettingGroup>();
}
