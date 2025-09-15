using System.Security.Claims;

namespace VemQueCabe.Application.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="ClaimsPrincipal"/> class to retrieve user-specific information.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return null;
        }
        return userId;
    }
}
