using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Shared;

namespace VemQueCabe.Application.Interfaces;

/// <summary>
/// Interface for managing ride-related operations.
/// </summary>
public interface IRideService
{
    /// <summary>
    /// Creates a new ride based on the provided data.
    /// </summary>
    /// <param name="dto">The data transfer object containing ride creation details.</param>
    /// <returns>A result containing the created ride or an error.</returns>
    Task<Result<ResponseRide>> CreateRide(CreateRideDto dto);

    /// <summary>
    /// Retrieves all rides in the system.
    /// </summary>
    /// <returns>A result containing a collection of ride response DTOs or an error.</returns>
    Task<Result<IEnumerable<ResponseRide>>> GetAllRides();

    /// <summary>
    /// Retrieves a specific ride by its ID.
    /// </summary>
    /// <param name="id">The ID of the ride to retrieve.</param>
    /// <returns>A result containing the ride response DTO or an error.</returns>
    Task<Result<ResponseRide>> GetRideById(int id);

    /// <summary>
    /// Retrieves all rides associated with a specific driver.
    /// </summary>
    /// <param name="id">The ID of the driver.</param>
    /// <returns>A result containing a collection of ride response DTOs or an error.</returns>
    Task<Result<IEnumerable<ResponseRide>>> GetRidesByDriverId(int id);

    /// <summary>
    /// Marks a ride as ended.
    /// </summary>
    /// <param name="id">The ID of the ride to end.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> SetEndRide(int id);

    /// <summary>
    /// Deletes a ride from the system.
    /// </summary>
    /// <param name="id">The ID of the ride to delete.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> DeleteRide(int id);
}
