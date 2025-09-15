namespace VemQueCabe.Application.Dtos;

/// <summary>
/// DTO for updating user information.
/// </summary>
public class UpdateUserDto
{
    public UpdateNameDto? Name { get; set; }
    public UpdatePhoneNumberDto? PhoneNumber { get; set; }
}
