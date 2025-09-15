using Microsoft.AspNetCore.Authorization;
using VemQueCabe.Api.Extensions.Policies.Requirements;
using VemQueCabe.Application.Extensions;
using VemQueCabe.Domain.Shared;

namespace VemQueCabe.Api.Extensions.Policies.Handlers;

/// <summary>
/// Authorization handler to ensure that the user is the owner of the ride they are trying to access or modify.
/// </summary>
public class RideOwnerHandler : AuthorizationHandler<RideOwnerRequirement>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RideOwnerHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Service to access the application's data repositories.</param>
    /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
    public RideOwnerHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }
    
    /// <summary>
    /// Handles the authorization requirement to ensure the user is the owner of the ride.
    /// </summary>
    /// <param name="context">Authorization handler context containing user and resource information.</param>
    /// <param name="requirement">The specific requirement to be checked for authorization.</param>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RideOwnerRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            context.Fail();
            return;
        }
        var id = httpContext.Request.RouteValues["id"]?.ToString();

        if (int.TryParse(id, out var rideId))
        {
            var ride = await _unitOfWork.Rides.GetRideByIdAsync(rideId);
            var userId = httpContext.User.GetUserId();
            
            if (ride != null && ride.DriverId == userId)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}