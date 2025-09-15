using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Enums;

namespace VemQueCabe.Domain.Interfaces;

/// <summary>
/// Interface for managing ride request data operations.
/// </summary>
public interface IRideRequestRepository
{
    /// <summary>
    /// Retrieves a ride request by its unique identifier.
    /// </summary>
    /// <param name="requestId">The unique identifier of the ride request.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ride request if found, otherwise null.</returns>
    Task<RideRequest?> GetRequestByIdAsync(int requestId);

    /// <summary>
    /// Retrieves all ride requests.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all ride requests.</returns>
    Task<IEnumerable<RideRequest>> GetAllRequestsAsync();

    /// <summary>
    /// Adds a new ride request to the repository.
    /// </summary>
    /// <param name="rideRequest">The ride request to add.</param>
    void AddRequest(RideRequest rideRequest);

    /// <summary>
    /// Updates an existing ride request in the repository.
    /// </summary>
    /// <param name="rideRequest">The ride request to update.</param>
    void UpdateRequest(RideRequest rideRequest);

    /// <summary>
    /// Deletes a ride request from the repository.
    /// </summary>
    /// <param name="rideRequest">The ride request to delete.</param>
    void DeleteRequest(RideRequest rideRequest);

    /// <summary>
    /// Retrieves all ride requests associated with a specific passenger.
    /// </summary>
    /// <param name="passengerId">The unique identifier of the passenger.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of ride requests for the specified passenger.</returns>
    Task<IEnumerable<RideRequest>> GetRequestsByPassengerIdAsync(int passengerId);

    /// <summary>
    /// Retrieves all ride requests with a specific status.
    /// </summary>
    /// <param name="status">The status of the ride requests to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of ride requests with the specified status.</returns>
    Task<IEnumerable<RideRequest>> GetRequestsByStatusAsync(Status status);
}
