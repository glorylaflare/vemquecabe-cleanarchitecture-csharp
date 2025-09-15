using System.Text.RegularExpressions;

namespace VemQueCabe.Domain.ValueObjects;

/// <summary>
/// Represents an email value object with validation for its format.
/// </summary>
public class Email
{
    public string Address { get; private set; }

    protected Email() { }

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Email address cannot be null or empty.", nameof(address));
        if (!IsValid(address))
            throw new ArgumentException("Invalid email address format.", nameof(address));

        Address = address;
    }

    private bool IsValid(string address)
    {
        var pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
        return Regex.IsMatch(address, pattern, RegexOptions.IgnoreCase);
    }
}
