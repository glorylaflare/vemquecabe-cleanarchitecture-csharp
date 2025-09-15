using Microsoft.EntityFrameworkCore;
using VemQueCabe.Domain.Entities;

namespace VemQueCabe.Infra.Data.Context;

/// <summary>
/// Represents the application's database context, providing access to entity sets and configuration.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Passenger> Passengers { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<RideRequest> RideRequests { get; set; }
    public DbSet<Ride> Rides { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
