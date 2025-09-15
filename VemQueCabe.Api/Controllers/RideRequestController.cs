using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Enums;

namespace VemQueCabe.Api.Controllers;

/// <summary>
/// Controller responsible for managing ride requests.
/// </summary>
[Route("api/request")]
[ApiController]
[Authorize]
public class RideRequestController : ControllerBase
{
    private readonly IRideRequestService _rideRequestService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RideRequestController"/> class.
    /// </summary>
    /// <param name="rideRequestService">Service for handling ride requests.</param>
    public RideRequestController(IRideRequestService rideRequestService)
    {
        _rideRequestService = rideRequestService;
    }

    /// <summary>
    /// Retrieves all ride requests.
    /// </summary>
    /// <returns>A list of all ride requests.</returns>
    /// <response code="200">Returns the list of ride requests.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">No ride requests found.</response>
    /// <response code="500">An error occurred while retrieving ride requests.</response>
    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ResponseRideRequest>>> GetAllRequests()
    {
        var result = await _rideRequestService.GetAllRideRequests();

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Retrieves all ride requests for a specific passenger.
    /// </summary>
    /// <param name="id">The ID of the passenger.</param>
    /// <returns>A list of ride requests for the specified passenger.</returns>
    /// <response code="200">Returns the list of ride requests for the passenger.</response>
    /// <response code="400">Invalid passenger ID provided.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">No ride requests found for the specified passenger.</response>
    /// <response code="500">An error occurred while retrieving ride requests for the passenger.</response>
    [HttpGet("passenger/{id:int}/list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ResponseRideRequest>>> GetAllRequestsByPassenger([Range(1, int.MaxValue)] int id)
    {
        var result = await _rideRequestService.GetRequestsByPassengerId(id);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Retrieves all ride requests with a specific status.
    /// </summary>
    /// <param name="status">The status of the ride requests (e.g., Pending, InProgress).</param>
    /// <returns>A list of ride requests with the specified status.</returns>
    /// <response code="200">Returns the list of ride requests with the specified status.</response>
    /// <response code="400">Invalid status value provided.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">No ride requests found with the specified status.</response>
    /// <response code="500">An error occurred while retrieving ride requests with the specified status.</response>
    [HttpGet("status/get")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ResponseRideRequest>>> GetAllRequestsByStatus([FromQuery] Status status)
    {
        var result = await _rideRequestService.GetRequestsByStatus(status);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Retrieves a specific ride request by its ID.
    /// </summary>
    /// <param name="id">The ID of the ride request.</param>
    /// <returns>The ride request with the specified ID.</returns>
    /// <response code="200">Returns the ride request with the specified ID.</response>
    /// <response code="400">Invalid ride request ID provided.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">Ride request with the specified ID not found.</response>
    /// <response code="500">An error occurred while retrieving the ride request by ID.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseRideRequest>> GetRequestById([Range(1, int.MaxValue)] int id)
    {
        var result = await _rideRequestService.GetRideRequestById(id);
       
        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new ride request.
    /// </summary>
    /// <param name="rideRequest">The data transfer object containing the ride request details.</param>
    /// <returns>The created ride request.</returns>
    /// <response code="201">Ride request created successfully.</response>
    /// <response code="400">Invalid data provided for creating the ride request.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">An error occurred while creating the ride request.</response>
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateRequest([FromBody] CreateRideRequestDto rideRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _rideRequestService.CreateRideRequest(rideRequest);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return CreatedAtAction(nameof(GetRequestById), new { id = result.Value?.RequestId }, result);
    }

    /// <summary>
    /// Updates the status of a specific ride request.
    /// </summary>
    /// <param name="id">The ID of the ride request.</param>
    /// <param name="status">The new status to be applied.</param>
    /// <returns>No content if the update is successful.</returns>
    /// <response code="204">Ride request status updated successfully.</response>
    /// <response code="400">Invalid status value provided.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">Ride request with the specified ID not found or update failed.</response>
    /// <response code="500">An error occurred while updating the ride request status.</response>
    [HttpPatch("{id:int}/update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "RideRequestOwner")]
    public async Task<ActionResult> UpdateRequestStatus([Range(1, int.MaxValue)] int id, [FromQuery] Status status)
    {
        var result = await _rideRequestService.UpdateRideRequestStatus(id, status);
        
        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a specific ride request.
    /// </summary>
    /// <param name="id">The ID of the ride request to delete.</param>
    /// <returns>No content if the deletion is successful.</returns>
    /// <response code="204">Ride request deleted successfully.</response>
    /// <response code="400">Invalid ride request ID provided.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="403">If the user is not allowed to delete the ride request.</response>
    /// <response code="404">Ride request with the specified ID not found or deletion failed.</response>
    /// <response code="500">An error occurred while deleting the ride request.</response>
    [HttpDelete("{id:int}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "RideRequestOwner")]
    public async Task<ActionResult> DeleteRequest([Range(1, int.MaxValue)] int id)
    {
        var result = await _rideRequestService.DeleteRideRequest(id);
        
        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return NoContent();
    }

    /// <summary>
    /// Cancels a specific ride request.
    /// </summary>
    /// <param name="id">The ID of the ride request to cancel.</param>
    /// <returns>Returns an HTTP 200 OK response if the cancellation is successful.</returns>
    /// <response code="200">Ride request canceled successfully.</response>
    /// <response code="400">Invalid ride request ID provided or cancellation not allowed.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="403">If the user is not allowed to cancel the ride request.</response>
    /// <response code="500">An error occurred while canceling the ride request.</response>
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "RideRequestOwner")]
    public async Task<ActionResult> CancelRequestById([Range(1, int.MaxValue)] int id)
    {
        var result = await _rideRequestService.CancelRideRequest(id);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok("Ride request canceled successfully.");
    }
}
