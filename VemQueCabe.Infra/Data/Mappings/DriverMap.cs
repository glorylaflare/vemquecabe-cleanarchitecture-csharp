using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VemQueCabe.Domain.Entities;

namespace VemQueCabe.Infra.Data.Mappings;

/// <summary>
/// Provides the Entity Framework Core configuration for the Driver entity.
/// </summary>
public class DriverMap : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.HasKey(d => d.UserId);
        builder.HasOne(d => d.User)
         .WithOne()
         .HasForeignKey<Driver>(d => d.UserId)
         .HasConstraintName("FK_Driver_User")
         .OnDelete(DeleteBehavior.Cascade);
        builder.OwnsOne(d => d.Vehicle, vehicle =>
        {
            vehicle.Property(v => v.Brand).HasColumnName("VehicleBrand").IsRequired().HasMaxLength(maxLength: 150);
            vehicle.Property(v => v.Model).HasColumnName("VehicleModel").IsRequired().HasMaxLength(maxLength: 150);
            vehicle.Property(v => v.Year).HasColumnName("VehicleYear").IsRequired();
            vehicle.Property(v => v.Color).HasColumnName("VehicleColor").IsRequired().HasMaxLength(maxLength: 150);
            vehicle.Property(v => v.Plate).HasColumnName("VehiclePlate").IsRequired().HasMaxLength(maxLength: 100);
            vehicle.HasIndex(v => v.Plate).IsUnique();
        });
    }
}
