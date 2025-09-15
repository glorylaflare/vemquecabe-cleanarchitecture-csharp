using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Responses;

namespace VemQueCabe.Api.Controllers;

/// <summary>
/// Controller responsible for managing driver-related operations.
/// </summary>
[Route("api/driver")]
[ApiController]
[Authorize]
public class DriverController : ControllerBase
{
    private readonly IDriverService _driverService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DriverController"/> class.
    /// </summary>
    /// <param name="driverService">Service for driver-related operations.</param>
    public DriverController(IDriverService driverService)
    {
        _driverService = driverService;
    }

    /// <summary>
    /// Retrieves a list of all available drivers.
    /// </summary>
    /// <returns>A list of available drivers.</returns>
    /// <response code="200">Returns the list of available drivers.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">If no available drivers are found.</response>
    /// <response code="500">If an unexpected error occurs on the server.</response>
    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ResponseDriver>>> GetAllDrivers()
    {
        var result = await _driverService.GetAllDrivers();

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Retrieves a list of currently available drivers.
    /// </summary>
    /// <returns>A list of available drivers.</returns>
    /// <response code="200">Returns the list of available drivers.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">If no available drivers are found.</response>
    /// <response code="500">If an unexpected error occurs on the server.</response>
    [HttpGet("list/available")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ResponseDriver>>> GetAvailableDrivers()
    {
        var result = await _driverService.GetAvailableDrivers();

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Retrieves a specific driver by their ID.
    /// </summary>
    /// <param name="id">The ID of the driver to retrieve.</param>
    /// <returns>The driver with the specified ID.</returns>
    /// <response code="200">Returns the driver with the specified ID.</response>
    /// <response code="400">If the provided ID is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">If the driver is not found.</response>
    /// <response code="500">If an unexpected error occurs on the server.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDriver>> GetDriverById([Range(1, int.MaxValue)] int id)
    {
        var result = await _driverService.GetDriver(id);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Registers a new driver.
    /// </summary>
    /// <param name="driver">The data transfer object containing driver details.</param>
    /// <returns>The created driver.</returns>
    /// <response code="201">Returns the newly created driver.</response>
    /// <response code="400">If the model state is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="403">If the user is not allowed to register a driver.</response>
    /// <response code="409">If a driver with the same email already exists.</response>
    /// <response code="500">If an unexpected error occurs on the server.</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateDriver([FromBody] CreateDriverDto driver)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _driverService.RegisterDriver(driver);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return CreatedAtAction(nameof(GetDriverById), new { id = result.Value?.User.UserId }, result.Value);
    }

    /// <summary>
    /// Updates the vehicle information for a specific driver.
    /// </summary>
    /// <param name="id">The ID of the driver whose vehicle is being updated.</param>
    /// <param name="vehicle">The data transfer object containing updated vehicle details.</param>
    /// <returns>No content if the update is successful.</returns>
    /// <response code="204">If the update is successful.</response>
    /// <response code="400">If the model state is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="403">If the user is not allowed to update the vehicle.</response>
    /// <response code="404">If the driver is not found.</response>
    /// <response code="409">If there is a conflict while updating the vehicle.</response>
    /// <response code="500">If an unexpected error occurs on the server.</response>
    [HttpPut("{id:int}/vehicle")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "DriversOnly")]
    public async Task<ActionResult> UpdateVehicleInfo([Range(1, int.MaxValue)] int id, [FromBody] UpdateVehicleDto vehicle)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _driverService.UpdateVehicle(id, vehicle);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a driver by their ID.
    /// </summary>
    /// <param name="id">The ID of the driver to delete.</param>
    /// <returns>No content if the deletion is successful.</returns>
    /// <response code="204">If the deletion is successful.</response>
    /// <response code="404">If the driver is not found.</response>
    /// <response code="400">If the provided ID is invalid.</response>
    /// <response code="500">If an unexpected error occurs on the server.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="403">If the user is not allowed to delete the driver.</response>
    [HttpDelete("{id:int}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "DriversOnly")]
    public async Task<ActionResult> DeleteDriver([Range(1, int.MaxValue)] int id)
    {
        var result = await _driverService.DeleteDriver(id);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return NoContent();
    }
}
