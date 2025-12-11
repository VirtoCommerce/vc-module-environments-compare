using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;
public class EnvironmentsCompareSettings
{
    public string CurrentEnvironmentName { get; set; } = ModuleConstants.EnvironmentsCompare.CurrentEnvironmentName;

    public IList<ComparableEnvironment> ComparableEnvironments { get; set; } = [];

    public WhiteListSettings WhiteList { get; set; } = new WhiteListSettings();
}
