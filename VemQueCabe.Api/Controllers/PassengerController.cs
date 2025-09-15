using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Responses;

namespace VemQueCabe.Api.Controllers;

/// <summary>
/// Controller responsible for managing passenger-related operations.
/// </summary>
[Route("api/passenger")]
[ApiController]
[Authorize]
public class PassengerController : ControllerBase
{
    private readonly IPassengerService _passengerService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PassengerController"/> class.
    /// </summary>
    /// <param name="passengerService">Service for handling passenger operations.</param>
    public PassengerController(IPassengerService passengerService)
    {
        _passengerService = passengerService;
    }

    /// <summary>
    /// Retrieves all passengers.
    /// </summary>
    /// <returns>A list of passengers.</returns>
    /// <response code="200">Returns the list of passengers.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">If no passengers are found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ResponsePassenger>>> GetAllPassengers()
    {
        var result = await _passengerService.GetAllPassengers();
        
        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Retrieves a passenger by ID.
    /// </summary>
    /// <param name="id">The ID of the passenger.</param>
    /// <returns>The passenger with the specified ID.</returns>
    /// <response code="200">Returns the passenger with the specified ID.</response>
    /// <response code="400">If the ID is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">If the passenger is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponsePassenger>> GetPassengerById([Range(1, int.MaxValue)] int id)
    {
        var result = await _passengerService.GetPassenger(id);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new passenger.
    /// </summary>
    /// <param name="passenger">The passenger data to create.</param>
    /// <returns>The created passenger.</returns>
    /// <response code="201">Returns the newly created passenger.</response>
    /// <response code="400">If the model state is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreatePassenger([FromBody] CreatePassengerDto passenger)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _passengerService.RegisterPassenger(passenger);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return CreatedAtAction(nameof(GetPassengerById), new { id = result.Value?.User.UserId }, result.Value);
    }

    /// <summary>
    /// Updates only the payment details of a passenger.
    /// </summary>
    /// <param name="id">The ID of the passenger to update.</param>
    /// <param name="paymentDetails">The updated payment details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">If the update is successful.</response>
    /// <response code="400">If the model state is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="403">If the user is not allowed to update the payment details.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPut("{id:int}/payment")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "PassengersOnly")]
    public async Task<ActionResult> UpdatePaymentInfo([Range(1, int.MaxValue)] int id, [FromBody] UpdatePaymentDetailsDto paymentDetails)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _passengerService.UpdatePaymentDetails(id, paymentDetails);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a passenger by ID.
    /// </summary>
    /// <param name="id">The ID of the passenger to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">If the deletion is successful.</response>
    /// <response code="400">If the ID is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="403">If the user is not allowed to delete the passenger.</response>
    /// <response code="404">If the passenger is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpDelete("{id:int}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "PassengersOnly")]
    public async Task<ActionResult> DeletePassenger([Range(1, int.MaxValue)] int id)
    {
        var result = await _passengerService.DeletePassenger(id);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return NoContent();
    }
}
