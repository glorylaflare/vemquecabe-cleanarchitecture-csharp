using Moq;
using VemQueCabe.Application.Extensions;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.ValueObjects;
using VemQueCabe.Tests.Application.Fixtures;

namespace VemQueCabe.Tests.Application.Services;

public class UserServiceTests : UserServiceFixture
{
    [Fact]
    public async Task CreateUser_ShouldReturnSuccess_WhenUserIsCreated()
    {
        // Arrange
        var dto = GenerateCreateUserDto();
        var email = dto.Email;
        var password = dto.Password;
        var user = GenerateUser(dto);
        var response = GenerateResponseUser(user);
        var cacheKey = CacheKeys.User.ById(user.UserId);
        
        _mockRepo.Setup(r => r.Users.GetUserByEmailAsync(email)).ReturnsAsync((User?)null);
        _mockPassword.Setup(p => p.Hash(password)).Returns(It.IsAny<string>());
        _mockMap.Setup(m => m.Map<User>(dto)).Returns(user);
        _mockRepo.Setup(r => r.Users.CreateUser(user));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockMap.Setup(m => m.Map<ResponseUser>(user)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().CreateUser(dto);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(response.Email, result.Value.Email);
        Assert.Equal(response.Role, result.Value.Role);
        
        _mockRepo.Verify(r => r.Users.GetUserByEmailAsync(email), Times.Once);
        _mockPassword.Verify(p => p.Hash(password), Times.Once);
        _mockMap.Verify(m => m.Map<User>(dto), Times.Once);
        _mockRepo.Verify(r => r.Users.CreateUser(user), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<ResponseUser>(user), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenUserAlreadyExists()
    {
        // Arrange
        var dto = GenerateCreateUserDto();
        var email = dto.Email;
        var user = GenerateUser(dto);
        
        _mockRepo.Setup(r => r.Users.GetUserByEmailAsync(email)).ReturnsAsync(user);
        
        // Act
        var result = await CreateService().CreateUser(dto);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("An error occurred while trying to create the user.", result.Error.Message);
        Assert.Equal(409, result.Error.Code);
        
        _mockRepo.Verify(r => r.Users.GetUserByEmailAsync(email), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenCommitFails()
    {
        // Arrange
        var dto = GenerateCreateUserDto();
        var email = dto.Email;
        var password = dto.Password;
        var user = GenerateUser(dto);
        
        _mockRepo.Setup(r => r.Users.GetUserByEmailAsync(email)).ReturnsAsync((User?)null);
        _mockPassword.Setup(p => p.Hash(password)).Returns(It.IsAny<string>());
        _mockMap.Setup(m => m.Map<User>(dto)).Returns(user);
        _mockRepo.Setup(r => r.Users.CreateUser(user));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().CreateUser(dto);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Users.GetUserByEmailAsync(email), Times.Once);
        _mockPassword.Verify(p => p.Hash(password), Times.Once);
        _mockMap.Verify(m => m.Map<User>(dto), Times.Once);
        _mockRepo.Verify(r => r.Users.CreateUser(user), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task GetUser_ShouldReturnSuccess_WhenUserExists()
    {
        // Arrange
        const int id = 1;
        var dto = GenerateCreateUserDto();
        var user = GenerateUser(dto);
        var response = GenerateResponseUser(user);
        var cacheKey = CacheKeys.User.ById(id);
        
        _mockCache.Setup(c => c.GetAsync<ResponseUser>(cacheKey)).ReturnsAsync((ResponseUser?)null);
        _mockRepo.Setup(r => r.Users.GetUserByIdAsync(id)).ReturnsAsync(user);
        _mockMap.Setup(m => m.Map<ResponseUser>(user)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().GetUser(id);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(response.Email, result.Value.Email);
        Assert.Equal(response.Role, result.Value.Role);
        
        _mockCache.Verify(c => c.GetAsync<ResponseUser>(cacheKey), Times.Once);
        _mockRepo.Verify(r => r.Users.GetUserByIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map<ResponseUser>(user), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetUser_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        const int id = 1;
        var key = CacheKeys.User.ById(id);

        _mockCache.Setup(c => c.GetAsync<ResponseUser>(key)).ReturnsAsync((ResponseUser?)null);
        _mockRepo.Setup(r => r.Users.GetUserByIdAsync(id)).ReturnsAsync((User?)null);
         
        // Act
        var result = await CreateService().GetUser(id);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockCache.Verify(c => c.GetAsync<ResponseUser>(key), Times.Once);
        _mockRepo.Verify(r => r.Users.GetUserByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetUser_ShouldReturnSuccess_WhenUserIsInCache()
    {
        // Arrange
        const int id = 1;
        var dto = GenerateCreateUserDto();
        var user = GenerateUser(dto);
        var response = GenerateResponseUser(user);
        var cacheKey = CacheKeys.User.ById(id);
        
        _mockCache.Setup(c => c.GetAsync<ResponseUser>(cacheKey)).ReturnsAsync(response);
        
        // Act
        var result = await CreateService().GetUser(id);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(response.Email, result.Value.Email);
        Assert.Equal(response.Role, result.Value.Role);
        
        _mockCache.Verify(c => c.GetAsync<ResponseUser>(cacheKey), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnSuccess_WhenUserIsUpdated()
    {
        // Arrange
        const int id = 1;
        var dto = GenerateUpdateUserDto();
        var user = GenerateUser(GenerateCreateUserDto(), id);
        var cacheKey = CacheKeys.User.ById(id);
        var response = GenerateResponseUser(user);
        
        _mockRepo.Setup(r => r.Users.GetUserByIdAsync(id)).ReturnsAsync(user);
        _mockMap.Setup(m => m.Map(dto, user)).Returns(user);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockMap.Setup(m => m.Map<ResponseUser>(user)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().UpdateUser(id, dto);
        
        // Assert
        Assert.True(result.IsSuccess);
        
        _mockRepo.Verify(r => r.Users.GetUserByIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map(dto, user), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<ResponseUser>(user), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        const int id = 1;
        var dto = GenerateUpdateUserDto();
        
        _mockRepo.Setup(r => r.Users.GetUserByIdAsync(id)).ReturnsAsync((User?)null);
        
        // Act
        var result = await CreateService().UpdateUser(id, dto);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Users.GetUserByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnFailure_WhenCommitFails()
    {
        // Arrange
        const int id = 1;
        var dto = GenerateUpdateUserDto();
        var user = GenerateUser(GenerateCreateUserDto(), id);
        
        _mockRepo.Setup(r => r.Users.GetUserByIdAsync(id)).ReturnsAsync(user);
        _mockMap.Setup(m => m.Map(dto, user)).Returns(user);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().UpdateUser(id, dto);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Users.GetUserByIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map(dto, user), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task SetUserRole_ShouldReturnSuccess_WhenUserRoleIsSet()
    {
        // Arrange
        const int id = 1;
        var user = GenerateUser(GenerateCreateUserDto(), id);
        const string role = "Driver";
        const string token = "token";
        user.AssignRefreshToken("refreshToken", DateTime.UtcNow.AddDays(7));
        var response = GenerateResponseUser(user);
        var cacheKey = CacheKeys.User.ById(id);
        
        _mockRepo.Setup(r => r.Users.GetUserByIdAsync(id)).ReturnsAsync(user);
        _mockToken.Setup(t => t.GenerateToken(user)).Returns(token);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockMap.Setup(m => m.Map<ResponseUser>(user)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().SetUserRole(id, role);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(user.Email.Address, result.Value.Email);
        Assert.Equal(role, result.Value.Role);
        Assert.Equal(token, result.Value.Token);
        Assert.Equal(user.Token.RefreshToken, result.Value.RefreshToken);
        
        _mockRepo.Verify(r => r.Users.GetUserByIdAsync(id), Times.Once);
        _mockToken.Verify(t => t.GenerateToken(user), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<ResponseUser>(user), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task SetUserRole_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        const int id = 1;
        const string role = "Driver";
        
        _mockRepo.Setup(r => r.Users.GetUserByIdAsync(id)).ReturnsAsync((User?)null);
        
        // Act
        var result = await CreateService().SetUserRole(id, role);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Users.GetUserByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task SetUserRole_ShouldReturnFailure_WhenCommitFails()
    {
        // Arrange
        const int id = 1;
        var user = GenerateUser(GenerateCreateUserDto(), id);
        const string role = "Driver";
        const string token = "token";
        user.AssignRefreshToken("refreshToken", DateTime.UtcNow.AddDays(7));
        
        _mockRepo.Setup(r => r.Users.GetUserByIdAsync(id)).ReturnsAsync(user);
        _mockToken.Setup(t => t.GenerateToken(user)).Returns(token);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().SetUserRole(id, role);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Users.GetUserByIdAsync(id), Times.Once);
        _mockToken.Verify(t => t.GenerateToken(user), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateEmail_ShouldReturnSuccess_WhenEmailIsUpdated()
    {
        // Arrange
        const int id = 1;
        var dto = GenerateUpdateEmailDto();
        var emailAddress = dto.Address;
        var user = GenerateUser(GenerateCreateUserDto(), id);
        var email = GenerateEmail(dto);
        var response = GenerateResponseUser(user);
        var cacheKey = CacheKeys.User.ById(id);
        
        _mockRepo.Setup(r => r.Users.ExistsByEmailAsync(emailAddress)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.Users.GetUserByIdAsync(id)).ReturnsAsync(user);
        _mockMap.Setup(m => m.Map<Email>(dto)).Returns(email);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockMap.Setup(m => m.Map<ResponseUser>(user)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().UpdateEmail(id, dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(emailAddress, user.Email.Address);
        
        _mockRepo.Verify(r => r.Users.ExistsByEmailAsync(emailAddress), Times.Once);
        _mockRepo.Verify(r => r.Users.GetUserByIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map<Email>(dto), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<ResponseUser>(user), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEmail_ShouldReturnFailure_WhenEmailIsAlreadyInUse()
    {
        // Arrange
        const int id = 1;
        var dto = GenerateUpdateEmailDto();
        var emailAddress = dto.Address;
        
        _mockRepo.Setup(r => r.Users.ExistsByEmailAsync(emailAddress)).ReturnsAsync(true);
        
        // Act
        var result = await CreateService().UpdateEmail(id, dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("It wasn't possible to update the user.", result.Error.Message);
        Assert.Equal(409, result.Error.Code);
        
        _mockRepo.Verify(r => r.Users.ExistsByEmailAsync(emailAddress), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateEmail_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        const int id = 1;
        var dto = GenerateUpdateEmailDto();
        var emailAddress = dto.Address;
        
        _mockRepo.Setup(r => r.Users.ExistsByEmailAsync(emailAddress)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.Users.GetUserByIdAsync(id)).ReturnsAsync((User?)null);
        
        // Act
        var result = await CreateService().UpdateEmail(id, dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Users.ExistsByEmailAsync(emailAddress), Times.Once);
        _mockRepo.Verify(r => r.Users.GetUserByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task UpdateEmail_ShouldReturnFailure_WhenCommitFails()
    {
        // Arrange
        const int id = 1;
        var dto = GenerateUpdateEmailDto();
        var emailAddress = dto.Address;
        var user = GenerateUser(GenerateCreateUserDto(), id);
        var email = GenerateEmail(dto);
        
        _mockRepo.Setup(r => r.Users.ExistsByEmailAsync(emailAddress)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.Users.GetUserByIdAsync(id)).ReturnsAsync(user);
        _mockMap.Setup(m => m.Map<Email>(dto)).Returns(email);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().UpdateEmail(id, dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Users.ExistsByEmailAsync(emailAddress), Times.Once);
        _mockRepo.Verify(r => r.Users.GetUserByIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map<Email>(dto), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }
}