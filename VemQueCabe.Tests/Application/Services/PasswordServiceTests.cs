using VemQueCabe.Application.Services;

namespace VemQueCabe.Tests.Application.Services;

public class PasswordServiceTests
{
    private readonly PasswordService _passwordService = new();
    
    [Theory]
    [InlineData("TesteDeSenha123!")]
    [InlineData("OutraSenha456$")]
    [InlineData("SenhaComCaracteresEspeciais@#%&*()")]
    public void HashPassword_ShouldReturnHashedPassword(string password)
    {
        // Act
        var hashedPassword = _passwordService.Hash(password);

        // Assert
        Assert.NotEqual(password, hashedPassword);
    }
    
    
    [Fact]
    public void HashPassword_ShouldThrowExceptionForNullOrEmptyPassword()
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            _passwordService.Hash(string.Empty));

        // Assert
        Assert.Equal("Password cannot be null or empty. (Parameter 'password')", exception.Message);
        Assert.Equal("password", exception.ParamName);
    }
    
    [Theory]
    [InlineData("TesteDeSenha123!")]
    [InlineData("OutraSenha456$")]
    [InlineData("SenhaComCaracteresEspeciais@#%&*()")]
    public void ComparePassword_ShouldReturnTrueForMatchingPasswords(string password)
    {
        // Act
        var hashedPassword = _passwordService.Hash(password);

        // Assert
        Assert.True(_passwordService.Compare(password, hashedPassword));
    }
    
    [Theory]
    [InlineData("TesteDeSenha123!", "TesteDaSenha1234!")]
    [InlineData("OutraSenha456$", "Outra4567$")]
    [InlineData("SenhaComCaracteresEspeciais@#%&*()", "SenhaCom")]
    public void ComparePassword_ShouldReturnFalseForNonMatchingPasswords(string password, string passwordToCompare)
    {
        // Arrange
        var hashPassword = BCrypt.Net.BCrypt.HashPassword(passwordToCompare);
        
        // Act
        var hashedPassword = _passwordService.Compare(password, hashPassword);

        // Assert
        Assert.False(hashedPassword);
    }

    [Fact]
    public void ComparePassword_ShouldThrowExceptionForNullOrEmptyPassword()
    {
        // Arrange
        const string hashedPassword = "hashedPassword";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            _passwordService.Compare(string.Empty, hashedPassword));
        
        // Assert
        Assert.Equal("Password cannot be null or empty. (Parameter 'password')", exception.Message);
        Assert.Equal("password", exception.ParamName);
    }

    [Fact]
    public void ComparePassword_ShouldThrowExceptionForNullOrEmptyHashedPassword()
    {
        // Arrange
        const string password = "TesteDeSenha123!";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            _passwordService.Compare(password, string.Empty));
        
        // Assert
        Assert.Equal("Hashed password cannot be null or empty. (Parameter 'hashedPassword')", exception.Message);
        Assert.Equal("hashedPassword", exception.ParamName);
    }
}