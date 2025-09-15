using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Domain.ValueObjects;

public class NameTests
{
    [Fact]
    public void CreateName_ValidData_ShouldCreateName()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Marston";
        
        // Act
        var name = new Name(firstName, lastName);
        
        // Assert
        Assert.NotNull(name);
        Assert.Equal(firstName, name.FirstName);
        Assert.Equal(lastName, name.LastName);
    }

    [Fact]
    public void CreateName_EmptyFirstName_ShouldThrowArgumentException()
    {
        // Arrange
        var firstName = "";
        var lastName = "Marston";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new Name(firstName, lastName));
        
        // Assert
        Assert.Equal("firstName", exception.ParamName);
        Assert.Contains("First name cannot be null or empty", exception.Message);
    }

    [Fact]
    public void CreateName_NullLastName_ShouldThrowArgumentException()
    {
        // Arrange
        var firstName = "John";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            new Name(firstName, null));
        
        // Assert
        Assert.Equal("lastName", exception.ParamName);
        Assert.Contains("Last name cannot be null or empty", exception.Message);
    }
}