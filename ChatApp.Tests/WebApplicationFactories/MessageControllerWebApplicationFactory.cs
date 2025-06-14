using ChatApp.Business.Interfaces.Services;
using ChatApp.Tests.Fakes.Providers;
using ChatApp.Tests.Fakes.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ChatApp.Tests.WebApplicationFactories;

public class MessageControllerWebApplicationFactory : BaseWebApplicationFactory<Program>
{
    public Guid CurrentUserId { get; } = Guid.NewGuid();
    public Mock<IMessageService> MessageServiceMock { get; } = new();
    public Mock<IChatNotificationService> ChatNotificationServiceMock { get; } = new();
    public Mock<IChatService> ChatServiceMock { get; } = new();

    protected override void ConfigureMocks(IServiceCollection services)
    {
        var messageServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMessageService));
        if (messageServiceDescriptor != null)
            services.Remove(messageServiceDescriptor);

        var notificationServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IChatNotificationService));
        if (notificationServiceDescriptor != null)
            services.Remove(notificationServiceDescriptor);
        
        services.AddSingleton(MessageServiceMock.Object);
        services.AddSingleton(ChatNotificationServiceMock.Object);
        
        var chatServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IChatService));
        if (chatServiceDescriptor != null)
            services.Remove(chatServiceDescriptor);
        
        ChatServiceMock
            .Setup(x => x.EnsureUserIsParticipantAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        services.AddSingleton(new CurrentUserProvider(CurrentUserId));
        
        services.AddSingleton(ChatServiceMock.Object);
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
    }
}