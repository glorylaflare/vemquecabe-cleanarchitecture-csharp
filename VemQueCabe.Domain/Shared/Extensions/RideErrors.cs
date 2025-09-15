namespace VemQueCabe.Domain.Shared.Extensions;

/// <summary>
/// Provides predefined error messages related to rides.
/// </summary>
public static class RideErrors
{
    /// <summary>
    /// Returns an error indicating that a ride was not found.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a "Not Found" (404) status and a relevant message.</returns>
    public static Error RideNotFound() =>
        Error.NotFound("No rides found.");

    /// <summary>
    /// Returns an error indicating that a ride with the specified ID was not found.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a "Not Found" (404) status and a relevant message.</returns>
    public static Error RideKeyNotFound() =>
        Error.NotFound("Ride with ID not found.");

    /// <summary>
    /// Returns an error indicating that no rides were found for the specified driver.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a "Bad Request" (400) status and a relevant message.</returns>
    public static Error RideNotFoundForThisDriver() =>
        Error.NotFound("No rides found for this driver.");
}
