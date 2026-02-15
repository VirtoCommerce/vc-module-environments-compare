using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core.Models;

namespace VirtoCommerce.EnvironmentsCompare.Core.Services;

public interface IComparableSettingsMasterProvider
{
    Task<IList<ComparableSettingScope>> GetAllComparableSettingsAsync();
}
