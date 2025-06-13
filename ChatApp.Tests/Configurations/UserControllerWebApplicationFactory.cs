using ChatApp.Business.Interfaces.Services;
using ChatApp.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
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
            
            services.AddDbContext<ChatAppDbContext>(options =>
            {
                options.UseNpgsql("Host=postgres;User ID=postgres;Password=1234;Port=5432;Database=chatapp_test");
            });
            
            UserServiceMock = new Mock<IUserService>();
            services.AddSingleton(UserServiceMock.Object);
            
            var userServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IUserService));
            if (userServiceDescriptor != null)
            {
                services.Remove(userServiceDescriptor);
            }

            UserServiceMock = new Mock<IUserService>();
            services.AddSingleton(UserServiceMock.Object);
        });
    }
}