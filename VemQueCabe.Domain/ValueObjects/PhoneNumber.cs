namespace VemQueCabe.Domain.ValueObjects;

/// <summary>
/// Represents a phone number value object with country code and number.
/// </summary>
public class PhoneNumber
{
    public string CountryCode { get; private set; } 
    public string Number { get; private set; } 

    protected PhoneNumber() { }

    public PhoneNumber(string countryCode, string number)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentException("Country code cannot be null or empty.", nameof(countryCode));
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Number cannot be null or empty.", nameof(number));
        if (!IsNumberValid(number))
            throw new ArgumentException("Number must be between 7 and 15 digits and contain only numeric characters.", nameof(number));
        if (!IsCountryCodeValid(countryCode))
            throw new ArgumentException("Invalid country code format.", nameof(countryCode));

        CountryCode = countryCode;
        Number = number;
    }

    public bool IsNumberValid(string number) =>
        number.Length >= 7 && number.All(char.IsDigit);

    public bool IsCountryCodeValid(string countryCode) =>
        countryCode.Length > 1 && countryCode.All(char.IsDigit);
}
