namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparedEnvironment
{
    public bool IsComparisonBase { get; set; }

    public bool IsCurrent { get; set; }

    public string EnvironmentName { get; set; }

    public string ErrorMessage { get; set; }
}
