using StackExchange.Redis;

namespace ChatApp.API.ServiceCollection;

public static class RedisConfiguration
{
    public static void AddRedisConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis");

        services.AddSingleton<IConnectionMultiplexer>(provider =>
            ConnectionMultiplexer.Connect(connectionString!));
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
        });
    }
}