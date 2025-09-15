using VemQueCabe.Domain.Entities;

namespace VemQueCabe.Domain.Interfaces;

/// <summary>
/// Interface for managing user data operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found, otherwise null.</returns>
    Task<User?> GetUserByIdAsync(int id);

    /// <summary>
    /// Updates the details of an existing user.
    /// </summary>
    /// <param name="user">The user entity with updated information.</param>
    void UpdateUser(User user);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found, otherwise null.</returns>
    Task<User?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Retrieves a user by their refresh token.
    /// </summary>
    /// <param name="token">The refresh token associated with the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found, otherwise null.</returns>
    Task<User?> GetUserByRefreshToken(string token);

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="user">The user entity to be created.</param>
    void CreateUser(User user);

    /// <summary>
    /// Checks if a user exists with the specified email address.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if a user exists with the specified email, otherwise false.</returns>
    Task<bool> ExistsByEmailAsync(string email);
}
