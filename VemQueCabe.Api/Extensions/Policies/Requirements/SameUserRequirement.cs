using Microsoft.AspNetCore.Authorization;

namespace VemQueCabe.Api.Extensions.Policies.Requirements;

/// <summary>
/// Authorization requirement to ensure the user is the same as the resource owner.
/// </summary>
public class SameUserRequirement : IAuthorizationRequirement {}