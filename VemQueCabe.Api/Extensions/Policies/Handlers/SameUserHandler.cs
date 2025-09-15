using Microsoft.AspNetCore.Authorization;
using VemQueCabe.Api.Extensions.Policies.Requirements;
using VemQueCabe.Application.Extensions;

namespace VemQueCabe.Api.Extensions.Policies.Handlers;

/// <summary>
/// Authorization handler to ensure the user is the same as the resource owner.
/// </summary>
public class SameUserHandler : AuthorizationHandler<SameUserRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SameUserHandler"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public SameUserHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    /// <summary>
    /// Handles the authorization requirement to ensure the user is the same as the resource owner.
    /// </summary>
    /// <param name="context">The authorization context containing information about the user and the resource.</param>
    /// <param name="requirement">The authorization requirement being evaluated.</param>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        var userIdClaim = httpContext.User.GetUserId();
        if (userIdClaim == null)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        if (!int.TryParse(httpContext.Request.RouteValues["id"]?.ToString(), out var userId))
            context.Fail();
        
        if (userIdClaim == userId)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}