namespace VemQueCabe.Domain.ValueObjects;

/// <summary>
/// Represents the payment details of a transaction, including card number, expiration date, CVV, and cardholder name.
/// </summary>
public class PaymentDetails
{
    public string CardNumber { get; private set; }
    public DateOnly ExpirationDate { get; private set; }
    public string Cvv { get; private set; }
    public string? CardHolderName { get; private set; }

    protected PaymentDetails() { }

    public PaymentDetails(string cardNumber, DateOnly expirationDate, string cvv)
    {
        if (expirationDate <= DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Expiration date must be in the future.", nameof(expirationDate));
        if (string.IsNullOrWhiteSpace(cvv) || cvv.Length != 3 || !cvv.All(char.IsDigit))
            throw new ArgumentException("CVV must be a 3-digit number.", nameof(cvv));
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new ArgumentException("Card number cannot be null or empty.", nameof(cardNumber));
        if (!cardNumber.All(char.IsDigit))
            throw new ArgumentException("Card number must contain only digits.", nameof(cardNumber));
        
        CardNumber = ValidateCardNumber(cardNumber);
        ExpirationDate = expirationDate;
        Cvv = cvv;

        SetCardHolder(cardNumber);
    }

    private void SetCardHolder(string cardNumber)
    {
        switch (cardNumber[0])
        {
            case '3':
                CardHolderName = "Amex";
                break;
            case '4':
                CardHolderName = "Visa";
                break;
            case '5':
                CardHolderName = "MasterCard";
                break;
            default:
                CardHolderName = "Unknown";
                break;
        }
    }
    
    // Luhn algorithm to validate card number
    private string ValidateCardNumber(string cardNumber)
    {
        var digits = cardNumber.Length;
        
        if (digits < 13 || digits > 19)
            throw new ArgumentException("Card number must be between 13 and 19 digits.", nameof(cardNumber));
        
        var sum = 0;
        var alt = false;
        for (var i = digits - 1; i >= 0; i--)
        {
            var digit = int.Parse(cardNumber[i].ToString()) - 0;
            if (alt)
            {
                digit *= 2;
            }
            sum += digit / 10;
            sum += digit % 10;
            
            alt = !alt;
        }

        var result = sum % 10 == 0;
        
        if (!result) 
            throw new ArgumentException("Invalid card number.", nameof(cardNumber));
        
        return cardNumber;
    }
}
