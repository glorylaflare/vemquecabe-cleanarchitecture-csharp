using Microsoft.AspNetCore.Authorization;

namespace VemQueCabe.Api.Extensions.Policies.Requirements;

/// <summary>
/// Authorization requirement to ensure the user is the owner of the ride.
/// </summary>
public class RideOwnerRequirement : IAuthorizationRequirement {}