using ChatApp.DataAccess.Interfaces;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ChatAppDbContext _context;
    public UserRepository(ChatAppDbContext context) => _context = context;

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

        return user;
    }

    public async Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<Guid> userIds)
    {
        return await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }
    
    public async Task<bool> UserNameExistsAsync(string userName)
    {
        return await _context.Users
            .AnyAsync(u => u.UserName == userName);
    }
    
    public async Task<bool> UserPhoneNumberExistsAsync(string phoneNumber)
    {
        return await _context.Users
            .AnyAsync(u => u.PhoneNumber == phoneNumber);
    }
}