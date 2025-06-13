using ChatApp.API.Hubs;
using ChatApp.API.ServiceCollection;
using ChatApp.DataAccess;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Host.ConfigureLogging(configuration);

try
{
    Log.Information("Initializing the application.");


    services.AddDbServices(configuration);

    services.AddControllersAndSwagger();

    services.AddOptions(configuration);
    services.AddAuthServices();
    services.AddServices();
    services.AddRepositories();

    services.AddRedisConfiguration(configuration);
    
    services.AddAuthenticationConfiguration(configuration);
    
    services.AddSignalR();

    var app = builder.Build();
    
    
    var env = app.Services.GetRequiredService<IWebHostEnvironment>();
    if (!env.IsEnvironment("Testing"))
    {
        await app.MigrateDataBaseAsync();
    }

    app.MapHub<ChatHub>("/chathub");
    
    app.ConfigureMiddleware(builder.Environment);
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application is stopped due to an exception.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }