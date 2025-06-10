using ChatApp.Domain.Models;

namespace ChatApp.DataAccess.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<Guid> userIds);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<bool> UserNameExistsAsync(string userName);
    Task<bool> UserPhoneNumberExistsAsync(string phoneNumber);
}