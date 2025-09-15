using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VemQueCabe.Domain.Entities;

namespace VemQueCabe.Infra.Data.Mappings;

/// <summary>
/// Provides the Entity Framework Core configuration for the Ride entity.
/// </summary>
public class RideMap : IEntityTypeConfiguration<Ride>
{
    public void Configure(EntityTypeBuilder<Ride> builder)
    {
        builder.HasKey(r => r.RideId);
        builder.HasOne(r => r.RideRequest)
         .WithOne()
         .HasForeignKey<Ride>(r => r.RideRequestId)
         .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(r => r.Driver)
         .WithMany()
         .HasForeignKey(r => r.DriverId)
         .OnDelete(DeleteBehavior.Restrict);
        builder.Property(r => r.StartTime).IsRequired();
        builder.Property(r => r.EndTime);
        builder.OwnsOne(r => r.Fare, fare =>
        {
            fare.Property(f => f.BaseFare).HasColumnName("FareAmount").IsRequired().HasColumnType("decimal(18,2)");
            fare.Property(f => f.PricePerKilometer).HasColumnName("PricePerKilometer").IsRequired().HasColumnType("decimal(18,2)");
            fare.Property(f => f.SurgeMultiplier).HasColumnName("SurgeMultiplier").IsRequired().HasColumnType("decimal(18,2)");
            fare.Property(f => f.Total).HasColumnName("TotalFare").IsRequired().HasColumnType("decimal(18,2)");
        });
    }
}
