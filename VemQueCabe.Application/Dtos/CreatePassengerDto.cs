namespace VemQueCabe.Application.Dtos;

/// <summary>
/// Data Transfer Object for creating a new passenger.
/// </summary>
public class CreatePassengerDto
{
    public int UserId { get; init; }
    public string CardNumber { get; init; }
    public DateOnly ExpirationDate { get; init; }
    public string Cvv { get; init; }
}
