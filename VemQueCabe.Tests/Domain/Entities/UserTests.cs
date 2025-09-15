using Bogus;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Enums;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Domain.Entities;

public class UserTests
{
    private readonly Faker _faker = new();

    private Name CreateName() =>
        new(_faker.Name.FirstName(), _faker.Name.LastName());
    
    private Email CreateEmail() =>
        new(_faker.Internet.Email());
    
    private Password CreatePassword() =>
        new(_faker.Internet.Password(8, true, null, "!1Aa"));
    
    [Fact]
    public void CreateUser_ValidData_ShouldCreateUser()
    {
        // Arrange
        var name = CreateName();
        var email = CreateEmail();
        var password = CreatePassword();
        
        // Act
        var user = new User(name, email, password);
        
        // Assert
        Assert.NotNull(user);
        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
        Assert.Equal(password, user.Password);
        Assert.Equal(Role.Pending, user.Role);
    }

    [Fact]
    public void CreateUser_NullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var email = CreateEmail();
        var password = CreatePassword();
        
        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new User(null, email, password));
        
        // Assert
        Assert.Equal("name", exception.ParamName);
        Assert.Contains("Name cannot be null", exception.Message);
    }
    
    [Fact]
    public void CreateUser_NullEmail_ShouldThrowArgumentNullException()
    {
        // Arrange
        var name = CreateName();
        var password = CreatePassword();
        
        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new User(name, null, password));
        
        // Assert
        Assert.Equal("email", exception.ParamName);
        Assert.Contains("Email cannot be null", exception.Message);
    }
    
    [Fact]
    public void CreateUser_NullPassword_ShouldThrowArgumentNullException()
    {
        // Arrange
        var name = CreateName();
        var email = CreateEmail();
        
        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new User(name, email, null));
        
        // Assert
        Assert.Equal("password", exception.ParamName);
        Assert.Contains("Password cannot be null", exception.Message);
    }

    [Fact]
    public void SetRole_ValidRole_ShouldUpdateRole()
    {
        // Arrange
        var role = "Passenger";
        var name = CreateName();
        var email = CreateEmail();
        var password = CreatePassword();
        var user = new User(name, email, password);
        
        // Act
        user.SetRole(role);
        
        // Assert
        Assert.Equal(Role.Passenger, user.Role);
    }

    [Fact]
    public void SetRole_InvalidRole_ShouldThrowArgumentException()
    {
        // Arrange
        var role = "NotARole";
        var name = CreateName();
        var email = CreateEmail();
        var password = CreatePassword();
        var user = new User(name, email, password);
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            user.SetRole(role));
        
        // Assert
        Assert.Equal("role", exception.ParamName);
        Assert.Contains("Invalid role", exception.Message);
    }
    
    [Fact]
    public void SetRole_EmptyRole_ShouldThrowArgumentException()
    {
        // Arrange
        var role = "";
        var name = CreateName();
        var email = CreateEmail();
        var password = CreatePassword();
        var user = new User(name, email, password);
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            user.SetRole(role));
        
        // Assert
        Assert.Equal("role", exception.ParamName);
        Assert.Contains("Role cannot be null or empty", exception.Message);
    }

    [Fact]
    public void SetRole_SameRole_ShouldThrowArgumentException()
    {
        // Arrange
        var role = "Passenger";
        var name = CreateName();
        var email = CreateEmail();
        var password = CreatePassword();
        var user = new User(name, email, password);
        user.SetRole(role);
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            user.SetRole(role));
        
        // Assert
        Assert.Equal("role", exception.ParamName);
        Assert.Contains("User already has this role", exception.Message);
    }

    [Fact]
    public void UpdateEmailAddress_ValidEmail_ShouldUpdateEmail()
    {
        // Arrange
        var name = CreateName();
        var email = CreateEmail();
        var password = CreatePassword();
        var user = new User(name, email, password);
        var newEmail = CreateEmail();

        // Act
        user.UpdateEmailAddress(newEmail);
        
        // Assert
        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public void UpdateEmailAddress_NullEmail_ShouldThrowArgumentNullException()
    {
        // Arrange
        var name = CreateName();
        var email = CreateEmail();
        var password = CreatePassword();
        var user = new User(name, email, password);

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => 
            user.UpdateEmailAddress(null));
        
        // Assert
        Assert.Equal("email", exception.ParamName);
        Assert.Contains("Email cannot be null", exception.Message);
    }
}