using System.ComponentModel.DataAnnotations;
using VemQueCabe.Domain.Enums;

namespace VemQueCabe.Domain.Entities;

/// <summary>
/// Represents a ride request made by a passenger.
/// </summary>
public class RideRequest
{
    [Key]
    public int RequestId { get; private set; }
    public int PassengerId { get; private set; }
    public Passenger Passenger { get; private set; }
    public string StartLocation { get; private set; }
    public string EndLocation { get; private set; } 
    public decimal Distance { get; private set; } 
    public string UserPreferences { get; private set; } 
    public Status Status { get; private set; } 

    protected RideRequest() { }

    public RideRequest(int passengerId, string startLocation, string endLocation, decimal distance, string? userPreferences)
    {
        if (passengerId <= 0)
            throw new ArgumentException("PassagerId must be greater than zero.", nameof(passengerId));

        if (string.IsNullOrWhiteSpace(startLocation))
            throw new ArgumentException("StartLocation cannot be null or empty.", nameof(startLocation));

        if (string.IsNullOrWhiteSpace(endLocation))
            throw new ArgumentException("EndLocation cannot be null or empty.", nameof(endLocation));

        if (distance <= 0)
            throw new ArgumentException("Distance must be greater than zero.", nameof(distance));

        PassengerId = passengerId;
        StartLocation = startLocation.Trim();
        EndLocation = endLocation.Trim();
        Distance = distance;
        UserPreferences = userPreferences ?? string.Empty;
        Status = Status.Pending;
    }

    public void UpdateStatus(Status newStatus)
    {
        if (Status == newStatus)
            throw new InvalidOperationException("Status is already set to the requested value.");

        Status = newStatus;
    }
}
