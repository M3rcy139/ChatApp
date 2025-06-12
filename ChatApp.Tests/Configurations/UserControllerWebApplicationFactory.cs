using ChatApp.Business.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ChatApp.Tests.Configurations;

public class UserControllerWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IUserService> UserServiceMock { get; private set; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IUserService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            UserServiceMock = new Mock<IUserService>();
            services.AddSingleton(UserServiceMock.Object);
        });
    }
}