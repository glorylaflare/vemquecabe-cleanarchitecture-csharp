namespace VemQueCabe.Domain.ValueObjects;

/// <summary>
/// Represents a person's name with a first name and a last name.
/// </summary>
public class Name
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    protected Name() { }

    public Name(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be null or empty.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
    }
}
