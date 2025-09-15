namespace VemQueCabe.Domain.Shared.Extensions;

/// <summary>
/// Provides predefined authentication-related errors.
/// </summary>
public static class AuthErrors
{
    /// <summary>
    /// Creates an error representing invalid credentials provided by the user.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a 401 status code and a message indicating invalid credentials.</returns>
    public static Error InvalidCredentials() =>
        Error.Unauthorized("Invalid credentials provided.");
}
