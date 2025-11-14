using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparedEnvironmentSettingGroup
{
    public string Name { get; set; }

    public IList<ComparedEnvironmentSetting> Settings { get; set; } = new List<ComparedEnvironmentSetting>();
}
