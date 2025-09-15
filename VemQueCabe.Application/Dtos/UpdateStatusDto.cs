using System.ComponentModel.DataAnnotations;

namespace VemQueCabe.Application.Dtos;

/// <summary>
/// Data Transfer Object for updating the status.
/// </summary>
public class UpdateStatusDto
{
    [Required(ErrorMessage = "Status is required.")]
    public string Status { get; init; }
}
