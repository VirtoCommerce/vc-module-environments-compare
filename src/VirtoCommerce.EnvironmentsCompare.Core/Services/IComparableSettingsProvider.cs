using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core.Models;

namespace VirtoCommerce.EnvironmentsCompare.Core.Services;

public interface IComparableSettingsProvider
{
    Task<IList<ComparableSettingScope>> GetComparableSettingsAsync();
}
