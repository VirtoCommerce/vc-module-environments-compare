namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparableSetting
{
    public string Name { get; set; }
    public string Description { get; set; }

    public object Value { get; set; }

    public bool IsSecret { get; set; }

}
