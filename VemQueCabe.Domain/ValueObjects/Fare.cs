namespace VemQueCabe.Domain.ValueObjects;

/// <summary>
/// Represents a fare value object that encapsulates pricing details for a trip.
/// </summary>
public class Fare
{
    public decimal BaseFare { get; private set; }
    public decimal PricePerKilometer { get; private set; } = 2.0m;
    public decimal SurgeMultiplier { get; private set; }
    public decimal Total { get; private set; }

    protected Fare() { }

    public Fare(decimal baseFare, decimal surgeMultiplier)
    {
        if (baseFare <= 0)
            throw new ArgumentException("Base fare must be greater than zero.", nameof(baseFare));
        if (surgeMultiplier < 1)
            throw new ArgumentException("Surge multiplier must be at least one.", nameof(surgeMultiplier));

        BaseFare = baseFare;
        SurgeMultiplier = surgeMultiplier;
    }

    public void CalculateTotal(decimal distance)
    {
        if (distance <= 0)
            throw new ArgumentException("Distance must be greater than zero.", nameof(distance));

        Total = ((BaseFare + (PricePerKilometer * distance)) * SurgeMultiplier);
    }
}
