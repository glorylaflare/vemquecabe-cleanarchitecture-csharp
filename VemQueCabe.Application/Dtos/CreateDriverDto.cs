namespace VemQueCabe.Application.Dtos;

/// <summary>
/// Data Transfer Object for creating a Driver.
/// </summary>
public class CreateDriverDto
{
    public int UserId { get; init; }
    public string Brand { get; init; }
    public string Model { get; init; }
    public int Year { get; init; }
    public string Color { get; init; }
    public string Plate { get; init; }
}
