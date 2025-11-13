using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.EnvironmentsCompare.Core.Services;

public interface ISettingsCompareService
{
    Task CompareAsync(IList<string> environmentNames, string mainEnvironment = null);
}
