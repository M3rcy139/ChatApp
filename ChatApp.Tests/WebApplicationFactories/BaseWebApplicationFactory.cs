using ChatApp.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Tests.WebApplicationFactories;

public abstract class BaseWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ChatAppDbContext>));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);
            
            services.AddDbContext<ChatAppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            ConfigureMocks(services);
            
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ChatAppDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected abstract void ConfigureMocks(IServiceCollection services);
}