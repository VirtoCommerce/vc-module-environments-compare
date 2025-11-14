using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparedEnvironmentSettingScope
{
    public string Name { get; set; }

    public IList<ComparedEnvironmentSettingGroup> SettingGroups { get; set; } = new List<ComparedEnvironmentSettingGroup>();
}
