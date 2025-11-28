using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class EnvironmentsCompareSettingsService(IConfiguration configuration) : IEnvironmentsCompareSettingsService
{
    public IList<ComparableEnvironment> ComparableEnvironments => configuration.GetSection("EnvironmentsCompare:ComparableEnvironments")?.Get<List<ComparableEnvironment>>() ?? [];
}
