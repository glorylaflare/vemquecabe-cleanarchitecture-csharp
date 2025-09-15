using VemQueCabe.Domain.Entities;

namespace VemQueCabe.Domain.Interfaces;

/// <summary>
/// Interface for managing driver-related data operations.
/// </summary>
public interface IDriverRepository
{
    /// <summary>
    /// Retrieves a driver by their unique user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the driver if found, otherwise null.</returns>
    Task<Driver?> GetDriverByIdAsync(int userId);

    /// <summary>
    /// Retrieves all drivers in the system.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all drivers.</returns>
    Task<IEnumerable<Driver>> GetAllDriversAsync();

    /// <summary>
    /// Adds a new driver to the system.
    /// </summary>
    /// <param name="driver">The driver entity to add.</param>
    void AddDriver(Driver driver);

    /// <summary>
    /// Deletes an existing driver from the system.
    /// </summary>
    /// <param name="driver">The driver entity to delete.</param>
    void DeleteDriver(Driver driver);

    /// <summary>
    /// Retrieves all drivers who are currently available.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of available drivers.</returns>
    Task<IEnumerable<Driver>> GetAvailableDriversAsync();

    /// <summary>
    /// Checks if a driver exists by their vehicle's license plate.
    /// </summary>
    /// <param name="plate">The license plate of the vehicle.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if a driver with the specified plate exists, otherwise false.</returns>
    Task<bool> ExistsByPlateAsync(string plate);
}
