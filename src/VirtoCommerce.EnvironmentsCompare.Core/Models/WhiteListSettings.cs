using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class WhiteListSettings
{
    public WhiteListSetting SectionKeys { get; set; } = new();
    public WhiteListSetting SettingKeys { get; set; } = new();
}

public class WhiteListSetting
{
    public IList<string> Include { get; set; } = [];
    public IList<string> Exclude { get; set; } = [];
}
