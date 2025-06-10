using ChatApp.Business.Interfaces.Authentication;
using ChatApp.Infrastructure.Authentication;

namespace ChatApp.API.ServiceCollection;

public static class AuthServicesConfiguration
{
    public static void AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
    }
}