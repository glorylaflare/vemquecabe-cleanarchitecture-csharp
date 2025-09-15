using System.ComponentModel.DataAnnotations;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Domain.Entities;

/// <summary>
/// Represents a passenger entity in the domain.
/// </summary>
public class Passenger
{
    [Key]
    public int UserId { get; private set; } // Chave estrangeira para User 1:1
    public User User { get; private set; } 
    public PaymentDetails PaymentInformation { get; private set; }
    public bool HasActiveRequest { get; private set; }

    protected Passenger() { }

    public Passenger(int userId, PaymentDetails paymentInformation)
    {
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero.");

        if (paymentInformation == null)
            throw new ArgumentNullException(nameof(paymentInformation), "Payment information cannot be null.");

        UserId = userId;
        PaymentInformation = paymentInformation;
        HasActiveRequest = false;
    }

    public void SetHasActiveRequest(bool hasActiveRequest)
    {
        if (hasActiveRequest == HasActiveRequest)
            throw new InvalidOperationException("Request status is already set to the requested value.");

        HasActiveRequest = hasActiveRequest;
    }

    public void UpdatePaymentInformation(PaymentDetails paymentDetails) =>
        PaymentInformation = paymentDetails ?? throw new ArgumentNullException(nameof(paymentDetails), "Payment information cannot be null.");
}
