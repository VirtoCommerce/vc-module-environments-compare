using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.EnvironmentsCompare.Web.Filters;

public class AuthorizationByKeyAttribute : TypeFilterAttribute
{
    public AuthorizationByKeyAttribute() : base(typeof(AuthorizationByKeyFilter))
    {
    }
}
