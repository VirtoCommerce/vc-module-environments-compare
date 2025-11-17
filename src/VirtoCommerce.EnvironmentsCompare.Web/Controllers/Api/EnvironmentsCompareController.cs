using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.EnvironmentsCompare.Core;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.EnvironmentsCompare.Web.Models;
using Permissions = VirtoCommerce.EnvironmentsCompare.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.EnvironmentsCompare.Web.Controllers.Api;

[Authorize]
[Route("api/environments-compare")]
public class EnvironmentsCompareController(IEnvironmentsCompareSettingsService settingsService, ISettingsCompareService settingsCompareService) : Controller
{
    [HttpGet]
    [Route("get-environments")]
    [Authorize(Permissions.Access)]
    public ActionResult<IList<EnvironmentResponseItem>> GetEnvironments()
    {
        var result = settingsService.ComparableEnvironments?.Select(x => new EnvironmentResponseItem() { Name = x.Name, Url = x.Url })?.ToList();
        result.Insert(0, new EnvironmentResponseItem() { Name = ModuleConstants.EnvironmentsCompare.CurrentEnvironmentName });
        return Ok(result);
    }

    [HttpPost]
    [Route("compare-environments")]
    [Authorize(Permissions.Access)]
    public async Task<ActionResult<SettingsComparisonResult>> CompareEnvironments([FromBody] CompareEnvironmentsRequest request)
    {
        if (request?.EnvironmentNames == null || request.EnvironmentNames.Count < 2)
        {
            return BadRequest("At least 2 environments are required for comparison.");
        }

        var result = await settingsCompareService.CompareAsync(request.EnvironmentNames, request.BaseEnvironmentName);
        return Ok(result);
    }
}
