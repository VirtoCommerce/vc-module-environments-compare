namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparedEnvironmentSettingValue
{
    public string EnvironmentName { get; set; }

    public object Value { get; set; }

    public bool EqualsBaseValue { get; set; }
}
