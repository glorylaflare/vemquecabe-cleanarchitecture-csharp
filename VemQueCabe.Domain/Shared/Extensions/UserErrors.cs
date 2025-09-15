namespace VemQueCabe.Domain.Shared.Extensions;

/// <summary>
/// Provides predefined user-related error messages.
/// </summary>
public static class UserErrors
{
    /// <summary>
    /// Returns an error indicating that the user was not found.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a 404 status code and a "User not found." message.</returns>
    public static Error UserNotFound() =>
        Error.NotFound("User not found.");
    
    /// <summary>
    /// Returns an error indicating that the user already exists.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a 409 status code and an appropriate message.</returns>
    public static Error UserAlreadyExists() =>
        Error.Conflict("An error occurred while trying to create the user.");
    
    /// <summary>
    /// Returns an error indicating that the user key (ID) was not found.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a 404 status code and a "User with ID not found." message.</returns>
    public static Error UserKeyNotFound() =>
        Error.NotFound("User with ID not found.");

    /// <summary>
    /// Returns an error indicating that the email is already in use.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a 409 status code and a "It wasn't possible to update the user." message.</returns>
    public static Error EmailAlreadyInUse() =>
        Error.Conflict("It wasn't possible to update the user.");
}
