using ChatApp.DataAccess.Repositories;
using ChatApp.Tests.Arrange;
using ChatApp.Tests.Base;
using ChatApp.Tests.TestData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ChatApp.Tests.Tests.Repositories;

public class UserRepositoryTests : RepositoryTestBase
{
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        _repository = new UserRepository(Context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUserToContext()
    {
        // Arrange
        var user = UserTestData.CreateUser();

        // Act
        await _repository.AddAsync(user);

        // Assert
        var savedUser = await Context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        savedUser.Should().NotBeNull();
        savedUser!.PhoneNumber.Should().Be(user.PhoneNumber);
    }

    [Fact]
    public async Task GetUserByPhoneNumberAsync_ShouldReturnUser_IfExists()
    {
        // Arrange
        var user = await UserRepositoryArrange.AddUserAsync(Context);

        // Act
        var result = await _repository.GetUserByPhoneNumberAsync(user.PhoneNumber);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetUserByPhoneNumberAsync_ShouldReturnNull_IfNotExists()
    {
        // Act
        var result = await _repository.GetUserByPhoneNumberAsync(UserTestData.UnknownPhone);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUsersByIdsAsync_ShouldReturnCorrectUsers()
    {
        // Arrange
        var user1 = await UserRepositoryArrange.AddUserAsync(Context);
        var user2 = await UserRepositoryArrange.AddUserAsync(Context);

        // Act
        var result = await _repository.GetUsersByIdsAsync(new[] { user1.Id, user2.Id });

        // Assert
        result.Should().HaveCount(2);
        result.Select(u => u.Id).Should().Contain(new[] { user1.Id, user2.Id });
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_IfExists()
    {
        // Arrange
        var user = await UserRepositoryArrange.AddUserAsync(Context);

        // Act
        var result = await _repository.GetUserByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNull_IfNotExists()
    {
        // Act
        var result = await _repository.GetUserByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UserNameExistsAsync_ShouldReturnTrue_IfUserNameExists()
    {
        // Arrange
        var user = await UserRepositoryArrange.AddUserAsync(Context);

        // Act
        var result = await _repository.UserNameExistsAsync(user.UserName);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UserNameExistsAsync_ShouldReturnFalse_IfUserNameDoesNotExist()
    {
        // Act
        var result = await _repository.UserNameExistsAsync(UserTestData.UnknownUsername);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UserPhoneNumberExistsAsync_ShouldReturnTrue_IfPhoneNumberExists()
    {
        // Arrange
        var user = await UserRepositoryArrange.AddUserAsync(Context);

        // Act
        var result = await _repository.UserPhoneNumberExistsAsync(user.PhoneNumber);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UserPhoneNumberExistsAsync_ShouldReturnFalse_IfPhoneNumberDoesNotExist()
    {
        // Act
        var result = await _repository.UserPhoneNumberExistsAsync(UserTestData.UnknownPhone);

        // Assert
        result.Should().BeFalse();
    }
}
