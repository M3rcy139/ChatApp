using ChatApp.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.ServiceCollection;

public static class MigrateDataBase
{
    public static async Task<IHost> MigrateDataBaseAsync(this IHost host, int retryCount = 10)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChatAppDbContext>();

        for (var i = 0; i < retryCount; i++)
        {
            try
            {
                dbContext.Database.Migrate();
                return host;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] Failed to connect to DB. Attempt {i + 1} of {retryCount}: {ex.Message}");
                await Task.Delay(2000); 
            }
        }

        throw new Exception("Unable to connect to the database after several retries.");
    }
}