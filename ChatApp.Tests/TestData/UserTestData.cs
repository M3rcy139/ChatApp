using ChatApp.Domain.Models;

namespace ChatApp.Tests.TestData;

public static class UserTestData
{
    public const string ValidUsername = "testuser";
    public const string DuplicateUsername = "duplicateUser";
    public const string UnknownUsername = "unknownUser";

    public const string ValidPhone = "1234567890";
    public const string DuplicatePhone = "duplicatedPhone";
    public const string UnknownPhone = "unknown";

    public const string ValidPassword = "password";
    public const string WrongPassword = "wrongpass";

    public static byte[] HashedPassword => new byte[] { 1, 2, 3, 4 };

    public const string JwtToken = "token";
    
    public static User CreateUser() => new User
    {
        Id = Guid.NewGuid(),
        UserName = ValidUsername,
        PhoneNumber = ValidPhone,
        PasswordHash = HashedPassword
    };
    
    public static List<User> CreateUsers(List<Guid> ids)
    {
        return ids
            .Select(id => new User { Id = id })
            .ToList();
    }
}