using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.EnvironmentsCompare.Core;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;

namespace VirtoCommerce.EnvironmentsCompare.Web.Controllers.Api;

[Authorize]
[Route("api/environments-compare-external")]
public class EnvironmentsCompareExternalController(IComparableSettingsMasterProvider comparableSettingsMasterProvider) : Controller
{
    [HttpGet]
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<IList<ComparableSetting>>> GetSettings()
    {
        return Ok(await comparableSettingsMasterProvider.GetAllComparableSettingsAsync());
    }
}
