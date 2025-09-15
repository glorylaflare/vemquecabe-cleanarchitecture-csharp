using VemQueCabe.Domain.Interfaces;
using VemQueCabe.Domain.Shared;
using VemQueCabe.Infra.Data.Context;
using VemQueCabe.Infra.Data.Repositories;

namespace VemQueCabe.Infra.Data;

/// <summary>
/// Represents the Unit of Work pattern implementation, encapsulating multiple repositories and a shared database context.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Drivers = new DriverRepository(_context);
        Passengers = new PassengerRepository(_context);
        RideRequests = new RideRequestRepository(_context);
        Rides = new RideRepository(_context);
        Users = new UserRepository(_context);
    }

    public IDriverRepository Drivers { get; private set; }
    public IPassengerRepository Passengers { get; private set; }
    public IRideRequestRepository RideRequests { get; private set; }
    public IRideRepository Rides { get; private set; }
    public IUserRepository Users { get; private set; }

    public async Task<bool> CommitAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
