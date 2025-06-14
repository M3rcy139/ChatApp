using ChatApp.Business.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ChatApp.Tests.WebApplicationFactories;

public class UserControllerWebApplicationFactory : BaseWebApplicationFactory<Program>
{
    public Mock<IUserService> UserServiceMock { get; private set; } = new();

    protected override void ConfigureMocks(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUserService));
        if (descriptor != null)
            services.Remove(descriptor);
        
        services.AddSingleton(UserServiceMock.Object);
    }
}