using VemQueCabe.Domain.Entities;

namespace VemQueCabe.Domain.Interfaces;

/// <summary>
/// Interface for managing passenger data in the repository.
/// </summary>
public interface IPassengerRepository
{
    /// <summary>
    /// Retrieves a passenger by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the passenger.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the passenger if found, otherwise null.</returns>
    Task<Passenger?> GetPassengerByIdAsync(int id);

    /// <summary>
    /// Retrieves all passengers from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all passengers.</returns>
    Task<IEnumerable<Passenger>> GetAllPassengersAsync();

    /// <summary>
    /// Adds a new passenger to the repository.
    /// </summary>
    /// <param name="passenger">The passenger entity to add.</param>
    void AddPassenger(Passenger passenger);

    /// <summary>
    /// Deletes an existing passenger from the repository.
    /// </summary>
    /// <param name="passenger">The passenger entity to delete.</param>
    void DeletePassenger(Passenger passenger);
}
