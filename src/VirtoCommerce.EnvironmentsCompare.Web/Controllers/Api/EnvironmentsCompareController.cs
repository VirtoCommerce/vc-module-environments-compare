using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VirtoCommerce.EnvironmentsCompare.Core;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.EnvironmentsCompare.Web.Models;
using VirtoCommerce.Platform.Core.Common;
using Permissions = VirtoCommerce.EnvironmentsCompare.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.EnvironmentsCompare.Web.Controllers.Api;

[Authorize]
[Route("api/environments-compare")]
public class EnvironmentsCompareController(IOptions<EnvironmentsCompareSettings> options, IEnvironmentsCompareService settingsCompareService) : Controller
{
    [HttpGet]
    [Route("get-environments")]
    [Authorize(Permissions.Read)]
    public ActionResult<IList<EnvironmentResponseItem>> GetEnvironments()
    {
        var result = new List<EnvironmentResponseItem>
        {
            new()
            {
                Url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}",
                Name = GetCurrentEnvironmentName(),
                IsCurrent = true
            }
        };

        if (options.Value != null && !options.Value.ComparableEnvironments.IsNullOrEmpty())
        {
            result.AddRange(options.Value.ComparableEnvironments.Select(x => new EnvironmentResponseItem() { Name = x.Name, Url = x.Url }));
        }

        return Ok(result);
    }

    [HttpPost]
    [Route("compare-environments")]
    [Authorize(Permissions.Read)]
    public async Task<ActionResult<SettingsComparisonResult>> CompareEnvironments([FromBody] CompareEnvironmentsRequest request)
    {
        if (request?.EnvironmentNames == null || request.EnvironmentNames.Count < 2)
        {
            return BadRequest("At least 2 environments are required for comparison.");
        }

        var result = await settingsCompareService.CompareAsync(request.EnvironmentNames, request.BaseEnvironmentName, request.ShowAll);
        return Ok(result);
    }

    [HttpGet]
    [Route("export-settings/{environmentName}")]
    [Authorize(Permissions.Read)]
    public async Task<ActionResult> ExportSettings(string environmentName)
    {
        if (environmentName.IsNullOrEmpty())
        {
            return BadRequest("Environment name is required for export.");
        }

        var environment = await settingsCompareService.GetComparableEnvironmentsAsync([environmentName]);

        var serializerOptions = new JsonSerializerOptions { WriteIndented = true };
        var resultJson = JsonSerializer.Serialize(environment, serializerOptions);
        var resultBytes = System.Text.Encoding.UTF8.GetBytes(resultJson);
        var resultFileName = $"{environmentName}-settings-{DateTime.UtcNow.ToString("yyyy-dd-M--HH-mm-ss")}.json";

        return File(resultBytes, "application/octet-stream", resultFileName);
    }

    private string GetCurrentEnvironmentName()
    {
        return options.Value?.CurrentEnvironmentName ??
            ModuleConstants.EnvironmentsCompare.CurrentEnvironmentName;
    }
}
