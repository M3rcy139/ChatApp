using ChatApp.API.ServiceCollection;
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

    services.AddAuthenticationConfiguration(configuration);

    var app = builder.Build();

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