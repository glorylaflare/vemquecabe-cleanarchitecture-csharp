namespace VemQueCabe.Application.Responses;

/// <summary>
/// Represents the response data transfer object for a ride.
/// </summary>
public class ResponseRide
{
    public int RideId { get; init; }
    public ResponseRideRequest RideRequest { get; init; }
    public ResponseDriver Driver { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public ResponseFare Fare { get; init; }
}
