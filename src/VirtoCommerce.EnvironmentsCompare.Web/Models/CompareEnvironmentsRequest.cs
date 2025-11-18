using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Web.Models;

public class CompareEnvironmentsRequest
{
    public IList<string> EnvironmentNames { get; set; }
    public string BaseEnvironmentName { get; set; }
    public bool ShowAll { get; set; }
}
