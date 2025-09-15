using Microsoft.EntityFrameworkCore;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Interfaces;
using VemQueCabe.Infra.Data.Context;

namespace VemQueCabe.Infra.Data.Repositories;

/// <summary>
/// Repository for managing driver-related data operations.
/// </summary>
public class DriverRepository : IDriverRepository
{
    private readonly AppDbContext _context;
    public DriverRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddDriver(Driver driver)
    {
        _context.Drivers.Add(driver); // Operação em memória
    }

    public void DeleteDriver(Driver driver)
    {
        _context.Drivers.Remove(driver); // Operação em memória
    }

    public async Task<IEnumerable<Driver>> GetAllDriversAsync()
    {
        return await _context.Drivers
            .Include(d => d.User) 
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Driver>> GetAvailableDriversAsync()
    {
        return await _context.Drivers
            .Include(d => d.User)
            .Where(d => d.IsAvailable)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Driver?> GetDriverByIdAsync(int userId)
    {
        return await _context.Drivers
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }

    public async Task<bool> ExistsByPlateAsync(string plate)
    {
        return await _context.Drivers
            .AsNoTracking()
            .AnyAsync(d => d.Vehicle.Plate == plate);
    }
}
