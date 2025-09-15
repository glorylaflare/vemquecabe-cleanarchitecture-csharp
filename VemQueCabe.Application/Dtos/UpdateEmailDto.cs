using System.ComponentModel.DataAnnotations;

namespace VemQueCabe.Application.Dtos;

/// <summary>
/// DTO for updating an email address.
/// </summary>
public class UpdateEmailDto
{
    [Required(ErrorMessage = "Email address is required.")]
    public string Address { get; init; }
}
