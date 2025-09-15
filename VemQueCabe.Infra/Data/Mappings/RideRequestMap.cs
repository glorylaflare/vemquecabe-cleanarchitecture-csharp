using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VemQueCabe.Domain.Entities;

namespace VemQueCabe.Infra.Data.Mappings;

/// <summary>
/// Provides the Entity Framework Core mapping configuration for the RideRequest entity.
/// </summary>
public class RideRequestMap : IEntityTypeConfiguration<RideRequest>
{
    public void Configure(EntityTypeBuilder<RideRequest> builder)
    {
        builder.HasKey(r => r.RequestId);
        builder.HasOne(r => r.Passenger)
         .WithMany()
         .HasForeignKey(r => r.PassengerId)
         .OnDelete(DeleteBehavior.Cascade);
        builder.Property(r => r.StartLocation).IsRequired().HasMaxLength(maxLength: 150);
        builder.Property(r => r.EndLocation).IsRequired().HasMaxLength(maxLength: 150);
        builder.Property(r => r.Distance).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(r => r.UserPreferences).HasMaxLength(255);
        builder.Property(r => r.Status).IsRequired();
    }
}