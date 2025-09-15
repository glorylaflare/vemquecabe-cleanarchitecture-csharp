using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Domain.ValueObjects;

public class FareTests
{
    [Theory]
    [InlineData(5.0, 1.5)]
    [InlineData(10.0, 2.0)]
    [InlineData(7.5, 3.0)]
    [InlineData(20.0, 1.2)]
    [InlineData(15.0, 2.5)]
    [InlineData(8.0, 1.0)]
    public void CreateFare_ValidData_ShouldCreateFare(decimal baseFare, decimal surgeMultiplier)
    {
        // Act
        var fare = new Fare(baseFare, surgeMultiplier);
        
        // Assert
        Assert.NotNull(fare);
        Assert.Equal(baseFare, fare.BaseFare);
        Assert.Equal(surgeMultiplier, fare.SurgeMultiplier);
        Assert.Equal(2.0m, fare.PricePerKilometer); 
    }

    [Fact]
    public void CreateFare_NegativeBaseFare_ShouldThrowArgumentException()
    {
        // Arrange
        var baseFare = -5.0m;
        var surgeMultiplier = 1.5m;
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new Fare(baseFare, surgeMultiplier));
        
        // Assert
        Assert.Equal("baseFare", exception.ParamName);
        Assert.Contains("Base fare must be greater than zero.", exception.Message);
    }

    [Fact]
    public void CreateFare_SurgeMultiplierLessThanOne_ShouldThrowArgumentException()
    {
        // Arrange
        var baseFare = 5.0m;
        var surgeMultiplier = 0.5m;
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new Fare(baseFare, surgeMultiplier));
        
        // Assert
        Assert.Equal("surgeMultiplier", exception.ParamName);
        Assert.Contains("Surge multiplier must be at least one.", exception.Message);
    }
    
    [Fact]
    public void CalculateTotal_ValidDistance_ShouldCalculateTotal()
    {
        // Arrange
        var baseFare = 5.0m;
        var surgeMultiplier = 1.5m;
        var distance = 10.0m;
        var fare = new Fare(5.0m, 1.5m);
        var expectedTotal = (5.0m + (2.0m * distance)) * surgeMultiplier;
        
        // Act
        fare.CalculateTotal(distance);
        
        // Assert
        Assert.Equal(expectedTotal, fare.Total);
    }

    [Fact]
    public void CalculateTotal_NegativeDistance_ShouldThrowArgumentException()
    {
        // Arrange
        var baseFare = 5.0m;
        var surgeMultiplier = 1.5m;
        var distance = -10.0m;
        var fare = new Fare(5.0m, 1.5m);
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            fare.CalculateTotal(distance));
        
        // Assert
        Assert.Equal("distance", exception.ParamName);
        Assert.Contains("Distance must be greater than zero.", exception.Message);
    }
}