using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Responses;

namespace VemQueCabe.Api.Controllers;

/// <summary>
/// Controller responsible for managing user-related operations.
/// </summary>
[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="userService">The user service.</param>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The user with the specified ID.</returns>
    /// <response code="200">Returns the user with the specified ID.</response>
    /// <response code="400">Invalid user ID.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    public async Task<ActionResult<ResponseUser>> GetUserById([Range(1, int.MaxValue)] int id)
    {
        var result = await _userService.GetUser(id);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="role">The user information.</param>
    /// <returns>The created user.</returns>
    /// <response code="201">User created successfully.</response>
    /// <response code="400">Invalid input data or model state.</response>
    /// <response code="403">The user does not have permission to update the role.</response>
    /// <response code="500">Internal server error occurred during user creation.</response>
    [HttpPost("{id:int}/role")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseAuth>> SetUserRole([Range(1, int.MaxValue)] int id, [FromBody] CreateRoleDto role)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.SetUserRole(id, role.RoleName);
        
        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }
        
        return Ok(result.Value);
    }

    /// <summary>
    /// Updates a user's information.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="user">The updated user information.</param>
    /// <returns>No content if the update is successful.</returns>
    /// <response code="204">User updated successfully.</response>
    /// <response code="400">Invalid input data.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="403">If the user is forbidden from updating the email.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:int}/update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    public async Task<ActionResult> UpdateUser([Range(1, int.MaxValue)] int id, [FromBody] UpdateUserDto user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.UpdateUser(id, user);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return NoContent();
    }

    /// <summary>
    /// Updates the email address of a user.
    /// </summary>
    /// <param name="id">The ID of the user whose email is to be updated.</param>
    /// <param name="email">The new email address to be set for the user.</param>
    /// <returns>No content if the email update is successful.</returns>
    /// <response code="204">Email updated successfully.</response>
    /// <response code="400">Invalid input data or model state.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="403">If the user is forbidden from updating the email.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error occurred during email update.</response>
    [HttpPatch("{id:int}/email")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    public async Task<ActionResult> UpdateUserEmail([Range(1, int.MaxValue)] int id, [FromBody] UpdateEmailDto email)
    {
        var result = await _userService.UpdateEmail(id, email);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return NoContent();
    }
}
