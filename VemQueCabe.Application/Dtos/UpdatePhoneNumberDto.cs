namespace VemQueCabe.Application.Dtos;

/// <summary>
/// DTO for updating a phone number.
/// </summary>
public class UpdatePhoneNumberDto
{
    public string? CountryCode { get; init; }
    public string? Number { get; init; }
}
