using ChatApp.Business.Interfaces.Cache;
using ChatApp.Business.Interfaces.Services;
using ChatApp.Business.Services;
using ChatApp.Business.Services.CacheServices;

namespace ChatApp.API.ServiceCollection;

public static class ServiceConfiguration
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IUserService, UserService>();
        
        services.AddScoped<IMessageCacheService, MessageCacheService>();
        services.AddScoped<IChatCacheService, ChatCacheService>();
    }
}