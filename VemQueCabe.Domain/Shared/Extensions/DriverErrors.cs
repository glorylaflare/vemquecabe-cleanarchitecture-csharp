namespace VemQueCabe.Domain.Shared.Extensions;

/// <summary>
/// Provides predefined error messages related to driver operations.
/// </summary>
public static class DriverErrors
{
    /// <summary>
    /// Creates an error indicating that a driver was not found.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a "Not Found" status and a relevant message.</returns>
    public static Error DriverNotFound() =>
        Error.NotFound("Driver not found.");

    /// <summary>
    /// Creates an error indicating that a driver with the specified ID was not found.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a "Not Found" status and a relevant message.</returns>
    public static Error DriverKeyNotFound() =>
        Error.NotFound("Driver with ID not found.");

    /// <summary>
    /// Creates an error indicating that a driver with the same vehicle plate already exists.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a "Conflict" status and a relevant message.</returns>
    public static Error DriverAlreadyExists() =>
        Error.Conflict("An error occurred while trying to register the driver.");

    /// <summary>
    /// Creates an error indicating that the driver is not available.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a "Not Found" status and a relevant message.</returns>
    public static Error DriverNotAvailable() =>
        Error.NotFound("The driver is not available.");

    /// <summary>
    /// Creates an error indicating that no available drivers were found.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance with a "Not Found" status and a relevant message.</returns>
    public static Error DriversNoAvailableFound() =>
        Error.NotFound("No available drivers found.");
}
