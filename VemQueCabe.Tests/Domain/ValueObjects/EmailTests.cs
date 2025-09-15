using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Domain.ValueObjects;

public class EmailTests
{
    [Fact]
    public void CreateEmail_ValidAddress_ShouldCreateEmail()
    {
        // Arrange
        var address = "john@teste.com";
        
        // Act
        var email = new Email(address);
        
        // Assert
        Assert.NotNull(email);
        Assert.Equal(address, email.Address);
    }

    [Fact]
    public void CreateEmail_NullOrEmptyAddress_ShouldThrowArgumentException()
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new Email(null));
        
        // Assert
        Assert.Equal("address", exception.ParamName);
        Assert.Contains("Email address cannot be null or empty.", exception.Message);
    }

    [Theory]
    [InlineData("plainaddress")]
    [InlineData("@missingusername.com")]
    [InlineData("username@.com")]
    [InlineData("username@com")]
    [InlineData("username@domain..com")]
    [InlineData("username@domain,com")]
    [InlineData("username@domain com")]
    public void CreateEmail_InvalidFormat_ShouldThrowArgumentException(string address)
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new Email(address));
        
        // Assert
        Assert.Equal("address", exception.ParamName);
        Assert.Contains("Invalid email address format.", exception.Message);
    }
}