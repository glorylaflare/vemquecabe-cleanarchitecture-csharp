using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Domain.Entities;

public class DriverTests
{
    [Fact]
    public void CreateDriver_ValidParameters_ShouldCreateDriver()
    {
        // Arrange
        var userId = 1;
        var vehicle = new Vehicle("Ford", "Focus", 2020, "Branco", "AAA1A11");

        // Act
        var driver = new Driver(userId, vehicle);

        // Assert
        Assert.Equal(userId, driver.UserId);
        Assert.Equal(vehicle, driver.Vehicle);
        Assert.True(driver.IsAvailable);
    }

    [Fact]
    public void CreateDriver_InvalidUserId_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var userId = -1;
        var vehicle = new Vehicle("Ford", "Focus", 2020, "Branco", "AAA1A11");

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => 
            new Driver(userId, vehicle));
        
        // Assert
        Assert.Equal("userId", exception.ParamName);
        Assert.Contains("UserId must be greater than zero.", exception.Message);
    }

    [Fact]
    public void CreateDriver_NullVehicle_ShouldThrowArgumentNullException()
    {
        // Arrange
        var userId = 1;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new Driver(userId, null));
        
        // Assert
        Assert.Equal("vehicle", exception.ParamName);
        Assert.Contains("Vehicle cannot be null.", exception.Message);
    }

    [Fact]
    public void SetAvailability_ValidChange_ShouldUpdateAvailability()
    {
        // Arrange
        var userId = 1;
        var vehicle = new Vehicle("Ford", "Focus", 2020, "Branco", "AAA1A11");
        var driver = new Driver(userId, vehicle);
        var availability = false;

        // Act
        driver.SetAvailability(availability);

        // Assert
        Assert.Equal(availability, driver.IsAvailable);
    }

    [Fact]
    public void SetAvailability_SameValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = 1;
        var vehicle = new Vehicle("Ford", "Focus", 2020, "Branco", "AAA1A11");
        var driver = new Driver(userId, vehicle);
        var availability = true;

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => 
            driver.SetAvailability(availability));

        // Assert
        Assert.Equal("Availability status is already set to the requested value.", exception.Message);
    }
    
    [Fact]
    public void UpdateVehicle_ValidVehicle_ShouldUpdateVehicle()
    {
        // Arrange
        var userId = 1;
        var vehicle = new Vehicle("Ford", "Focus", 2020, "Branco", "AAA1A11");
        var driver = new Driver(userId, vehicle);
        var newVehicle = new Vehicle("Chevrolet", "Cruze", 2021, "Preto", "BBB2B22");

        // Act
        driver.UpdateVehicle(newVehicle);

        // Assert
        Assert.Equal(newVehicle, driver.Vehicle);
    }

    [Fact]
    public void UpdateVehicle_NullVehicle_ShouldThrowArgumentNullException()
    {
        // Arrange
        var userId = 1;
        var vehicle = new Vehicle("Ford", "Focus", 2020, "Branco", "AAA1A11");
        var driver = new Driver(userId, vehicle);

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => 
            driver.UpdateVehicle(null));

        // Assert
        Assert.Equal("vehicle", exception.ParamName);
        Assert.Contains("Vehicle cannot be null.", exception.Message);
    }
}