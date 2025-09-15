using VemQueCabe.Domain.Enums;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Application.Responses;

/// <summary>
/// Represents the response data transfer object for a user.
/// </summary>
public record ResponseUser
{
    public int UserId { get; init; }
    public Name Name { get; init; }
    public Email Email { get; init; }
    public PhoneNumber? PhoneNumber { get; init; }
    public Role Role { get; init; }
}
