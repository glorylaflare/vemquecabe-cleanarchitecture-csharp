using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Domain.ValueObjects;

public class PhoneNumberTests
{
    [Fact]
    public void CreatePhoneNumber_ValidData_ShouldCreatePhoneNumber()
    {
        // Arrange
        var countryCode = "41";
        var number = "1234567";
        
        // Act
        var phoneNumber = new PhoneNumber(countryCode, number);
        
        // Assert
        Assert.NotNull(phoneNumber);
        Assert.Equal(countryCode, phoneNumber.CountryCode);
        Assert.Equal(number, phoneNumber.Number);
    }

    [Fact]
    public void CreatePhoneNumber_NullCountryCode_ShouldThrowArgumentException()
    {
        // Arrange
        var number = "123456789";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new PhoneNumber(null, number));
        
        // Assert
        Assert.Equal("countryCode", exception.ParamName);
        Assert.Contains("Country code cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void CreatePhoneNumber_NullNumber_ShouldThrowArgumentException()
    {
        // Arrange
        var countryCode = "41";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new PhoneNumber(countryCode, null));
        
        // Assert
        Assert.Equal("number", exception.ParamName);
        Assert.Contains("Number cannot be null or empty.", exception.Message);
    }

    [Theory]
    [InlineData("123")] 
    [InlineData("123456")] 
    [InlineData("12345abc")] 
    [InlineData("!@#$%^&*()")] 
    public void CreatePhoneNumber_InvalidNumber_ShouldThrowArgumentException(string number)
    {
        // Arrange
        var countryCode = "41";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new PhoneNumber(countryCode, number));
        
        // Assert
        Assert.Equal("number", exception.ParamName);
        Assert.Contains("Number must be between 7 and 15 digits and contain only numeric characters.", exception.Message);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("a1")]
    [InlineData("!@#")]
    public void CreatePhoneNumber_InvalidCountryCode_ShouldThrowArgumentException(string countryCode)
    {
        // Arrange
        var number = "123456789";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new PhoneNumber(countryCode, number));
        
        // Assert
        Assert.Equal("countryCode", exception.ParamName);
        Assert.Contains("Invalid country code format.", exception.Message);
    }
}