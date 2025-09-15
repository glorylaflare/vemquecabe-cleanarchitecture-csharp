using Microsoft.EntityFrameworkCore;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Interfaces;
using VemQueCabe.Infra.Data.Context;

namespace VemQueCabe.Infra.Data.Repositories;

/// <summary>
/// Repository for managing user data operations in the database.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == id);
    }

    public void UpdateUser(User user)
    {
        _context.Users.Update(user);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Address == email);
    }
    
    public async Task<User?> GetUserByRefreshToken(string token)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Token.RefreshToken == token);
    }

    public void CreateUser(User user)
    {
        _context.Users.Add(user); // Operação em memória
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email.Address == email);
    }
}
