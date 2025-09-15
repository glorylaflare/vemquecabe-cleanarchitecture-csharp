using System.ComponentModel.DataAnnotations;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Domain.Entities;

/// <summary>
/// Represents a ride entity in the domain.
/// </summary>
public class Ride
{
    [Key]
    public int RideId { get; private set; }
    public int RideRequestId { get; private set; }
    public RideRequest RideRequest { get; private set; }
    public int DriverId { get; private set; }
    public Driver Driver { get; private set; }
    public DateTime StartTime { get; private set; } 
    public DateTime? EndTime { get; private set; } 
    public Fare Fare { get; private set; } 

    protected Ride() { }

    public Ride(int rideRequestId, int driverId, Fare fare)
    {
        if (rideRequestId <= 0)
            throw new ArgumentException("RideRequestId must be greater than zero", nameof(rideRequestId));

        if (driverId <= 0)
            throw new ArgumentException("DriverId must be greater than zero", nameof(driverId));

        if (fare == null)
            throw new ArgumentNullException(nameof(fare), "Fare cannot be null.");

        RideRequestId = rideRequestId;
        DriverId = driverId;
        StartTime = DateTime.UtcNow;
        Fare = fare;
    }

    public void EndRide()
    {
        if (EndTime.HasValue)
            throw new InvalidOperationException("The ride has already ended.");

        EndTime = DateTime.UtcNow;
    }
}