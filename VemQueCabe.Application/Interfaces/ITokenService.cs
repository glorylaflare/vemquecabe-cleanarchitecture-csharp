using VemQueCabe.Domain.Entities;

namespace VemQueCabe.Application.Interfaces;

/// <summary>
/// Provides methods for generating authentication tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <returns>A string representing the generated JWT token.</returns>
    string GenerateToken(User user);

    /// <summary>
    /// Generates a new refresh token.
    /// </summary>
    /// <returns>A string representing the generated refresh token.</returns>
    string GenerateRefreshToken();
}
