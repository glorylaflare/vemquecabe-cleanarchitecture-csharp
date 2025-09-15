using VemQueCabe.Application.Requests;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Shared;

namespace VemQueCabe.Application.Interfaces;

/// <summary>
/// Defines the contract for authentication services.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user based on the provided login request data.
    /// </summary>
    /// <param name="login">The login request data containing email and password.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a <see cref="ResponseAuth"/> if authentication is successful,
    /// or an error if authentication fails.
    /// </returns>
    Task<Result<ResponseAuth>> AuthenticateAsync(RequestLogin login);
    
    /// <summary>
    /// Refreshes the authentication token using the provided refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token used to obtain a new authentication token.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a <see cref="ResponseAuth"/> with the new token if successful,
    /// or an error if the refresh token is invalid or expired.
    /// </returns>
    Task<Result<ResponseAuth>> RefreshTokenAsync(string refreshToken);
}
