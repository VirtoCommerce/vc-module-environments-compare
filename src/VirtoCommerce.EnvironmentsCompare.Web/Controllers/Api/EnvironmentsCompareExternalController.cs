using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.EnvironmentsCompare.Core;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.EnvironmentsCompare.Web.Filters;

namespace VirtoCommerce.EnvironmentsCompare.Web.Controllers.Api;

[AuthorizationByKey]
[Route(ModuleConstants.EnvironmentsCompare.SettingsCompareRoute)]
public class EnvironmentsCompareExternalController(IComparableSettingsMasterProvider comparableSettingsMasterProvider) : Controller
{
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<IList<ComparableSettingScope>>> GetSettings()
    {
        var result = await comparableSettingsMasterProvider.GetAllComparableSettingsAsync();
        return Ok(result);
    }
}
