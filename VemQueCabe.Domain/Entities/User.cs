using System.ComponentModel.DataAnnotations;
using VemQueCabe.Domain.Enums;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Domain.Entities;

/// <summary>
/// Represents a user in the system with properties such as Name, Email, PhoneNumber, Password, and Role.
/// </summary>
public class User
{
    [Key]
    public int UserId { get; private set; }
    public Name Name { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public Password Password { get; private set; }
    public Role Role { get; private set; }
    public Token Token { get; private set; }

    protected User() { }

    public User(Name name, Email email, Password password)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");
        Email = email ?? throw new ArgumentNullException(nameof(email), "Email cannot be null.");
        Password = password ?? throw new ArgumentNullException(nameof(password), "Password cannot be null.");
        Role = Role.Pending;
    }

    public void SetRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be null or empty.", nameof(role));

        if (!Enum.TryParse<Role>(role, true, out var parsedRole))
            throw new ArgumentException("Invalid role.", nameof(role));
        
        if (parsedRole == Role)
            throw new ArgumentException("User already has this role.", nameof(role));

        Role = parsedRole;
    }
    
    public void UpdateEmailAddress(Email email) => 
        Email = email ?? throw new ArgumentNullException(nameof(email), "Email cannot be null.");

    public void AssignRefreshToken(string refreshToken, DateTime expiresAt) =>
        Token = new Token(refreshToken, expiresAt);
}
