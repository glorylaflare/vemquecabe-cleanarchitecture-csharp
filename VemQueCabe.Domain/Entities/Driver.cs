using System.ComponentModel.DataAnnotations;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Domain.Entities;

/// <summary>
/// Represents a driver entity in the system.
/// </summary>
public class Driver
{
    [Key]
    public int UserId { get; private set; } // Chave estrangeira para User 1:1
    public User User { get; private set; }
    public Vehicle Vehicle { get; private set; } 
    public bool IsAvailable { get; private set; }

    protected Driver() { }

    public Driver(int userId, Vehicle vehicle)
    {
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId), "UserId must be greater than zero.");

        if (vehicle == null)
            throw new ArgumentNullException(nameof(vehicle), "Vehicle cannot be null.");

        UserId = userId;
        Vehicle = vehicle;
        IsAvailable = true;
    }

    public void SetAvailability(bool isAvailable)
    {
        if (isAvailable == IsAvailable)
            throw new InvalidOperationException("Availability status is already set to the requested value.");

        IsAvailable = isAvailable;
    }

    public void UpdateVehicle(Vehicle vehicle) =>
        Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle), "Vehicle cannot be null.");
}
