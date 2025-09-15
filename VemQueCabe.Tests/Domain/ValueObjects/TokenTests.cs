using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Domain.ValueObjects;

public class TokenTests
{
    [Fact]
    public void CreateToken_ValidData_ShouldCreateToken()
    {
        // Arrange
        var bcrypt = new BCrypt.Net.BCrypt().ToString();
        var refreshToken = bcrypt;
        var expiresAt = DateTime.UtcNow.AddDays(7);
        
        // Act
        var token = new Token(refreshToken, expiresAt);
        
        // Assert
        Assert.NotNull(token);
        Assert.Equal(refreshToken, token.RefreshToken);
        Assert.Equal(expiresAt, token.ExpiresAt);
    }

    [Fact]
    public void CreateToken_EmptyRefreshToken_ShouldThrowArgumentException()
    {
        // Arrange
        var refreshToken = "";
        var expiresAt = DateTime.UtcNow.AddDays(7);
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new Token(refreshToken, expiresAt));
        
        // Assert
        Assert.Equal("refreshToken", exception.ParamName);
        Assert.Contains("Refresh token cannot be null or empty", exception.Message);
    }

    [Fact]
    public void CreateToken_PastExpirationDate_ShouldThrowArgumentException()
    {
        // Arrange
        var bcrypt = new BCrypt.Net.BCrypt().ToString();
        var refreshToken = bcrypt;
        var expiresAt = DateTime.UtcNow.AddDays(-1);
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new Token(refreshToken, expiresAt));
        
        // Assert
        Assert.Equal("expiresAt", exception.ParamName);
        Assert.Contains("Expiration date must be in the future", exception.Message);
    }

    [Fact]
    public void CreateToken_ExpirationDateIsNow_ShouldThrowArgumentException()
    {
        // Arrange
        var bcrypt = new BCrypt.Net.BCrypt().ToString();
        var refreshToken = bcrypt;
        var expiresAt = DateTime.UtcNow;
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new Token(refreshToken, expiresAt));
        
        // Assert
        Assert.Equal("expiresAt", exception.ParamName);
        Assert.Contains("Expiration date must be in the future", exception.Message);
    }
}