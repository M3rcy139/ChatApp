using ChatApp.Business.DTOs.Requests;

namespace ChatApp.Tests.TestData;

public class UserDtoTestData
{
    public const string UserName = "testuser";
    public const string PhoneNumber = "+70000000000";
    public const string Password = "password123";

    public const string JwtToken = "token";

    public static RegisterUserRequest CreateRegisterUserRequest() => new()
    {
        UserName = UserName,
        PhoneNumber = PhoneNumber,
        Password = Password,
    };
    
    public static LoginUserRequest CreateLoginUserRequest() => new()
    {
        PhoneNumber = PhoneNumber,
        Password = Password,
    };
}