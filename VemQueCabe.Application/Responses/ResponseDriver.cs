using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Application.Responses;

/// <summary>
/// Represents the response data transfer object for a driver.
/// </summary>
public class ResponseDriver
{
    public ResponseUser User { get; init; }
    public Vehicle Vehicle { get; init; }
    public bool IsAvailable { get; init; }
}
