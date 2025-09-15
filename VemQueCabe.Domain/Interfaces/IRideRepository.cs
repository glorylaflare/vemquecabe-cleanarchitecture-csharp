using VemQueCabe.Domain.Entities;

namespace VemQueCabe.Domain.Interfaces;

/// <summary>
/// Interface for managing ride-related data operations.
/// </summary>
public interface IRideRepository
{
    /// <summary>
    /// Retrieves a ride by its unique identifier.
    /// </summary>
    /// <param name="rideId">The unique identifier of the ride.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ride if found, otherwise null.</returns>
    Task<Ride?> GetRideByIdAsync(int rideId);

    /// <summary>
    /// Retrieves all rides in the system.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all rides.</returns>
    Task<IEnumerable<Ride>> GetAllRidesAsync();

    /// <summary>
    /// Creates a new ride in the system.
    /// </summary>
    /// <param name="ride">The ride entity to be created.</param>
    void CreateRide(Ride ride);

    /// <summary>
    /// Updates an existing ride in the system.
    /// </summary>
    /// <param name="ride">The ride entity with updated information.</param>
    void UpdateRide(Ride ride);

    /// <summary>
    /// Deletes a ride from the system.
    /// </summary>
    /// <param name="ride">The ride entity to be deleted.</param>
    void DeleteRide(Ride ride);

    /// <summary>
    /// Retrieves all rides associated with a specific driver.
    /// </summary>
    /// <param name="driverId">The unique identifier of the driver.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of rides associated with the driver.</returns>
    Task<IEnumerable<Ride>> GetRidesByDriverIdAsync(int driverId);
}
