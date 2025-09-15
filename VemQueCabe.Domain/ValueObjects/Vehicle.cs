using System.Text.RegularExpressions;

namespace VemQueCabe.Domain.ValueObjects;

/// <summary>
/// Represents a vehicle with properties such as brand, model, year, color, and plate.
/// </summary>
public class Vehicle
{
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public string Color { get; private set; }
    public string Plate { get; private set; }

    protected Vehicle() { }

    public Vehicle(string brand, string model, int year, string color, string plate)
    {
        if (string.IsNullOrWhiteSpace(brand))
            throw new ArgumentException("Brand cannot be null or empty.", nameof(brand));
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be null or empty.", nameof(model));
        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Color cannot be null or empty.", nameof(color));
        if (year < 1886 || year >= DateTime.Now.Year + 1)
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be between 1886 and the current year");
        if (!ValidatePlateFormat(plate))
            throw new ArgumentException("Invalid plate format.", nameof(plate));

        Brand = brand;
        Model = model;
        Year = year;
        Color = color;
        Plate = plate;
    }

    private bool ValidatePlateFormat(string plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            throw new ArgumentException("Plate cannot be null or empty.", nameof(plate));

        var pattern = @"^[A-Z]{3}[0-9][A-Z][0-9]{2}$";
        return Regex.IsMatch(plate, pattern, RegexOptions.IgnoreCase);
    }
}
