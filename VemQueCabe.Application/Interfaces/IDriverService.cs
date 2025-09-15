using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Shared;

namespace VemQueCabe.Application.Interfaces;

/// <summary>
/// Service interface for managing drivers and their associated operations.
/// </summary>
public interface IDriverService
{
    /// <summary>
    /// Registers a new driver in the system.
    /// </summary>
    /// <param name="dto">The data transfer object containing driver details.</param>
    /// <returns>A result containing the created driver entity.</returns>
    Task<Result<ResponseDriver>> RegisterDriver(CreateDriverDto dto);

    /// <summary>
    /// Retrieves a driver by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the driver.</param>
    /// <returns>A result containing the driver details.</returns>
    Task<Result<ResponseDriver>> GetDriver(int id);

    /// <summary>
    /// Retrieves all drivers in the system.
    /// </summary>
    /// <returns>A result containing a collection of driver details.</returns>
    Task<Result<IEnumerable<ResponseDriver>>> GetAllDrivers();

    /// <summary>
    /// Retrieves all available drivers in the system.
    /// </summary>
    /// <returns>A result containing a collection of available driver details.</returns>
    Task<Result<IEnumerable<ResponseDriver>>> GetAvailableDrivers();

    /// <summary>
    /// Updates the vehicle information for a specific driver.
    /// </summary>
    /// <param name="id">The unique identifier of the driver.</param>
    /// <param name="dto">The data transfer object containing updated vehicle details.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    Task<Result> UpdateVehicle(int id, UpdateVehicleDto dto);

    /// <summary>
    /// Deletes a driver from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the driver.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    Task<Result> DeleteDriver(int id);
}
