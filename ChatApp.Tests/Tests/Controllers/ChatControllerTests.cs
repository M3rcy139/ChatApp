using System.Net;
using System.Net.Http.Json;
using ChatApp.Business.DTOs.Requests;
using ChatApp.Business.DTOs.Responses;
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
using Xunit.Abstractions;

namespace ChatApp.Tests.Tests.Controllers;

[Collection("ControllerTests")]
public class ChatControllerTests : IClassFixture<ChatControllerWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ChatControllerWebApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Mock<IChatService> _chatServiceMock;

    public ChatControllerTests(ChatControllerWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
        _client = factory.CreateClient();
        _chatServiceMock = factory.ChatServiceMock;
    }

    [Fact]
    public async Task CreateChat_ShouldReturnCreatedMessage()
    {
        // Arrange
        var userId = _factory.CurrentUserId;

        var request = new CreateChatRequest
        {
            Name = ChatTestData.ChatName,
            ParticipantUserIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };

        var chat = ChatTestData.CreateChat(null);

        ChatControllerMocks.SetupCreateChat(_chatServiceMock, userId, chat, request);

        // Act
        var response = await _client.PostAsJsonAsync("/api/chats", request);
        var content = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        json.Should().ContainKey("message");
        json["message"].Should().Be(string.Format(InfoMessages.CreatedChat, chat.Id));
    }

    [Fact]
    public async Task CreateChat_ShouldReturnBadRequest_WhenUserNotFound()
    {
        // Arrange
        var request = new CreateChatRequest
        {
            Name = ChatTestData.ChatName,
            ParticipantUserIds = new List<Guid> { Guid.NewGuid() }
        };

        ChatControllerMocks.SetupCreateChatThrows(_chatServiceMock, new ArgumentNullException(ErrorMessages.UserNotFound));

        // Act
        var response = await _client.PostAsJsonAsync("api/chats", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().Contain(ErrorMessages.UserNotFound);
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
