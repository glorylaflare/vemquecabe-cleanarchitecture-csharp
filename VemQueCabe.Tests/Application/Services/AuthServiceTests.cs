using Moq;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Tests.Application.Fixtures;

namespace VemQueCabe.Tests.Application.Services;

public class AuthServiceTests : AuthServiceFixture
{
    [Fact]
    public async Task AuthenticateAsync_ShouldReturnTokens_WhenCredentialsAreValid()
    {
        // Arrange
        var request = CreateRequestLogin();
        var user = CreateUser(request);
        const string token = "validToken";
        const string refreshToken = "existingRefreshToken";
        var email = request.Email;
        
        _mockRepo.Setup(r => r.Users.GetUserByEmailAsync(email)).ReturnsAsync(user);
        _mockPassword.Setup(p => p.Compare(request.Password, user.Password.Hashed)).Returns(true);
        _mockToken.Setup(t => t.GenerateToken(user)).Returns(token);
        _mockToken.Setup(t => t.GenerateRefreshToken()).Returns(refreshToken);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        
        // Act
        var result = await CreateService().AuthenticateAsync(request);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(user.Role.ToString(), result.Value.Role);
        Assert.Equal(token, result.Value.Token);
        Assert.Equal(refreshToken, result.Value.RefreshToken);
        
        _mockRepo.Verify(r => r.Users.GetUserByEmailAsync(email), Times.Once);
        _mockPassword.Verify(p => p.Compare(request.Password, user.Password.Hashed), Times.Once);
        _mockToken.Verify(t => t.GenerateToken(user), Times.Once);
        _mockToken.Verify(t => t.GenerateRefreshToken(), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnFailure_WhenCredentialsAreInvalid()
    {
        // Arrange
        var request = CreateRequestLogin();
        var email = request.Email;

        _mockRepo.Setup(r => r.Users.GetUserByEmailAsync(email)).ReturnsAsync((User?)null);
        
        // Act
        var result = await CreateService().AuthenticateAsync(request);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Invalid credentials provided.", result.Error.Message);
        Assert.Equal(401, result.Error.Code);
        
        _mockRepo.Verify(r => r.Users.GetUserByEmailAsync(email), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        // Arrange
        var login = CreateRequestLogin();
        var user = CreateUser(login);
        const string refreshToken = "existingRefreshToken";
        const string newRefreshToken = "newRefresh";
        user.AssignRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        
        _mockRepo.Setup(r => r.Users.GetUserByRefreshToken(refreshToken)).ReturnsAsync(user);
        _mockToken.Setup(t => t.GenerateRefreshToken()).Returns(newRefreshToken);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        
        // Act
        var result = await CreateService().RefreshTokenAsync(refreshToken);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Email.Address, result.Value.Email);
        Assert.Equal(user.Role.ToString(), result.Value.Role);
        Assert.Equal(newRefreshToken, result.Value.RefreshToken);
        Assert.True(user.Token.ExpiresAt > DateTime.UtcNow);
    }
    
    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnFailure_WhenUserIsNull()
    {
        // Arrange
        const string refreshToken = "existingRefreshToken";
        
        _mockRepo.Setup(r => r.Users.GetUserByRefreshToken(refreshToken)).ReturnsAsync((User?)null);
        
        // Act
        var result = await CreateService().RefreshTokenAsync(refreshToken);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Users.GetUserByRefreshToken(refreshToken), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnFailure_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        var login = CreateRequestLogin();
        var user = CreateUser(login);
        const string refreshToken = "existingRefreshToken";
        const string newRefreshToken = "newRefresh";
        user.AssignRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        
        _mockRepo.Setup(r => r.Users.GetUserByRefreshToken(refreshToken)).ReturnsAsync(user);
        
        // Act
        var result = await CreateService().RefreshTokenAsync(refreshToken);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Invalid credentials provided.", result.Error.Message);
        Assert.Equal(401, result.Error.Code);
        
        _mockRepo.Verify(r => r.Users.GetUserByRefreshToken(refreshToken), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }
}