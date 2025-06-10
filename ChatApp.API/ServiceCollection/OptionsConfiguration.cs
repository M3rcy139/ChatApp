using ChatApp.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;

namespace ChatApp.API.ServiceCollection;

public static class OptionsConfiguration
{
    public static void AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
    }
}