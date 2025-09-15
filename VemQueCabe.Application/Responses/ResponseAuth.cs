namespace VemQueCabe.Application.Responses;

/// <summary>
/// Represents the response data for a login operation.
/// </summary>
public class ResponseAuth
{
    public string Email { get; init; }
    public string Token { get; init; }
    public string RefreshToken { get; init; }
    public string Role { get; init; }
}
