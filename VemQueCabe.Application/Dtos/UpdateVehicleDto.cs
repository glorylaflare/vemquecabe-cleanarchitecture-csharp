using System.ComponentModel.DataAnnotations;

namespace VemQueCabe.Application.Dtos;

/// <summary>
/// Data Transfer Object for updating vehicle information.
/// </summary>
public class UpdateVehicleDto
{
    [Required(ErrorMessage = "Brand is required.")]
    public string Brand { get; init; }
    [Required(ErrorMessage = "Model is required.")]
    public string Model { get; init; }
    [Required(ErrorMessage = "Year is required.")]
    public int Year { get; init; }
    [Required(ErrorMessage = "Color is required.")]
    public string Color { get; init; }
    [Required(ErrorMessage = "Plate is required.")]
    public string Plate { get; init; }
}
