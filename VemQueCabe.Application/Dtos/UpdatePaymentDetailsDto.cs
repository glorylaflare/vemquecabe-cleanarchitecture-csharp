using System.ComponentModel.DataAnnotations;

namespace VemQueCabe.Application.Dtos;

/// <summary>
/// DTO for updating payment details.
/// </summary>
public class UpdatePaymentDetailsDto
{
    [Required(ErrorMessage = "Card number is required.")]
    public string CardNumber { get; init; }
    [Required(ErrorMessage = "Expiration date is required.")]
    public DateOnly ExpirationDate { get; init; }
    [Required(ErrorMessage = "CVV is required.")]
    public string Cvv { get; init; }
}
