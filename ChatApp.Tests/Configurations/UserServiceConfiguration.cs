using ChatApp.Business.Interfaces.Authentication;
using ChatApp.Business.Services;
using ChatApp.DataAccess.Interfaces;
using Moq;

namespace ChatApp.Tests.Configurations;

public class UserServiceConfiguration
{
    public Mock<IUserRepository> UserRepoMock { get; }
    public Mock<IPasswordHasher> PasswordHasherMock { get; }
    public Mock<IJwtProvider> JwtProviderMock { get; }
    
    public UserService Service { get; }
    
    public UserServiceConfiguration()
    {
        UserRepoMock = new Mock<IUserRepository>();
        PasswordHasherMock = new Mock<IPasswordHasher>();
        JwtProviderMock = new Mock<IJwtProvider>();

        Service = new UserService(
            UserRepoMock.Object,
            PasswordHasherMock.Object,
            JwtProviderMock.Object
        );
    }

    public void ResetMocks()
    {
        UserRepoMock.Invocations.Clear();
        PasswordHasherMock.Invocations.Clear();
        JwtProviderMock.Invocations.Clear();
    }
}