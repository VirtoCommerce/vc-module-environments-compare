using System.Collections.Generic;

namespace VirtoCommerce.EnvironmentsCompare.Core.Models;

public class ComparableSettingsGroup
{
    public string ProviderName { get; set; }

    public IList<ComparableSetting> Settings { get; set; } = new List<ComparableSetting>();

    public string ErrorMessage { get; set; }
}
