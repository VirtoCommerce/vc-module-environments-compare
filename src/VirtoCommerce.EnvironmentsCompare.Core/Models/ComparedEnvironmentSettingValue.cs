namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparedEnvironmentSettingValue
{
    public string EnvironmentName { get; set; }

    public object Value { get; set; }

    public bool HasDifference { get; set; }
}
