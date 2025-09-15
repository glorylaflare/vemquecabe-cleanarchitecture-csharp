namespace VemQueCabe.Application.Dtos;

/// <summary>
/// Data Transfer Object for creating a ride.
/// </summary>
public class CreateRideDto
{
    public int RideRequestId { get; init; }
    public int DriverId { get; init; }
    public decimal BaseFare { get; init; }
    public decimal SurgeMultiplier { get; init; }
}
