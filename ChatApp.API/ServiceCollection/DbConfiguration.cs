using ChatApp.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.ServiceCollection;

public static class DbConfiguration
{
    public static void AddDbServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ChatAppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString(nameof(ChatAppDbContext)),
                b => b.MigrationsAssembly("ChatApp.Migrations")));
    }
}