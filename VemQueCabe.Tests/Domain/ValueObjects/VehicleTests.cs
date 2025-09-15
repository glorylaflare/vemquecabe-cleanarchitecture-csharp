using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Domain.ValueObjects;

public class VehicleTests
{
    [Fact]
    public void CreateVehicle_ValidData_ShouldCreateVehicle()
    {
        // Arrange
        var brand = "Toyota";
        var model = "Corolla";
        var year = 2020;
        var color = "Blue";
        var plate = "ABC1D23";
        
        // Act
        var vehicle = new Vehicle(brand, model, year, color, plate);
        
        // Assert
        Assert.NotNull(vehicle);
        Assert.Equal(brand, vehicle.Brand);
        Assert.Equal(model, vehicle.Model);
        Assert.Equal(year, vehicle.Year);
        Assert.Equal(color, vehicle.Color);
        Assert.Equal(plate, vehicle.Plate);
    }

    [Fact]
    public void CreateVehicle_NullOrEmptyBrand_ShouldThrowArgumentException()
    {
        // Arrange
        var model = "Corolla";
        var year = 2020;
        var color = "Blue";
        var plate = "ABC1D23";
        
        // Act 
        var exception = Assert.Throws<ArgumentException>(() => 
            new Vehicle(null, model, year, color, plate));
        
        // Assert
        Assert.Equal("brand", exception.ParamName);
        Assert.Contains("Brand cannot be null or empty", exception.Message);
    }

    [Fact]
    public void CreateVehicle_NullOrEmptyModel_ShouldThrowArgumentException()
    {
        // Arrange
        var brand = "Toyota";
        var year = 2020;
        var color = "Blue";
        var plate = "ABC1D23";
        
        // Act 
        var exception = Assert.Throws<ArgumentException>(() => 
            new Vehicle(brand, null, year, color, plate));
        
        // Assert
        Assert.Equal("model", exception.ParamName);
        Assert.Contains("Model cannot be null or empty", exception.Message);
    }

    [Fact]
    public void CreateVehicle_NullOrEmptyColor_ShouldThrowArgumentException()
    {
        // Arrange
        var brand = "Toyota";
        var model = "Corolla";
        var year = 2020;
        var plate = "ABC1D23";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new Vehicle(brand, model, year, null, plate));
        
        // Assert
        Assert.Equal("color", exception.ParamName);
        Assert.Contains("Color cannot be null or empty", exception.Message);
    }

    [Fact]
    public void CreateVehicle_InvalidYear_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var brand = "Toyota";
        var model = "Corolla";
        var color = "Blue";
        var plate = "ABC1D23";
        var invalidYear = 1800; 
        
        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => 
            new Vehicle(brand, model, invalidYear, color, plate));
        
        // Assert
        Assert.Equal("year", exception.ParamName);
        Assert.Contains("Year must be between 1886 and the current year", exception.Message);
    }

    [Fact]
    public void CreateVehicle_FutureYear_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var brand = "Toyota";
        var model = "Corolla";
        var color = "Blue";
        var plate = "ABC1D23";
        var invalidYear = DateTime.Now.Year + 1; 
        
        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => 
            new Vehicle(brand, model, invalidYear, color, plate));
        
        // Assert
        Assert.Equal("year", exception.ParamName);
        Assert.Contains("Year must be between 1886 and the current year", exception.Message);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("AB12C34")]
    [InlineData("A1B2C3D")]
    [InlineData("ABCDE12")]
    [InlineData("ABCD-123")]
    [InlineData("A!C1D23")]
    [InlineData("AB C1D23")]
    public void CreateVehicle_InvalidPlate_ShouldThrowArgumentException(string plate)
    {
        // Arrange
        var brand = "Toyota";
        var model = "Corolla";
        var year = 2020;
        var color = "Blue";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new Vehicle(brand, model, year, color, plate));
        
        // Assert
        Assert.Equal("plate", exception.ParamName);
        Assert.Contains("Invalid plate format", exception.Message);
    }
    
    [Fact]
    public void CreateVehicle_NullOrEmptyPlate_ShouldThrowArgumentException()
    {
        // Arrange
        var brand = "Toyota";
        var model = "Corolla";
        var year = 2020;
        var color = "Blue";
        
        // Act 
        var exception = Assert.Throws<ArgumentException>(() => 
            new Vehicle(brand, model, year, color, null));
        
        // Assert
        Assert.Equal("plate", exception.ParamName);
        Assert.Contains("Plate cannot be null or empty", exception.Message);
    }
}