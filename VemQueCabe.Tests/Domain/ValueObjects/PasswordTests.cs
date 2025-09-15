using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Domain.ValueObjects;

public class PasswordTests
{
    [Fact]
    public void CreatePassword_ValidData_ShouldCreatePassword()
    {
        // Arrange
        var bcrypt = new BCrypt.Net.BCrypt().ToString();
        var passwordString = bcrypt;
        
        // Act
        var password = new Password(passwordString);
        
        // Assert
        Assert.NotNull(password);
        Assert.Equal(passwordString, password.Hashed);
    }

    [Fact]
    public void CreatePassword_NullOrEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var passwordString = "";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new Password(passwordString));
        
        // Assert
        Assert.Equal("password", exception.ParamName);
        Assert.Contains("Password cannot be null or empty", exception.Message);
    }
}