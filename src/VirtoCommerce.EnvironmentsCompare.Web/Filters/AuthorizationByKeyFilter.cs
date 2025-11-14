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
        var requestApiKey = context.HttpContext.Request.Headers.FirstOrDefault(x => x.Key.EqualsIgnoreCase(ModuleConstants.EnvironmentsCompare.ApiKeyHeaderName)).Value.FirstOrDefault();

        if (requestApiKey.IsNullOrEmpty())
        {
            context.Result = new StatusCodeResult(403);
            return;
        }

        if (requestApiKey.GetSHA1Hash() != settingsService.SelfApiKey.GetSHA1Hash())
        {
            context.Result = new StatusCodeResult(403);
        }
    }
}
