using System.Net;
using System.Net.Http.Json;
using ChatApp.Business.DTOs.Requests;
using ChatApp.Business.Interfaces.Services;
using ChatApp.Domain.Constants;
using ChatApp.Domain.Models;
using ChatApp.Tests.Mocks;
using ChatApp.Tests.TestData;
using ChatApp.Tests.WebApplicationFactories;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace ChatApp.Tests.Tests.Controllers;

[Collection("ControllerTests")]
public class MessageControllerTests : IClassFixture<MessageControllerWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly MessageControllerWebApplicationFactory _factory;
    private readonly Mock<IMessageService> _messageServiceMock;
    private readonly Mock<IChatNotificationService> _notificationServiceMock;

    public MessageControllerTests(MessageControllerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _messageServiceMock = factory.MessageServiceMock;
        _notificationServiceMock = factory.ChatNotificationServiceMock;
    }

    [Fact]
    public async Task SendMessage_ShouldReturnSuccessMessage()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var userId = _factory.CurrentUserId;
        var now = DateTime.UtcNow;

        var request = new SendMessageRequest { Text = MessageTestData.MessageText };
        var message = MessageTestData.CreateMessage(request.Text);

        MessageControllerMocks.SetupSendMessage(_messageServiceMock, chatId, userId, message);
        MessageControllerMocks.SetupNotifyMessageSent(_notificationServiceMock, chatId, userId, request.Text, now);
        

        // Act
        var response = await _client.PostAsJsonAsync($"/api/chats/{chatId}/messages", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        content.Should().ContainKey("message");
        content!["message"].Should().Be(string.Format(InfoMessages.SentMessage, message.Id));
    }

    [Fact]
    public async Task EditMessage_ShouldReturnEditedMessage()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var userId = _factory.CurrentUserId;
        var newText = MessageTestData.EditedMessageText;

        var request = new EditMessageRequest { Text = newText };
        var message = MessageTestData.CreateEditedMessage(newText, chatId);

        MessageControllerMocks.SetupEditMessage(_messageServiceMock, chatId, userId, newText, message);
        MessageControllerMocks.SetupNotifyMessageEdited(_notificationServiceMock, chatId, message);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/chats/{chatId}/messages/{message.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        json.Should().ContainKey("message");
        json["message"].Should().Be(string.Format(InfoMessages.EditedMessage, message.Id));
    }

    [Fact]
    public async Task DeleteMessage_ShouldReturnDeletedMessage()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        var userId = _factory.CurrentUserId;

        MessageControllerMocks.SetupDeleteMessage(_messageServiceMock, chatId, userId, messageId);
        MessageControllerMocks.SetupNotifyMessageDeleted(_notificationServiceMock, chatId, messageId);

        // Act
        var response = await _client.DeleteAsync($"/api/chats/{chatId}/messages/{messageId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        json.Should().ContainKey("message");
        json["message"].Should().Be(string.Format(InfoMessages.DeletedMessage));
    }

    [Fact]
    public async Task GetMessages_ShouldReturnListOfMessages()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var messages = MessageTestData.CreateMessages();

        MessageControllerMocks.SetupGetMessagesByChatId(_messageServiceMock, chatId, 1, 50, messages);

        // Act
        var response = await _client.GetAsync($"/api/chats/{chatId}/messages");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var jsonString = await response.Content.ReadAsStringAsync();
        var responseBody = JsonConvert.DeserializeObject<List<Message>>(jsonString);
        responseBody.Should().NotBeNull();
        responseBody!.Count.Should().Be(messages.Count);
    }
    

    [Fact]
    public async Task SearchMessages_ShouldReturnFilteredMessages()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var query = MessageTestData.SearchMessage;
        var userId = _factory.CurrentUserId;
        var messages = MessageTestData.CreateMessages(query);

        MessageControllerMocks.SetupSearchMessages(_messageServiceMock, chatId, userId, query, messages);

        // Act
        var response = await _client.GetAsync($"/api/chats/{chatId}/messages/search?query={query}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<Message>>(json);
        result.Should().NotBeNull();
        result!.Count.Should().Be(messages.Count);
    }
}
