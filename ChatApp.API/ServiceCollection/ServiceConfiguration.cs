using ChatApp.Business.Interfaces.Services;
using ChatApp.Business.Services;

namespace ChatApp.API.ServiceCollection;

public static class ServiceConfiguration
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IUserService, UserService>();
    }
}