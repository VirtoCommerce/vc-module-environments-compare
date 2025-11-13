using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EnvironmentsCompare.Web.Filters;

public class AuthorizationByKeyFilter(IConfiguration configuration) : IAuthorizationFilter
{
    public const string ApiKeyHeaderName = "api-key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var requestApiKey = context.HttpContext.Request.Headers.FirstOrDefault(x => x.Key.EqualsIgnoreCase(ApiKeyHeaderName)).Value.FirstOrDefault();

        if (requestApiKey.IsNullOrEmpty())
        {
            context.Result = new StatusCodeResult(403);
            return;
        }

        var expectedApiKey = configuration["Authorization:ApiKey"];

        if (requestApiKey != expectedApiKey)
        {
            context.Result = new StatusCodeResult(403);
        }
    }
}
