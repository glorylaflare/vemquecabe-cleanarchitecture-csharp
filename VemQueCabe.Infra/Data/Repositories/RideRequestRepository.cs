using Microsoft.EntityFrameworkCore;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Enums;
using VemQueCabe.Domain.Interfaces;
using VemQueCabe.Infra.Data.Context;

namespace VemQueCabe.Infra.Data.Repositories;

/// <summary>
/// Repository for managing ride request data operations.
/// </summary>
public class RideRequestRepository : IRideRequestRepository
{
    private readonly AppDbContext _context;
    public RideRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddRequest(RideRequest rideRequest)
    {
        _context.RideRequests.Add(rideRequest); // Operação em memória
    }

    public void DeleteRequest(RideRequest request)
    {
        _context.RideRequests.Remove(request); // Operação em memória
    }

    public async Task<IEnumerable<RideRequest>> GetAllRequestsAsync()
    {
        return await _context.RideRequests
            .Include(r => r.Passenger)
            .ThenInclude(p => p.User)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<RideRequest?> GetRequestByIdAsync(int requestId)
    {
        return await _context.RideRequests
            .Include(r => r.Passenger)
            .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(r => r.RequestId == requestId);
    }

    public async Task<IEnumerable<RideRequest>> GetRequestsByPassengerIdAsync(int passengerId)
    {
        return await _context.RideRequests
            .Include(r => r.Passenger)
            .ThenInclude(p => p.User)
            .Where(r => r.PassengerId == passengerId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<RideRequest>> GetRequestsByStatusAsync(Status status)
    {
        return await _context.RideRequests
            .Include(r => r.Passenger)
            .ThenInclude(p => p.User)
            .Where(r => r.Status == status)
            .AsNoTracking()
            .ToListAsync();
    }

    public void UpdateRequest(RideRequest rideRequest)
    {
        _context.RideRequests.Update(rideRequest); // Operação em memória
    }
}
