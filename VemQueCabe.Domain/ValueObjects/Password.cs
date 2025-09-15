namespace VemQueCabe.Domain.ValueObjects;

/// <summary>
/// Represents a password value object with a hashed password.
/// </summary>
public class Password
{
    public string Hashed { get; private set; }

    protected Password() { }

    public Password(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        Hashed = password;
    }
}
