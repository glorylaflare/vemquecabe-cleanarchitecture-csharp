namespace VemQueCabe.Domain.ValueObjects;

/// <summary>
/// Represents a refresh token with its expiration and creation dates.
/// Used for authentication and token management.
/// </summary>
public class Token
{
    public string RefreshToken { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    protected Token() { }
    
    public Token(string refreshToken, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token cannot be null or empty.", nameof(refreshToken));
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration date must be in the future.", nameof(expiresAt));
        
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }
}