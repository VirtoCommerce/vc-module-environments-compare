using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparableSettingGroup
{
    public string GroupName { get; set; }

    public IList<ComparableSetting> Settings { get; set; } = new List<ComparableSetting>();
}
