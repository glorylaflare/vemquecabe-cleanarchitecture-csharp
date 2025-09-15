namespace VemQueCabe.Application.Responses;

/// <summary>
/// Represents the response DTO for a passenger.
/// </summary>
public class ResponsePassenger
{
    public ResponseUser User { get; init; }
    public ResponsePaymentDetails PaymentInformation { get; init; }
    public bool HasActiveRequest { get; init; }
}
