using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparedEnvironmentSetting
{
    public string Name { get; set; }

    public IList<ComparedEnvironmentSettingValue> ComparedValues { get; set; } = [];
}
