namespace VemQueCabe.Application.Responses;

/// <summary>
/// Represents the response DTO for a ride request.
/// </summary>
public class ResponseRideRequest
{
    public int RequestId { get; init; }
    public ResponsePassenger Passenger { get; init; }
    public string StartLocation { get; init; }
    public string EndLocation { get; init; }
    public decimal Distance { get; init; }
    public string? UserPreferences { get; init; }
    public string Status { get; init; }
}
