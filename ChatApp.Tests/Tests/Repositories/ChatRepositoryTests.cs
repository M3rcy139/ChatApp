using ChatApp.DataAccess.Repositories;
using ChatApp.Domain.Models;
using ChatApp.Tests.Arrange;
using ChatApp.Tests.TestData;
using ChatApp.Tests.Base;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ChatApp.Tests.Tests.Repositories;

public class ChatRepositoryTests : RepositoryTestBase
{
    private readonly ChatRepository _repository;

    public ChatRepositoryTests()
    {
        _repository = new ChatRepository(Context);
    }

    [Fact]
    public async Task CreateChatAsync_ShouldAddChatToContext()
    {
        // Arrange
        var user1 = await UserRepositoryArrange.AddUserAsync(Context);
        var user2 = await UserRepositoryArrange.AddUserAsync(Context);

        var chat = ChatTestData.CreateChat(new List<User> { user1, user2 });

        // Act
        var result = await _repository.CreateChatAsync(chat);

        // Assert
        var saved = await Context.Chats
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.Id == result.Id);

        saved.Should().NotBeNull();
        saved!.Users.Should().HaveCount(2);
        saved.Name.Should().Be(ChatTestData.ChatName);
    }

    [Fact]
    public async Task GetChatsByUserIdAsync_ShouldReturnChats_IfUserIsParticipant()
    {
        // Arrange
        var user = await UserRepositoryArrange.AddUserAsync(Context);
        var chat = await ChatRepositoryArrange.AddChatWithUsersAsync(Context, new List<User> { user });

        // Act
        var result = await _repository.GetChatsByUserIdAsync(user.Id);

        // Assert
        result.Should().ContainSingle(c => c.Id == chat.Id);
    }

    [Fact]
    public async Task GetChatsByUserIdAsync_ShouldReturnEmpty_IfUserNotInAnyChat()
    {
        // Arrange
        var user = await UserRepositoryArrange.AddUserAsync(Context);

        // Act
        var result = await _repository.GetChatsByUserIdAsync(user.Id);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetChatByIdAsync_ShouldReturnChatWithUsers_IfExists()
    {
        // Arrange
        var user = await UserRepositoryArrange.AddUserAsync(Context);
        var chat = await ChatRepositoryArrange.AddChatWithUsersAsync(Context, new List<User> { user });

        // Act
        var result = await _repository.GetChatByIdAsync(chat.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Users.Should().ContainSingle(u => u.Id == user.Id);
    }

    [Fact]
    public async Task GetChatByIdAsync_ShouldReturnNull_IfChatDoesNotExist()
    {
        // Act
        var result = await _repository.GetChatByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }
}
