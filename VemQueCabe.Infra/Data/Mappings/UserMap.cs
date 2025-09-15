using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VemQueCabe.Domain.Entities;

namespace VemQueCabe.Infra.Data.Mappings;

/// <summary>
/// Provides the Entity Framework Core mapping configuration for the User entity.
/// </summary>
public class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.UserId);
        builder.OwnsOne(u => u.Name, name =>
        {
            name.Property(n => n.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(maxLength: 128);
            name.Property(n => n.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(maxLength: 255);
        });
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Address).HasColumnName("Email").IsRequired().HasMaxLength(maxLength: 150);
            email.HasIndex(e => e.Address).IsUnique();
        });
        builder.OwnsOne(u => u.PhoneNumber, phone =>
        {
            phone.Property(p => p.CountryCode).HasColumnName("PhoneCountryCode").HasMaxLength(maxLength: 3);
            phone.Property(p => p.Number).HasColumnName("PhoneNumber").HasMaxLength(maxLength: 32);
        });
        builder.OwnsOne(u => u.Password, password =>
        {
            password.Property(p => p.Hashed).HasColumnName("PasswordHash").IsRequired().HasMaxLength(maxLength: 128);
        });
        builder.Property(e => e.Role).IsRequired();
        builder.OwnsOne(u => u.Token, token =>
        {
            token.Property(t => t.RefreshToken).HasColumnName("RefreshToken").HasMaxLength(maxLength: 512);
            token.Property(t => t.ExpiresAt).HasColumnName("TokenExpiresAt");
            token.Property(t => t.CreatedAt).HasColumnName("TokenCreatedAt");
        });
    }
}
