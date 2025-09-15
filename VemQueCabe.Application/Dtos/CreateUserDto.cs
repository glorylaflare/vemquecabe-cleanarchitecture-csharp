namespace VemQueCabe.Application.Dtos;

/// <summary>
/// Data Transfer Object for creating a new user.
/// </summary>
public class CreateUserDto
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string Password { get; set; }
}
