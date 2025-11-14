using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparableEnvironmentSettings
{
    public string EnvironmentName { get; set; }

    public IList<ComparableSettingScope> Settings { get; set; }

    public string ErrorMessage { get; set; }
}
