using System.Net;
using System.Net.Http.Json;
using ChatApp.Business.DTOs.Requests;
using ChatApp.Business.DTOs.Responses;
using ChatApp.Business.Interfaces.Services;
using ChatApp.Domain.Constants;
using ChatApp.Tests.Mocks;
using ChatApp.Tests.TestData;
using ChatApp.Tests.WebApplicationFactories;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace ChatApp.Tests.Tests.Controllers;

public class ChatControllerTests : IClassFixture<ChatControllerWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ChatControllerWebApplicationFactory _factory;
    private readonly Mock<IChatService> _chatServiceMock;

    public ChatControllerTests(ChatControllerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _chatServiceMock = factory.ChatServiceMock;
    }

    [Fact]
    public async Task CreateChat_ShouldReturnCreatedMessage()
    {
        // Arrange
        var userId = _factory.CurrentUserId;
        var chatId = Guid.NewGuid();

        var request = new CreateChatRequest
        {
            Name = ChatTestData.ChatName,
            ParticipantUserIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };

        ChatControllerMocks.SetupCreateChat(_chatServiceMock, userId, chatId, request);

        // Act
        var response = await _client.PostAsJsonAsync("/api/chats", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        json.Should().ContainKey("message");
        json["message"].Should().Be(string.Format(InfoMessages.CreatedChat, chatId));
    }

    [Fact]
    public async Task GetChats_ShouldReturnUserChats()
    {
        // Arrange
        var userId = _factory.CurrentUserId;
        var chats = ChatTestData.CreateChats();

        ChatControllerMocks.SetupGetChat(_chatServiceMock, userId, chats);

        // Act
        var response = await _client.GetAsync("/api/chats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var jsonString = await response.Content.ReadAsStringAsync();
        var responseBody = JsonConvert.DeserializeObject<List<UserChatsReponse>>(jsonString);
        responseBody.Should().NotBeNull();
    }
}
