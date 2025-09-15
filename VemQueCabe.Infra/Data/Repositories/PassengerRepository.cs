using Microsoft.EntityFrameworkCore;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Interfaces;
using VemQueCabe.Infra.Data.Context;

namespace VemQueCabe.Infra.Data.Repositories;

/// <summary>
/// Repository for managing passenger data in the database.
/// </summary>
public class PassengerRepository : IPassengerRepository
{
    private readonly AppDbContext _context;
    public PassengerRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddPassenger(Passenger passenger)
    {
        _context.Passengers.Add(passenger); // Operação em memória
    }

    public void DeletePassenger(Passenger passenger)
    {
        _context.Passengers.Remove(passenger); // Operação em memória
    }

    public async Task<IEnumerable<Passenger>> GetAllPassengersAsync()
    {
        return await _context.Passengers
            .Include(p => p.User)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Passenger?> GetPassengerByIdAsync(int id)
    {
        return await _context.Passengers
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == id);
    }
}
