using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Shared;

namespace VemQueCabe.Application.Interfaces;

/// <summary>
/// Defines the contract for passenger-related operations.
/// </summary>
public interface IPassengerService
{
    /// <summary>
    /// Registers a new passenger.
    /// </summary>
    /// <param name="id">The ID of the user to be registered as a passenger.</param>
    /// <param name="dto">The data transfer object containing passenger details.</param>
    /// <returns>A result containing the registered passenger entity.</returns>
    Task<Result<ResponsePassenger>> RegisterPassenger(CreatePassengerDto dto);

    /// <summary>
    /// Retrieves a passenger by their ID.
    /// </summary>
    /// <param name="id">The ID of the passenger to retrieve.</param>
    /// <returns>A result containing the passenger's response DTO.</returns>
    Task<Result<ResponsePassenger>> GetPassenger(int id);

    /// <summary>
    /// Retrieves all passengers.
    /// </summary>
    /// <returns>A result containing a collection of passenger response DTOs.</returns>
    Task<Result<IEnumerable<ResponsePassenger>>> GetAllPassengers();

    /// <summary>
    /// Updates the payment details of a passenger.
    /// </summary>
    /// <param name="id">The ID of the passenger whose payment details are to be updated.</param>
    /// <param name="dto">The data transfer object containing updated payment details.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    Task<Result> UpdatePaymentDetails(int id, UpdatePaymentDetailsDto dto);

    /// <summary>
    /// Deletes a passenger by their ID.
    /// </summary>
    /// <param name="id">The ID of the passenger to delete.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    Task<Result> DeletePassenger(int id);
}
