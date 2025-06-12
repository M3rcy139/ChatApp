using System.Security.Authentication;
using ChatApp.Business.Interfaces.Authentication;
using ChatApp.Business.Services;
using ChatApp.DataAccess.Interfaces;
using ChatApp.Domain.Constants;
using ChatApp.Domain.Models;
using ChatApp.Tests.Arrange;
using ChatApp.Tests.Configurations;
using ChatApp.Tests.Mocks;
using Moq;
using Xunit;

namespace ChatApp.Tests.Tests.Services;

public class UserServiceTests : IClassFixture<UserServiceConfiguration>
{
    private readonly UserServiceConfiguration _configuration;

    public UserServiceTests(UserServiceConfiguration configuration)
    {
        _configuration = configuration;
        _configuration.ResetMocks();
    }

    [Fact]
    public async Task Register_ShouldAddNewUser_WhenUserDoesNotExist()
    {
        // Arrange
        var username = UserServiceTestData.ValidUsername;
        var phone = UserServiceTestData.ValidPhone;
        var password = UserServiceTestData.ValidPassword;
        var hashedPassword = UserServiceTestData.HashedPassword;

        UserServiceMocks.SetupUserNameExists(_configuration.UserRepoMock, username, false);
        UserServiceMocks.SetupUserPhoneExists(_configuration.UserRepoMock, phone, false);
        UserServiceMocks.SetupPasswordHash(_configuration.PasswordHasherMock, password, hashedPassword);

        // Act
        await _configuration.Service.Register(username, phone, password);

        // Assert
        _configuration.UserRepoMock.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.UserName == username &&
            u.PhoneNumber == phone &&
            u.PasswordHash == hashedPassword
        )), Times.Once);
    }

    [Fact]
    public async Task Register_ShouldThrow_WhenUserNameAlreadyExists()
    {
        // Arrange
        var username = UserServiceTestData.DuplicateUsername;
        
        UserServiceMocks.SetupUserNameExists(_configuration.UserRepoMock, username, true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _configuration.Service.Register(username, UserServiceTestData.ValidPhone, UserServiceTestData.ValidPassword));

        Assert.Equal(ErrorMessages.AlreadyExistsUserName, ex.Message);
    }

    [Fact]
    public async Task Register_ShouldThrow_WhenPhoneNumberAlreadyExists()
    {
        // Arrange
        var username = UserServiceTestData.ValidUsername;
        var phone = UserServiceTestData.DuplicatePhone;

        UserServiceMocks.SetupUserNameExists(_configuration.UserRepoMock, username, false);
        UserServiceMocks.SetupUserPhoneExists(_configuration.UserRepoMock, phone, true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _configuration.Service.Register(username, phone, UserServiceTestData.ValidPassword));

        Assert.Equal(ErrorMessages.AlreadyExistsPhoneNumber, ex.Message);
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var phone = UserServiceTestData.ValidPhone;
        var password = UserServiceTestData.ValidPassword;
        var hashed = UserServiceTestData.HashedPassword;

        var user = UserServiceTestData.CreateUser();

        UserServiceMocks.SetupGetUserByPhone(_configuration.UserRepoMock, phone, user);
        UserServiceMocks.SetupPasswordVerify(_configuration.PasswordHasherMock, password, hashed, true);
        UserServiceMocks.SetupGenerateToken(_configuration.JwtProviderMock, user, UserServiceTestData.JwtToken);

        // Act
        var token = await _configuration.Service.Login(phone, password);

        // Assert
        Assert.Equal(UserServiceTestData.JwtToken, token);
    }

    [Fact]
    public async Task Login_ShouldThrow_WhenPasswordInvalid()
    {
        // Arrange
        var phone = UserServiceTestData.ValidPhone;
        var hashed = UserServiceTestData.HashedPassword;

        var user = UserServiceTestData.CreateUser();

        UserServiceMocks.SetupGetUserByPhone(_configuration.UserRepoMock, phone, user);
        UserServiceMocks.SetupPasswordVerify(_configuration.PasswordHasherMock, UserServiceTestData.WrongPassword, 
            hashed, false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthenticationException>(() =>
            _configuration.Service.Login(phone, UserServiceTestData.WrongPassword));

        Assert.Equal(ErrorMessages.FailedToLogin, ex.Message);
    }

    [Fact]
    public async Task Login_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        var phone = UserServiceTestData.UnknownPhone;

        UserServiceMocks.SetupGetUserByPhone(_configuration.UserRepoMock, phone, null);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            _configuration.Service.Login(phone, UserServiceTestData.ValidPassword));
    }
}
