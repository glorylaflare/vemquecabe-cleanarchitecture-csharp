namespace VemQueCabe.Application.Responses;

/// <summary>
/// Represents the details of a payment response.
/// </summary>
public class ResponsePaymentDetails
{
    public string CardNumber { get; init; }
    public DateOnly ExpirationDate { get; init; }
    public string CardHolderName { get; init; }
}
