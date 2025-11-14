using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VirtoCommerce.EnvironmentsCompare.Core;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EnvironmentsCompare.Web.Filters;

public class AuthorizationByKeyFilter(IEnvironmentsCompareSettingsService settingsService) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var requestApiKeyHash = context.HttpContext.Request.Headers.FirstOrDefault(x => x.Key.EqualsIgnoreCase(ModuleConstants.EnvironmentsCompare.ApiKeyHeaderName)).Value.FirstOrDefault();

        if (requestApiKeyHash.IsNullOrEmpty())
        {
            context.Result = new StatusCodeResult(403);
            return;
        }

        if (requestApiKeyHash != settingsService.SelfApiKey.GetSHA1Hash())
        {
            context.Result = new StatusCodeResult(403);
        }
    }
}
