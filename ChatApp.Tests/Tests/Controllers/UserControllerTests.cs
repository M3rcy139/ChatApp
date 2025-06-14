using System.Net;
using System.Net.Http.Json;
using ChatApp.Business.Interfaces.Services;
using ChatApp.Domain.Constants;
using ChatApp.Tests.TestData;
using ChatApp.Tests.WebApplicationFactories;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChatApp.Tests.Tests.Controllers;
    
public class UserControllerTests : IClassFixture<UserControllerWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserControllerWebApplicationFactory _factory;

    public UserControllerTests(UserControllerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _userServiceMock = factory.UserServiceMock;
    }

    [Fact]
    public async Task Register_ShouldReturnOkAndCallUserService()
    {
        // Arrange
        var request = UserDtoTestData.CreateRegisterUserRequest();

        _userServiceMock
            .Setup(s => s.Register(request.UserName, request.PhoneNumber, request.Password))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        json.Should().NotBeNull();
        json!["message"].ToString().Should().Be(InfoMessages.SuccessfulRegistration);

        _userServiceMock.Verify(s =>
            s.Register(request.UserName, request.PhoneNumber, request.Password), Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnOkSetCookieAndCallUserService()
    {
        // Arrange
        var request = UserDtoTestData.CreateLoginUserRequest();

        var expectedToken = UserDtoTestData.JwtToken;

        _userServiceMock
            .Setup(s => s.Login(request.PhoneNumber, request.Password))
            .ReturnsAsync(expectedToken);

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        json.Should().NotBeNull();
        json!["message"].ToString().Should().Be(InfoMessages.SuccessfulLogin);
        
        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies.Should().Contain(c => c.StartsWith("token="));

        _userServiceMock.Verify(s =>
            s.Login(request.PhoneNumber, request.Password), Times.Once);
    }
}
