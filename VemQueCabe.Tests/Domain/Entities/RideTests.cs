using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Domain.Entities;

public class RideTests
{
    [Fact]
    public void CreateRide_ValidParameters_ShouldCreateRide()
    {
        // Arrange
        var rideRequestId = 1;
        var driverId = 1;
        var fare = new Fare(50.0m, 5.0m);
        
        // Act
        var ride = new Ride(rideRequestId, driverId, fare);
        
        // Assert
        Assert.Equal(rideRequestId, ride.RideRequestId);
        Assert.Equal(driverId, ride.DriverId);
        Assert.Equal(fare, ride.Fare);
        Assert.NotEqual(default, ride.StartTime);
        Assert.Null(ride.EndTime);
    }

    [Fact]
    public void CreateRide_InvalidRideRequestId_ShouldThrowArgumentException()
    {
        // Arrange
        var rideRequestId = -1;
        var driverId = 1;
        var fare = new Fare(50.0m, 5.0m);
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new Ride(rideRequestId, driverId, fare));
        
        // Assert
        Assert.Equal("rideRequestId", exception.ParamName);
        Assert.Contains("RideRequestId must be greater than zero", exception.Message);
    }
    
    [Fact]
    public void CreateRide_InvalidDriverId_ShouldThrowArgumentException()
    {
        // Arrange
        var rideRequestId = 1;
        var driverId = -1;
        var fare = new Fare(50.0m, 5.0m);
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new Ride(rideRequestId, driverId, fare));
        
        // Assert
        Assert.Equal("driverId", exception.ParamName);
        Assert.Contains("DriverId must be greater than zero", exception.Message);
    }

    [Fact]
    public void CreateRide_NullFare_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rideRequestId = 1;
        var driverId = 1;
        
        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new Ride(rideRequestId, driverId, null));
        
        // Assert
        Assert.Equal("fare", exception.ParamName);
        Assert.Contains("Fare cannot be null.", exception.Message);
    }

    [Fact]
    public void EndRide_ValidEnd_ShouldSetEndTime()
    {
        // Arrange
        var rideRequestId = 1;
        var driverId = 1;
        var fare = new Fare(50.0m, 5.0m);
        var ride = new Ride(rideRequestId, driverId, fare);
        
        // Act
        ride.EndRide();
        
        // Assert
        Assert.NotNull(ride.EndTime);
        Assert.NotEqual(default, ride.EndTime);
    }

    [Fact]
    public void EndRide_AlreadyEnded_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var rideRequestId = 1;
        var driverId = 1;
        var fare = new Fare(50.0m, 5.0m);
        var ride = new Ride(rideRequestId, driverId, fare);
        ride.EndRide();
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => 
            ride.EndRide());
        
        // Assert
        Assert.Equal("The ride has already ended.", exception.Message);
    }
}