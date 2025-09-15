using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Enums;

namespace VemQueCabe.Tests.Domain.Entities;

public class RideRequestTests
{
    [Fact]
    public void CreateRideRequest_ValidParameters_ShouldCreateRideRequest()
    {
        // Arrange
        var passengerId = 1;
        var origin = "Rua Vergueiro, 3185 - Vila Mariana";
        var destination = "Avenida Paulista, 1000 - Bela Vista";
        var distance = 5.0m;
        var userPreferences = "No preferences";
        
        // Act
        var rideRequest = new RideRequest(passengerId, origin, destination, distance, userPreferences);
        
        // Assert
        Assert.Equal(passengerId, rideRequest.PassengerId);
        Assert.Equal(origin, rideRequest.StartLocation);
        Assert.Equal(destination, rideRequest.EndLocation);
        Assert.Equal(distance, rideRequest.Distance);
        Assert.Equal(userPreferences, rideRequest.UserPreferences);
        Assert.Equal(Status.Pending, rideRequest.Status);
    }

    [Fact]
    public void CreateRideRequest_InvalidPassengerId_ShouldThrowArgumentException()
    {
        // Arrange
        var passengerId = -1;
        var origin = "Rua Vergueiro, 3185 - Vila Mariana";
        var destination = "Avenida Paulista, 1000 - Bela Vista";
        var distance = 5.0m;
        var userPreferences = "No preferences";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new RideRequest(passengerId, origin, destination, distance, userPreferences));
        
        // Assert
        Assert.Equal("passengerId", exception.ParamName);
        Assert.Contains("PassagerId must be greater than zero.", exception.Message);
    }

    [Fact]
    public void CreateRideRequest_EmptyStartLocation_ShouldThrowArgumentException()
    {
        // Arrange
        var passengerId = 1;
        var origin = "";
        var destination = "Avenida Paulista, 1000 - Bela Vista";
        var distance = 5.0m;
        var userPreferences = "No preferences";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new RideRequest(passengerId, origin, destination, distance, userPreferences));
        
        // Assert
        Assert.Equal("startLocation", exception.ParamName);
        Assert.Contains("StartLocation cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void CreateRideRequest_EmptyEndLocation_ShouldThrowArgumentException()
    {
        // Arrange
        var passengerId = 1;
        var origin = "Rua Vergueiro, 3185 - Vila Mariana";
        var destination = "";
        var distance = 5.0m;
        var userPreferences = "No preferences";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new RideRequest(passengerId, origin, destination, distance, userPreferences));
        
        // Assert
        Assert.Equal("endLocation", exception.ParamName);
        Assert.Contains("EndLocation cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void CreateRideRequest_NegativeDistance_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var passengerId = 1;
        var origin = "Rua Vergueiro, 3185 - Vila Mariana";
        var destination = "Avenida Paulista, 1000 - Bela Vista";
        var distance = -5.0m;
        var userPreferences = "No preferences";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new RideRequest(passengerId, origin, destination, distance, userPreferences));

        // Assert
        Assert.Equal("distance", exception.ParamName);
        Assert.Contains("Distance must be greater than zero.", exception.Message);
    }

    [Fact]
    public void UpdateStatus_ValidStatusChange_ShouldUpdateStatus()
    {
        // Arrange
        var passengerId = 1;
        var origin = "Rua Vergueiro, 3185 - Vila Mariana";
        var destination = "Avenida Paulista, 1000 - Bela Vista";
        var distance = -5.0m;
        var userPreferences = "No preferences";
        var rideRequest = new RideRequest(passengerId, origin, destination, 5.0m, userPreferences);
        var newStatus = Status.InProgress;
        
        // Act
        rideRequest.UpdateStatus(newStatus);
        
        // Assert
        Assert.Equal(Status.InProgress, rideRequest.Status);
    }
    
    [Fact]
    public void UpdateStatus_SameStatus_ShouldThrowArgumentException()
    {
        // Arrange
        var passengerId = 1;
        var origin = "Rua Vergueiro, 3185 - Vila Mariana";
        var destination = "Avenida Paulista, 1000 - Bela Vista";
        var distance = -5.0m;
        var userPreferences = "No preferences";
        var rideRequest = new RideRequest(passengerId, origin, destination, 5.0m, userPreferences);
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => 
            rideRequest.UpdateStatus(Status.Pending));
        
        // Assert
        Assert.Equal("Status is already set to the requested value.", exception.Message);
    }
}