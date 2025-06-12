using ChatApp.Business.Interfaces.Authentication;
using ChatApp.DataAccess.Interfaces;
using ChatApp.Domain.Models;
using Moq;
using System;

namespace ChatApp.Tests.Mocks;

public static class UserServiceMocks
{
    public static void SetupUserNameExists(Mock<IUserRepository> userRepoMock, string username, bool exists)
    {
        userRepoMock.Setup(r => r.UserNameExistsAsync(username))
            .ReturnsAsync(exists);
    }

    public static void SetupUserPhoneExists(Mock<IUserRepository> userRepoMock, string phone, bool exists)
    {
        userRepoMock.Setup(r => r.UserPhoneNumberExistsAsync(phone))
            .ReturnsAsync(exists);
    }

    public static void SetupAddUser(Mock<IUserRepository> userRepoMock)
    {
        userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);
    }

    public static void SetupGetUserByPhone(Mock<IUserRepository> userRepoMock, string phone, User? user)
    {
        userRepoMock.Setup(r => r.GetUserByPhoneNumberAsync(phone))
            .ReturnsAsync(user);
    }

    public static void SetupPasswordHash(Mock<IPasswordHasher> hasherMock, string password, byte[] hash)
    {
        hasherMock.Setup(h => h.Generate(password))
            .Returns(hash);
    }

    public static void SetupPasswordVerify(Mock<IPasswordHasher> hasherMock, string password, byte[] hash, bool result)
    {
        hasherMock.Setup(h => h.Verify(password, hash))
            .Returns(result);
    }

    public static void SetupGenerateToken(Mock<IJwtProvider> jwtProviderMock, User user, string token)
    {
        jwtProviderMock.Setup(p => p.Generate(user))
            .Returns(token);
    }
}