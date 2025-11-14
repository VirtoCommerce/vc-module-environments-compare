using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using Permissions = VirtoCommerce.EnvironmentsCompare.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.EnvironmentsCompare.Web.Controllers.Api;

[Authorize]
[Route("api/environments-compare")]
public class EnvironmentsCompareController(IEnvironmentsCompareSettingsService settingsService, ISettingsCompareService settingsCompareService) : Controller
{
    [HttpGet]
    [Route("get-environments")]
    [Authorize(Permissions.Read)]
    public ActionResult<IList<string>> GetEnvironments()
    {
        var result = settingsService.ComparableEnvironments?.Select(x => x.Name)?.ToList();
        return Ok(result);
    }

    [HttpPost]
    [Route("compare-environments")]
    [Authorize(Permissions.Read)]
    public async Task<ActionResult<SettingsComparisonResult>> CompareEnvironments(IList<string> environmentNames, string baseEnvironmentName = null)
    {
        var result = await settingsCompareService.CompareAsync(environmentNames, baseEnvironmentName);
        return Ok(result);
    }
}
