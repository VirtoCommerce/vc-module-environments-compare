namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparableSetting
{
    /// <summary>
    /// Setting scope (e.g., "Platform", "Store")
    /// </summary>
    public string Scope { get; set; }

    public string StoreId { get; set; }

    public string Name { get; set; }

    public object Value { get; set; }

    public bool IsSecret { get; set; }
}
