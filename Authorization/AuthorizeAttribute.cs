namespace WebApi.Authorization;

using Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System.Linq;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public AuthorizeAttribute(params Role[] roles)
    {
        Roles = roles;
    }

    protected Role[] Roles { get; set; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // skip authorization if action is decorated with [AllowAnonymous] attribute
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        // authorization
        var user = (User?)context.HttpContext.Items["User"];
        if (user == null)
        {
            // not logged in or role not authorized
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }
        
        if (context.ActionDescriptor.EndpointMetadata
            .OfType<AuthorizeAttribute>()
            .Any(r => r.Roles.Any(role => role == user.Role))) return;
        
        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
    }
}