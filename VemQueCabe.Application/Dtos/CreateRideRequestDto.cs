namespace VemQueCabe.Application.Dtos;

/// <summary>
/// DTO for creating a ride request.
/// </summary>
public class CreateRideRequestDto
{
    public int PassengerId { get; set; }
    public string StartLocation { get; set; }
    public string EndLocation { get; set; }
    public decimal Distance { get; set; }
    public string? UserPreferences { get; set; }
}
