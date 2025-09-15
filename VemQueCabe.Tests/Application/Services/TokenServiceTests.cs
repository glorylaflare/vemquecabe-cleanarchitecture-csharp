using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bogus;
using Microsoft.Extensions.Configuration;
using Moq;
using VemQueCabe.Application.Services;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Enums;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Application.Services;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _mockConfig = new();
    private readonly Faker _faker = new();
    
    private TokenService CreateService() => 
        new(_mockConfig.Object);
    
    [Fact]
    public void GenerateToken_ShouldReturnValidToken()
    {
        // Arrange
        var user = new User(
            new Name(_faker.Name.FirstName(), _faker.Name.LastName()),
            new Email(_faker.Internet.Email()),
            new Password(_faker.Internet.Password()));
        user.SetRole(Role.Driver.ToString());
        var fakeSecret = _faker.Random.AlphaNumeric(64);
        
        _mockConfig.Setup(c => c["JWT:Secret"]).Returns(fakeSecret);
        _mockConfig.Setup(c => c["JWT:Issuer"]).Returns("your-app");
        _mockConfig.Setup(c => c["JWT:Audience"]).Returns("your-audience");
        _mockConfig.Setup(c => c["JWT:ExpireMinutes"]).Returns("60");   
        
        // Act
        var result = CreateService().GenerateToken(user);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<string>(result);
        
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result);
        Assert.Equal(user.UserId.ToString(), token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal(user.Email.Address, token.Claims.First(c => c.Type == ClaimTypes.Email).Value);
        Assert.Equal("Driver", token.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        
        _mockConfig.Verify(c => c["JWT:Secret"], Times.Once);
        _mockConfig.Verify(c => c["JWT:Issuer"], Times.Once);
        _mockConfig.Verify(c => c["JWT:Audience"], Times.Once);
        _mockConfig.Verify(c => c["JWT:ExpireMinutes"], Times.Once);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnValidToken()
    {
        // Act
        var result = CreateService().GenerateRefreshToken();
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<string>(result);
        Assert.Equal(44, result.Length);
    }
}