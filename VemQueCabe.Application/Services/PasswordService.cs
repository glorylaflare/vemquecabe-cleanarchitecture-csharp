using VemQueCabe.Application.Interfaces;

namespace VemQueCabe.Application.Services;

/// <summary>
/// Service for handling password hashing and comparison.
/// </summary>
public class PasswordService : IPasswordService
{
    public bool Compare(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Hashed password cannot be null or empty.", nameof(hashedPassword));

        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
