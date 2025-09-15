namespace VemQueCabe.Domain.Shared.Extensions;

/// <summary>
/// Provides predefined error messages for ride request operations.
/// </summary>
public static class RideRequestErrors
{
    /// <summary>
    /// Returns an error indicating that the passenger already has an existing ride request.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance representing a "Bad Request" (400) status.</returns>
    public static Error PassengerHasExistingRequest() =>
        Error.BadRequest("Passenger already has an active ride request.");

    /// <summary>
    /// Returns an error indicating that the ride request with the specified ID was not found.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance representing a "Not Found" (404) status.</returns>
    public static Error RideRequestKeyNotFound() =>
        Error.NotFound("Ride request with ID not found.");

    /// <summary>
    /// Returns an error indicating that no ride requests were found.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance representing a "Not Found" (404) status.</returns>
    public static Error RideRequestNotFound() =>
        Error.NotFound("No ride requests found.");

    /// <summary>
    /// Returns an error indicating that only pending ride requests can be canceled.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance representing a "Bad Request" (400) status.</returns>
    public static Error CannotCancelNonPendingRequest() =>
        Error.BadRequest("Only pending ride requests can be canceled.");
    
    /// <summary>
    /// Returns an error indicating that an active ride request cannot be deleted.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance representing a "Bad Request" (400) status.</returns>
    public static Error CannotDeleteActiveRequest() =>
        Error.BadRequest("Cannot delete an active ride request.");
}
