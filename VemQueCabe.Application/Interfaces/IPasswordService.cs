namespace VemQueCabe.Application.Interfaces;

/// <summary>
/// Provides methods for hashing passwords and comparing hashed passwords.
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Compares a plain text password with a hashed password to determine if they match.
    /// </summary>
    /// <param name="password">The plain text password to compare.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>True if the passwords match; otherwise, false.</returns>
    bool Compare(string password, string hashedPassword);

    /// <summary>
    /// Hashes a plain text password.
    /// </summary>
    /// <param name="password">The plain text password to hash.</param>
    /// <returns>The hashed password.</returns>
    string Hash(string password);
}
