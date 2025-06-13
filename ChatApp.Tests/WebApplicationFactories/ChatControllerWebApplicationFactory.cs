using ChatApp.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ChatApp.Tests.WebApplicationFactories;

public class ChatControllerWebApplicationFactory : BaseWebApplicationFactory<Program>
{
    public Guid CurrentUserId { get; } = Guid.NewGuid();
    public Mock<IChatService> ChatServiceMock { get; } = new();

    public ChatControllerWebApplicationFactory()
    {
        TestAuthHandler.CurrentUserId = CurrentUserId;
    }

    protected override void ConfigureMocks(IServiceCollection services)
    {
        var chatServiceDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(IChatService));
        if (chatServiceDescriptor != null)
            services.Remove(chatServiceDescriptor);

        services.AddSingleton(ChatServiceMock.Object);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
    }
}