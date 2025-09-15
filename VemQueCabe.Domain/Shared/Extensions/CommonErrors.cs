namespace VemQueCabe.Domain.Shared.Extensions;

/// <summary>
/// Provides common error instances for various scenarios.
/// </summary>
public static class CommonErrors
{
    /// <summary>
    /// Creates an error representing a failure to commit changes to the database.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a 400 status code and a descriptive message.</returns>
    public static Error CommitedFailed() =>
        Error.BadRequest("Failed to commit changes to the database.");

    /// <summary>
    /// Creates an error representing an invalid operation with a custom message.
    /// </summary>
    /// <param name="message">The error message describing the invalid operation.</param>
    /// <returns>An <see cref="Error"/> instance with a 400 status code and the provided message.</returns>
    public static Error InvalidOperation(string message) =>
        Error.BadRequest(message);

    /// <summary>
    /// Creates an error representing forbidden access to a resource.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a 403 status code and a descriptive message.</returns>
    public static Error ForbiddenAccess() =>
        Error.Forbidden("You are not authorized to access this resource.");
}
