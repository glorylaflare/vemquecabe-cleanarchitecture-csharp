using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Shared;

namespace VemQueCabe.Application.Interfaces;

/// <summary>
/// Interface for user-related operations in the application.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="dto">The data transfer object containing user creation details.</param>
    /// <returns>A <see cref="Result{T}"/> containing the created <see cref="ResponseUser"/> or an error.</returns>
    Task<Result<ResponseUser>> CreateUser(CreateUserDto dto);

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>A <see cref="Result{T}"/> containing the <see cref="ResponseUser"/> or an error.</returns>
    Task<Result<ResponseUser>> GetUser(int id);

    /// <summary>
    /// Updates the details of an existing user.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="dto">The data transfer object containing updated user details.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> UpdateUser(int id, UpdateUserDto dto);

    /// <summary>
    /// Updates the email address of an existing user.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="dto">The data transfer object containing the new email address.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> UpdateEmail(int id, UpdateEmailDto dto);

    /// <summary>
    /// Sets the role of a user in the system.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="role">The role to assign to the user.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result<ResponseAuth>> SetUserRole(int id, string role);
}
