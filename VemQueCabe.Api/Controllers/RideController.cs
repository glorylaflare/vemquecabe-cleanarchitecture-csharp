using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Responses;

namespace VemQueCabe.Api.Controllers;

/// <summary>
/// Controller for managing rides.
/// </summary>
[Route("api/ride")]
[ApiController]
[Authorize]
public class RideController : ControllerBase
{
    private readonly IRideService _rideService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RideController"/> class.
    /// </summary>
    /// <param name="rideService">Service for ride operations.</param>
    public RideController(IRideService rideService)
    {
        _rideService = rideService;
    }

    /// <summary>
    /// Retrieves all rides.
    /// </summary>
    /// <returns>A list of all rides.</returns>
    /// <response code="200">Returns the list of all rides.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">If no rides are found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ResponseRide>>> GetAllRides()
    {
        var result = await _rideService.GetAllRides();

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Retrieves a ride by its ID.
    /// </summary>
    /// <param name="id">The ID of the ride.</param>
    /// <returns>The ride with the specified ID.</returns>
    /// <response code="200">Returns the ride with the specified ID.</response>
    /// <response code="400">If the ride ID is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">If the ride is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseRide>> GetRideById([Range(1, int.MaxValue)] int id)
    {
        var result = await _rideService.GetRideById(id);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }


    /// <summary>
    /// Retrieves all rides associated with a specific driver by their ID.
    /// </summary>
    /// <param name="id">The ID of the driver whose rides are to be retrieved.</param>
    /// <returns>A list of rides associated with the specified driver.</returns>
    /// <response code="200">Returns the list of rides for the specified driver.</response>
    /// <response code="400">If the driver ID is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">If no rides are found for the specified driver.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("driver/{id:int}/list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ResponseRide>>> GetRidesByDriverId([Range(1, int.MaxValue)] int id)
    {
        var result = await _rideService.GetRidesByDriverId(id);
        
        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new ride.
    /// </summary>
    /// <param name="ride">The data transfer object containing ride details.</param>
    /// <returns>The created ride.</returns>
    /// <response code="201">Returns the newly created ride.</response>
    /// <response code="400">If the model state is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateRide([FromBody] CreateRideDto ride)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _rideService.CreateRide(ride);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return CreatedAtAction(nameof(GetRideById), new { id = result.Value?.RideId }, result);
    }

    /// <summary>
    /// Marks a ride as ended.
    /// </summary>
    /// <param name="id">The ID of the ride to end.</param>
    /// <returns>No content if the operation is successful.</returns>
    /// <response code="204">If the ride is successfully ended.</response>
    /// <response code="400">If the ride ID is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="403">If the user is not allowed to update the ride status.</response>
    /// <response code="404">If the ride is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "RideOwner")]
    public async Task<ActionResult> UpdateRideStatus([Range(1, int.MaxValue)] int id)
    {
        var result = await _rideService.SetEndRide(id);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a ride by its ID.
    /// </summary>
    /// <param name="id">The ID of the ride to delete.</param>
    /// <returns>No content if the operation is successful.</returns>
    /// <response code="204">If the ride is successfully deleted.</response>
    /// <response code="400">If the ride ID is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="403">If the user is not allowed to delete the ride.</response>
    /// <response code="404">If the ride is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpDelete("{id:int}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "RideOwner")]
    public async Task<ActionResult> DeleteRide([Range(1, int.MaxValue)] int id)
    {
        var result = await _rideService.DeleteRide(id);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return NoContent();
    }
}
