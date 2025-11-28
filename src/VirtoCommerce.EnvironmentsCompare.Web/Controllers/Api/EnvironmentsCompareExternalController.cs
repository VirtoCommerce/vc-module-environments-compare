using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.EnvironmentsCompare.Core;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using static VirtoCommerce.EnvironmentsCompare.Core.ModuleConstants.Security;

namespace VirtoCommerce.EnvironmentsCompare.Web.Controllers.Api;


[Route(ModuleConstants.EnvironmentsCompare.ApiEnvironmentsCompareRoute)]
[Authorize]
public class EnvironmentsCompareExternalController(IComparableSettingsMasterProvider comparableSettingsMasterProvider) : Controller
{
    [HttpGet]
    [Route("")]
    [Authorize(Permissions.Access)]
    public async Task<ActionResult<IList<ComparableSettingScope>>> GetSettings()
    {
        var result = await comparableSettingsMasterProvider.GetAllComparableSettingsAsync();
        return Ok(result);
    }
}
