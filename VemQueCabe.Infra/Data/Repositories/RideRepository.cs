using Microsoft.EntityFrameworkCore;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Interfaces;
using VemQueCabe.Infra.Data.Context;

namespace VemQueCabe.Infra.Data.Repositories;

/// <summary>
/// Repository for managing ride-related data operations.
/// </summary>
public class RideRepository : IRideRepository
{
    private readonly AppDbContext _context;
    public RideRepository(AppDbContext context)
    {
        _context = context;
    }

    public void CreateRide(Ride ride)
    {
        _context.Rides.Add(ride); // Operação em memória
    }

    public void DeleteRide(Ride ride)
    {
        _context.Rides.Remove(ride); // Operação em memória
    }

    public async Task<IEnumerable<Ride>> GetAllRidesAsync()
    {
        return await _context.Rides
            .Include(r => r.Driver)
            .ThenInclude(p => p.User)
            .Include(r => r.RideRequest)
            .ThenInclude(p => p.Passenger)
            .ThenInclude(p => p.User)
            .ToListAsync();
    }

    public async Task<Ride?> GetRideByIdAsync(int rideId)
    {
        return await _context.Rides
            .Include(r => r.Driver)
            .ThenInclude(p => p.User)
            .Include(r => r.RideRequest)
            .ThenInclude(p => p.Passenger)
            .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(r => r.RideId == rideId);
    }

    public async Task<IEnumerable<Ride>> GetRidesByDriverIdAsync(int driverId)
    {
        return await _context.Rides
            .Include(r => r.RideRequest)
            .ThenInclude(p => p.Passenger)
            .ThenInclude(p => p.User)
            .Where(r => r.DriverId == driverId)
            .ToListAsync();
    }

    public void UpdateRide(Ride ride)
    {
        _context.Rides.Update(ride); // Operação em memória
    }
}
