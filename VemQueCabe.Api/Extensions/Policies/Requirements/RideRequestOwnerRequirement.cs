using Microsoft.AspNetCore.Authorization;

namespace VemQueCabe.Api.Extensions.Policies.Requirements;

/// <summary>
/// Authorization requirement to ensure the user is the owner of the ride request.
/// </summary>
public class RideRequestOwnerRequirement : IAuthorizationRequirement {}