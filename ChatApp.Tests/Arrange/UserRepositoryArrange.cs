using ChatApp.DataAccess;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestData;

namespace ChatApp.Tests.Arrange;

public static class UserRepositoryArrange
{
    public static async Task<User> AddUserAsync(ChatAppDbContext context)
    {
        var user = UserTestData.CreateUser();
        
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }
}