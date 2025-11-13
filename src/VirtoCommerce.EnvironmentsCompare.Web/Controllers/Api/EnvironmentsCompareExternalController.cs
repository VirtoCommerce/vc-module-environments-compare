using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.EnvironmentsCompare.Web.Filters;

namespace VirtoCommerce.EnvironmentsCompare.Web.Controllers.Api;

[AuthorizationByKey]
[Route("api/environments-compare-external")]
public class EnvironmentsCompareExternalController(IComparableSettingsMasterProvider comparableSettingsMasterProvider) : Controller
{
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<IList<ComparableSetting>>> GetSettings()
    {
        return Ok(await comparableSettingsMasterProvider.GetAllComparableSettingsAsync());
    }
}
