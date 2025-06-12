using ChatApp.Domain.Constants;
using ChatApp.Domain.Models;
using ChatApp.Tests.Arrange;
using Moq;
using Xunit;
using ChatApp.Tests.Configurations;
using ChatApp.Tests.Mocks;
using ChatApp.Tests.TestData;

namespace ChatApp.Tests.Tests.Services;

public class MessageServiceTests : IClassFixture<MessageServiceConfiguration>
{
    private readonly MessageServiceConfiguration _configuration;

    public MessageServiceTests(MessageServiceConfiguration configuration)
    {
        _configuration = configuration;
        _configuration.ResetMocks();
    }

    [Fact]
    public async Task SendMessageAsync_ShouldSaveAndCacheMessage()
    {
        // Arrange
        var message = MessageTestData.CreateMessage();
        
        MessageServiceMocks.SetupAddMessage(_configuration.MessageRepositoryMock);

        // Act
        var result = await _configuration.Service.SendMessageAsync(message.ChatId, message.SenderId, message.Text);

        // Assert
        Assert.Equal(message.ChatId, result.ChatId);
        Assert.Equal(message.Text, result.Text);
        _configuration.MessageCacheMock.Verify(c => c.CacheMessageAsync(It.IsAny<Message>()), Times.Once);
    }

    [Fact]
    public async Task GetMessagesByChatIdAsync_ShouldReturnCachedIfPageFull()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var cachedMessages = MessageTestData.CreateCahedMessages(10);

        MessageServiceMocks.SetupGetCachedMessages(_configuration.MessageCacheMock, chatId, 1, 10, cachedMessages);

        // Act
        var result = await _configuration.Service.GetMessagesByChatIdAsync(chatId, 1, 10);

        // Assert
        Assert.Equal(10, result.Count());
        _configuration.MessageRepositoryMock.Verify(r => r.GetMessagesByChatIdAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetMessagesByChatIdAsync_ShouldFallbackToDbIfCacheMiss()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var dbMessages = MessageTestData.CreateMessages();

        MessageServiceMocks.SetupGetCachedMessages(_configuration.MessageCacheMock, chatId, 1, 10, new List<Message>());
        MessageServiceMocks.SetupGetMessagesFromDb(_configuration.MessageRepositoryMock, chatId, 1, 10, dbMessages);

        // Act
        var result = await _configuration.Service.GetMessagesByChatIdAsync(chatId, 1, 10);

        // Assert
        Assert.Single(result);
        _configuration.MessageCacheMock.Verify(c => c.CacheMessagesAsync(chatId, dbMessages), Times.Once);
    }

    [Fact]
    public async Task EditMessageAsync_ShouldUpdateTextAndCache()
    {
        // Arrange
        var message = MessageTestData.CreateMessage();

        MessageServiceMocks.SetupGetMessageById(_configuration.MessageRepositoryMock, message.Id, message);

        // Act
        var result = await _configuration.Service.EditMessageAsync(message.ChatId, message.Id, message.SenderId, "New");

        // Assert
        Assert.Equal("New", result.Text);
        _configuration.MessageRepositoryMock.Verify(r => r.UpdateMessageAsync(It.Is<Message>(m => m.Text == "New")), Times.Once);
        _configuration.MessageCacheMock.Verify(c => c.CacheMessageAsync(It.IsAny<Message>()), Times.Once);
    }

    [Fact]
    public async Task EditMessageAsync_Throws_IfMessageNotFound()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        
        MessageServiceMocks.SetupGetMessageById(_configuration.MessageRepositoryMock, messageId, null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _configuration.Service.EditMessageAsync(chatId, messageId, Guid.NewGuid(), "new text"));

        Assert.Equal(ErrorMessages.MessageNotFound, ex.Message);
    }

    [Fact]
    public async Task EditMessageAsync_Throws_IfNotOwner()
    {
        // Arrange
        var message = MessageTestData.CreateMessage();
        var anotherUser = Guid.NewGuid();

        MessageServiceMocks.SetupGetMessageById(_configuration.MessageRepositoryMock, message.Id, message);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _configuration.Service.EditMessageAsync(message.ChatId, message.Id, anotherUser, "new"));

        Assert.Equal(ErrorMessages.YouCanOnlyEditOwnMessages, ex.Message);
    }

    [Fact]
    public async Task DeleteMessageAsync_ShouldDeleteAndEvictCache()
    {
        // Arrange
        var message = MessageTestData.CreateMessage();

        MessageServiceMocks.SetupGetMessageById(_configuration.MessageRepositoryMock, message.Id, message);

        // Act
        await _configuration.Service.DeleteMessageAsync(message.ChatId, message.Id, message.SenderId);

        // Assert
        _configuration.MessageRepositoryMock.Verify(r => r.DeleteMessageAsync(message), Times.Once);
        _configuration.MessageCacheMock.Verify(c => c.DeleteMessageFromCacheAsync(message.ChatId, message.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteMessageAsync_Throws_IfNotOwner()
    {
        // Arrange
        var message = MessageTestData.CreateMessage();

        var anotherUser = Guid.NewGuid();

        MessageServiceMocks.SetupGetMessageById(_configuration.MessageRepositoryMock, message.Id, message);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _configuration.Service.DeleteMessageAsync(message.ChatId, message.Id, anotherUser));

        Assert.Equal(ErrorMessages.YouCanOnlyEditOwnMessages, ex.Message);
    }
    
    [Fact]
    public async Task DeleteMessageAsync_Throws_IfMessageNotFound()
    {
        // Arrange
        var message = MessageTestData.CreateMessage();

        MessageServiceMocks.SetupGetMessageByIdThrows(_configuration.MessageRepositoryMock, message.Id);
        _configuration.MessageRepositoryMock.Setup(r => r.GetMessageByIdAsync(Guid.NewGuid()))
            .ThrowsAsync(new ArgumentException(ErrorMessages.MessageNotFound));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _configuration.Service.DeleteMessageAsync(message.ChatId, message.Id, message.SenderId));
    }

    [Fact]
    public async Task SearchMessagesAsync_ShouldCallRepoWithTsQuery()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = MessageTestData.SearchQuery;
        var tsQuery = MessageTestData.TsSearchQuery;
        var expected = MessageTestData.CreateMessages(MessageTestData.SearchMessage);

        MessageServiceMocks.SetupSearchMessages(_configuration.MessageRepositoryMock, chatId, tsQuery, expected);

        // Act
        var result = await _configuration.Service.SearchMessagesAsync(chatId, userId, query);

        // Assert
        Assert.Single(result);
        Assert.Contains(MessageTestData.SearchQuery, result.First().Text);
    }
}
