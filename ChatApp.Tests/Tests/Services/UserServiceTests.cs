using System.Security.Authentication;
using ChatApp.Business.Interfaces.Authentication;
using ChatApp.Business.Services;
using ChatApp.DataAccess.Interfaces;
using ChatApp.Domain.Constants;
using ChatApp.Domain.Models;
using ChatApp.Tests.Arrange;
using ChatApp.Tests.Configurations;
using ChatApp.Tests.Mocks;
using ChatApp.Tests.TestData;
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
        var username = UserTestData.ValidUsername;
        var phone = UserTestData.ValidPhone;
        var password = UserTestData.ValidPassword;
        var hashedPassword = UserTestData.HashedPassword;

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
        var username = UserTestData.DuplicateUsername;
        
        UserServiceMocks.SetupUserNameExists(_configuration.UserRepoMock, username, true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _configuration.Service.Register(username, UserTestData.ValidPhone, UserTestData.ValidPassword));

        Assert.Equal(ErrorMessages.AlreadyExistsUserName, ex.Message);
    }

    [Fact]
    public async Task Register_ShouldThrow_WhenPhoneNumberAlreadyExists()
    {
        // Arrange
        var username = UserTestData.ValidUsername;
        var phone = UserTestData.DuplicatePhone;

        UserServiceMocks.SetupUserNameExists(_configuration.UserRepoMock, username, false);
        UserServiceMocks.SetupUserPhoneExists(_configuration.UserRepoMock, phone, true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _configuration.Service.Register(username, phone, UserTestData.ValidPassword));

        Assert.Equal(ErrorMessages.AlreadyExistsPhoneNumber, ex.Message);
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var phone = UserTestData.ValidPhone;
        var password = UserTestData.ValidPassword;
        var hashed = UserTestData.HashedPassword;

        var user = UserTestData.CreateUser();

        UserServiceMocks.SetupGetUserByPhone(_configuration.UserRepoMock, phone, user);
        UserServiceMocks.SetupPasswordVerify(_configuration.PasswordHasherMock, password, hashed, true);
        UserServiceMocks.SetupGenerateToken(_configuration.JwtProviderMock, user, UserTestData.JwtToken);

        // Act
        var token = await _configuration.Service.Login(phone, password);

        // Assert
        Assert.Equal(UserTestData.JwtToken, token);
    }

    [Fact]
    public async Task Login_ShouldThrow_WhenPasswordInvalid()
    {
        // Arrange
        var phone = UserTestData.ValidPhone;
        var hashed = UserTestData.HashedPassword;

        var user = UserTestData.CreateUser();

        UserServiceMocks.SetupGetUserByPhone(_configuration.UserRepoMock, phone, user);
        UserServiceMocks.SetupPasswordVerify(_configuration.PasswordHasherMock, UserTestData.WrongPassword, 
            hashed, false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthenticationException>(() =>
            _configuration.Service.Login(phone, UserTestData.WrongPassword));

        Assert.Equal(ErrorMessages.FailedToLogin, ex.Message);
    }

    [Fact]
    public async Task Login_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        var phone = UserTestData.UnknownPhone;

        UserServiceMocks.SetupGetUserByPhone(_configuration.UserRepoMock, phone, null);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            _configuration.Service.Login(phone, UserTestData.ValidPassword));
    }
}
