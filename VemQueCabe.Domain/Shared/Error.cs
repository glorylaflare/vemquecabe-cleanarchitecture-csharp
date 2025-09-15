namespace VemQueCabe.Domain.Shared;

/// <summary>
/// Represents an error with a specific code and message.
/// </summary>
/// <param name="Code">The HTTP status code associated with the error.</param>
/// <param name="Message">The error message describing the issue.</param>
public sealed record  Error(int Code, string Message)
{
    /// <summary>
    /// Creates an error representing a "Not Found" (404) status.
    /// </summary>
    /// <param name="message">The error message describing the issue.</param>
    /// <returns>An <see cref="Error"/> instance with a 404 status code.</returns>
    public static Error NotFound(string message) => new(404, message);

    /// <summary>
    /// Creates an error representing a "Bad Request" (400) status.
    /// </summary>
    /// <param name="message">The error message describing the issue.</param>
    /// <returns>An <see cref="Error"/> instance with a 400 status code.</returns>
    public static Error BadRequest(string message) => new(400, message);

    /// <summary>
    /// Creates an error representing a "Conflict" (409) status.
    /// </summary>
    /// <param name="message">The error message describing the issue.</param>
    /// <returns>An <see cref="Error"/> instance with a 409 status code.</returns>
    public static Error Conflict(string message) => new(409, message);
    
    /// <summary>
    /// Creates an error representing an "Unauthorized" (401) status.
    /// </summary>
    /// <param name="message">The error message describing the issue.</param>
    /// <returns>An <see cref="Error"/> instance with a 401 status code.</returns>
    public static Error Unauthorized(string message) => new(401, message);

    /// <summary>
    /// Creates an error representing a "Forbidden" (403) status.
    /// </summary>
    /// <param name="message">The error message describing the issue.</param>
    /// <returns>An <see cref="Error"/> instance with a 403 status code.</returns>
    public static Error Forbidden(string message) => new(403, message);
}
