using Microsoft.AspNetCore.Mvc;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Requests;

namespace VemQueCabe.Api.Controllers;

/// <summary>
/// Controller responsible for handling authentication-related operations.
/// </summary>
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authService">Service for handling authentication logic.</param>
    /// <param name="userService">Service for handling user-related operations.</param>
    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="dto">The user information.</param>
    /// <returns>The created user.</returns>
    /// <response code="201">User created successfully.</response>
    /// <response code="400">Invalid input data or model state.</response>
    /// <response code="500">Internal server error occurred during user creation.</response>
    [HttpPost("/api/register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Register([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.CreateUser(dto);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return CreatedAtAction("GetUserById", "User", new { id = result.Value?.UserId }, result);
    }

    /// <summary>
    /// Authenticates a user based on the provided login credentials.
    /// </summary>
    /// <param name="dto">The login credentials.</param>
    /// <returns>A response containing the authentication result.</returns>
    /// <response code="200">Returns the authentication result for a successful login.</response>
    /// <response code="400">Return if an error occurred during authentication.</response>
    /// <response code="401">If the authentication fails due to invalid credentials.</response>
    /// <response code="500">If an unexpected error occurs on the server.</response> 
    [HttpPost("/api/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Login([FromBody] RequestLogin dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.AuthenticateAsync(dto);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }
    
    /// <summary>
    /// Refreshes the authentication token using the provided refresh token.
    /// </summary>
    /// <param name="token">The refresh token request.</param>
    /// <returns>A response containing the new authentication token.</returns>
    /// <response code="200">Returns the new authentication token.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If the refresh token is invalid or expired.</response>
    /// <response code="500">If an unexpected error occurs on the server.</response>
    [HttpPost("/api/refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RefreshToken([FromBody] RequestRefreshToken token)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RefreshTokenAsync(token.RefreshToken);

        if (result.IsFailure)
        {
            var error = result.Error;
            return StatusCode(error.Code, error.Message);
        }

        return Ok(result.Value);
    }
}
