using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Enums;
using VemQueCabe.Domain.Shared;

namespace VemQueCabe.Application.Interfaces;

/// <summary>
/// Service interface for managing ride requests.
/// </summary>
public interface IRideRequestService
{
    /// <summary>
    /// Creates a new ride request.
    /// </summary>
    /// <param name="dto">The data transfer object containing ride request details.</param>
    /// <returns>A result containing the created ride request or an error.</returns>
    Task<Result<ResponseRideRequest>> CreateRideRequest(CreateRideRequestDto dto);

    /// <summary>
    /// Retrieves all ride requests.
    /// </summary>
    /// <returns>A result containing a collection of ride request response DTOs or an error.</returns>
    Task<Result<IEnumerable<ResponseRideRequest>>> GetAllRideRequests();

    /// <summary>
    /// Retrieves a specific ride request by its ID.
    /// </summary>
    /// <param name="id">The ID of the ride request.</param>
    /// <returns>A result containing the ride request response DTO or an error.</returns>
    Task<Result<ResponseRideRequest>> GetRideRequestById(int id);

    /// <summary>
    /// Retrieves all ride requests associated with a specific passenger ID.
    /// </summary>
    /// <param name="id">The ID of the passenger.</param>
    /// <returns>A result containing a collection of ride request response DTOs or an error.</returns>
    Task<Result<IEnumerable<ResponseRideRequest>>> GetRequestsByPassengerId(int id);

    /// <summary>
    /// Retrieves all ride requests with a specific status.
    /// </summary>
    /// <param name="status">The status of the ride requests to retrieve.</param>
    /// <returns>A result containing a collection of ride request response DTOs or an error.</returns>
    Task<Result<IEnumerable<ResponseRideRequest>>> GetRequestsByStatus(Status status);

    /// <summary>
    /// Updates the status of a specific ride request.
    /// </summary>
    /// <param name="id">The ID of the ride request to update.</param>
    /// <param name="status">The new status to set for the ride request.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> UpdateRideRequestStatus(int id, Status status);

    /// <summary>
    /// Cancels a specific ride request.
    /// </summary>
    /// <param name="id">The ID of the ride request to cancel.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> CancelRideRequest(int id);

    /// <summary>
    /// Deletes a specific ride request.
    /// </summary>
    /// <param name="id">The ID of the ride request to delete.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> DeleteRideRequest(int id);
}
