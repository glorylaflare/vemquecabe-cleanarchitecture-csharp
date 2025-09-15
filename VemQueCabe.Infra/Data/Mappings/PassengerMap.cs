using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VemQueCabe.Domain.Entities;

namespace VemQueCabe.Infra.Data.Mappings;

/// <summary>
/// Provides the Entity Framework Core mapping configuration for the Passenger entity.
/// </summary>
public class PassengerMap : IEntityTypeConfiguration<Passenger>
{
    public void Configure(EntityTypeBuilder<Passenger> builder)
    {
        builder.HasKey(p => p.UserId);
        builder.HasOne(p => p.User)
         .WithOne()
         .HasForeignKey<Passenger>(p => p.UserId)
         .HasConstraintName("FK_Passenger_User")
         .OnDelete(DeleteBehavior.Cascade);
        builder.OwnsOne(p => p.PaymentInformation, payment =>
        {
            payment.Property(p => p.CardNumber).HasColumnName("CardNumber").IsRequired().HasMaxLength(maxLength: 32);
            payment.Property(p => p.ExpirationDate).HasColumnName("ExpirationDate").IsRequired();
            payment.Property(p => p.Cvv).HasColumnName("CVV").IsRequired().HasMaxLength(maxLength: 3);
            payment.Property(p => p.CardHolderName).HasColumnName("CardHolderName").IsRequired().HasMaxLength(maxLength: 32);
        });
        builder.Property(p => p.HasActiveRequest).IsRequired();
    }
}
