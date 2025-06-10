using ChatApp.DataAccess.Interfaces;
using ChatApp.DataAccess.Repositories;

namespace ChatApp.API.ServiceCollection;

public static class RepositoryConfiguration
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}