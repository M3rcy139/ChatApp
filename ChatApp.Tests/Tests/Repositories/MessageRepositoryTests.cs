using ChatApp.DataAccess.Repositories;
using ChatApp.Domain.Models;
using ChatApp.Tests.Arrange;
using ChatApp.Tests.Base;
using ChatApp.Tests.TestData;
using FluentAssertions;
using Xunit;

namespace ChatApp.Tests.Tests.Repositories;

public class MessageRepositoryTests : RepositoryTestBase
{
    private readonly MessageRepository _repository;

    public MessageRepositoryTests()
    {
        _repository = new MessageRepository(Context);
    }

    [Fact]
    public async Task AddMessageAsync_ShouldAddMessageToContext()
    {
        // Arrange
        var message = MessageTestData.CreateMessage();

        // Act
        var result = await _repository.AddMessageAsync(message);

        // Assert
        var saved = await Context.Messages.FindAsync(result.Id);
        saved.Should().NotBeNull();
        saved!.Text.Should().Be(MessageTestData.MessageText);
    }

    [Fact]
    public async Task GetMessagesByChatIdAsync_ShouldReturnMessagesSortedBySentAtDesc()
    {
        // Arrange
        var chat = ChatTestData.CreateChat(null);
        
        await MessageRepositoryArrange.AddMessageAsync(Context, "First", chat.Id);
        await MessageRepositoryArrange.AddMessageAsync(Context, "Second", chat.Id);

        // Act
        var result = await _repository.GetMessagesByChatIdAsync(chat.Id);

        // Assert
        result.Should().HaveCount(2);
        result.First().Text.Should().Be("First");
    }

    [Fact]
    public async Task GetMessageByIdAsync_ShouldReturnMessage_IfExists()
    {
        // Arrange
        var message = await MessageRepositoryArrange.AddMessageAsync(Context);

        // Act
        var result = await _repository.GetMessageByIdAsync(message.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Text.Should().Be(MessageTestData.MessageText);
    }

    [Fact]
    public async Task UpdateMessageAsync_ShouldModifyMessageInDb()
    {
        // Arrange
        var message = await MessageRepositoryArrange.AddMessageAsync(Context);

        // Act
        message.Text = MessageTestData.UpdatedMessageText;
        await _repository.UpdateMessageAsync(message);

        // Assert
        var updated = await Context.Messages.FindAsync(message.Id);
        updated!.Text.Should().Be(MessageTestData.UpdatedMessageText);
    }

    [Fact]
    public async Task DeleteMessageAsync_ShouldRemoveMessageFromDb()
    {
        // Arrange
        var message = await MessageRepositoryArrange.AddMessageAsync(Context);

        // Act
        await _repository.DeleteMessageAsync(message);

        // Assert
        var deleted = await Context.Messages.FindAsync(message.Id);
        deleted.Should().BeNull();
    }
}
