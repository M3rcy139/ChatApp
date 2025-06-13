using ChatApp.Business.Interfaces.Services;
using ChatApp.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ChatApp.Tests.Configurations;

public class WebApplicationFactoryWithMocks : WebApplicationFactory<Program>
{
    public Guid CurrentUserId { get; } = Guid.NewGuid();
    public Mock<IChatService> ChatServiceMock { get; } = new();

    public WebApplicationFactoryWithMocks()
    {
        TestAuthHandler.CurrentUserId = CurrentUserId; 
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider));
            if (descriptor != null)
                services.Remove(descriptor);
            
            var jwtDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>));
            if (jwtDescriptor != null)
                services.Remove(jwtDescriptor);
            
            var authDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(Microsoft.AspNetCore.Authentication.IAuthenticationService));
            if (authDescriptor != null)
                services.Remove(authDescriptor);
            
            services.AddDbContext<ChatAppDbContext>(options =>
            {
                options.UseNpgsql("Host=postgres;User ID=postgres;Password=1234;Port=5432;Database=chatapp_test");
            });
            
            var chatServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IChatService));
            if (chatServiceDescriptor != null)
            {
                services.Remove(chatServiceDescriptor);
            }
            
            services.AddSingleton(ChatServiceMock.Object);
            
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
        });
    }
}