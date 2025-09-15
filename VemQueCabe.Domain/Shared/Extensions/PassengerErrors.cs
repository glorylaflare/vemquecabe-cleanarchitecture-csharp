namespace VemQueCabe.Domain.Shared.Extensions;

/// <summary>
/// Provides predefined error messages related to passengers.
/// </summary>
public static class PassengerErrors
{
    /// <summary>
    /// Creates an error indicating that no passengers were found.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a "Not Found" (404) status and a relevant message.</returns>
    public static Error PassengerNotFound() =>
        Error.NotFound("Passengers not found.");

    /// <summary>
    /// Creates an error indicating that a passenger key (ID) was not found.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a "Not Found" (404) status and a relevant message.</returns>
    public static Error PassengerKeyNotFound() =>
        Error.NotFound("Passenger with ID not found.");
}
