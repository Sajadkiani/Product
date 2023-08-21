using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Product.Api.Security;

public sealed class RequiredClaimsAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] claimNames;

    public RequiredClaimsAttribute(params string[] claimNames)
    {
        this.claimNames = claimNames;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context == null) return;
        
        if (!claimNames.All(claim => context.HttpContext.User.Claims.Any(clm => clm.Value == claim)))
        {
            context.Result = new UnauthorizedObjectResult(string.Empty);
        }
    }
}