using ChatApp.Business.Interfaces.Cache;
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

public class ChatServiceTests : IClassFixture<ChatServiceConfiguration>
{
    private readonly ChatServiceConfiguration _configuration;

    public ChatServiceTests(ChatServiceConfiguration configuration)
    {
        _configuration = configuration;
        _configuration.ResetMocks();
    }

    [Fact]
    public async Task CreateChatAsync_CreatesChatAndInvalidatesCache()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var participantIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var allUserIds = participantIds.Append(creatorId).Distinct().ToList();

        var users = UserServiceTestData.CreateUsers(allUserIds);
        var createdChat = ChatServiceTestData.CreateChat(users);

        foreach (var user in users)
        {
            ChatServiceMocks.SetupGetUserById(_configuration.UserRepoMock, user.Id, user);
        }

        ChatServiceMocks.SetupCreateChat(_configuration.ChatRepoMock, createdChat);

        // Act
        var result = await _configuration.Service.CreateChatAsync(creatorId, ChatServiceTestData.ChatName, participantIds);

        // Assert
        Assert.Equal(createdChat, result);
        foreach (var user in users)
        {
            _configuration.ChatCacheMock.Verify(c => c.InvalidateUserChatsCacheAsync(user.Id), Times.Once);
        }
    }

    [Fact]
    public async Task CreateChatAsync_ThrowsIfUserDoesNotExist()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var missingUserId = Guid.NewGuid(); 

        var participantIds = new List<Guid> { missingUserId };
        var allUserIds = participantIds.Append(creatorId).Distinct().ToList();
        
        ChatServiceMocks.SetupGetUserById(_configuration.UserRepoMock, creatorId, new User { Id = creatorId });
        ChatServiceMocks.SetupGetUserById(_configuration.UserRepoMock, missingUserId, null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _configuration.Service.CreateChatAsync(creatorId, ChatServiceTestData.ChatName, participantIds));

        Assert.Equal(ErrorMessages.UserNotFound, exception.Message);
        
        _configuration.ChatRepoMock.Verify(r => r.CreateChatAsync(It.IsAny<Chat>()), Times.Never);
        
        _configuration.ChatCacheMock.Verify(c => c.InvalidateUserChatsCacheAsync(It.IsAny<Guid>()), Times.Never);
    }
    
    [Fact]
    public async Task GetUserChatsAsync_ReturnsFromCache_IfExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cachedChats = ChatServiceTestData.CreateChats();

        ChatServiceMocks.SetupGetCachedUserChats(_configuration.ChatCacheMock, userId, cachedChats);

        // Act
        var result = await _configuration.Service.GetUserChatsAsync(userId);

        // Assert
        Assert.Equal(cachedChats, result);
        _configuration.ChatRepoMock.Verify(r => r.GetChatsByUserIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetUserChatsAsync_FetchesFromRepo_IfCacheMiss()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var repoChats = ChatServiceTestData.CreateChats();

        ChatServiceMocks.SetupGetCachedUserChats(_configuration.ChatCacheMock, userId, null);

        ChatServiceMocks.SetupGetChatsByUserId(_configuration.ChatRepoMock, userId, repoChats);

        // Act
        var result = await _configuration.Service.GetUserChatsAsync(userId);

        // Assert
        Assert.Equal(repoChats, result);
        _configuration.ChatCacheMock.Verify(c => c.CacheUserChatsAsync(userId, repoChats), Times.Once);
    }

    [Fact]
    public async Task EnsureUserIsParticipantAsync_ThrowsIfChatNotFound()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        
        ChatServiceMocks.SetupGetChatById(_configuration.ChatRepoMock, chatId, null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _configuration.Service.EnsureUserIsParticipantAsync(chatId, Guid.NewGuid()));

        Assert.Equal(ErrorMessages.ChatNotFound, ex.Message);
    }

    [Fact]
    public async Task EnsureUserIsParticipantAsync_ThrowsIfNotParticipant()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var chat = ChatServiceTestData.CreateChat(new List<User> { new() { Id = Guid.NewGuid() } });

        ChatServiceMocks.SetupGetChatById(_configuration.ChatRepoMock, chatId, chat);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _configuration.Service.EnsureUserIsParticipantAsync(chatId, userId));

        Assert.Equal(ErrorMessages.NotParticipantOfChat, ex.Message);
    }

    [Fact]
    public async Task EnsureUserIsParticipantAsync_DoesNotThrow_IfUserInChat()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var chat = ChatServiceTestData.CreateChat(new List<User> { new() { Id = userId } });

        ChatServiceMocks.SetupGetChatById(_configuration.ChatRepoMock, chatId, chat);

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _configuration.Service.EnsureUserIsParticipantAsync(chatId, userId));

        // Assert
        Assert.Null(exception);
    }
}
