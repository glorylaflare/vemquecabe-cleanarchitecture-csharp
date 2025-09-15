using System.ComponentModel.DataAnnotations;

namespace VemQueCabe.Application.Requests;

/// <summary>
/// Represents the data transfer object for login requests.
/// </summary>
public class RequestLogin
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; init; }

    [Required]
    [MinLength(3, ErrorMessage = "Password must be at least 3 characters long.")]
    [DataType(DataType.Password)]
    public string Password { get; init; }
}
