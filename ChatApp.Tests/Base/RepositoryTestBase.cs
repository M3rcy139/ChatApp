using ChatApp.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Tests.Base;

public abstract class RepositoryTestBase : IDisposable
{
    protected readonly ChatAppDbContext Context;

    protected RepositoryTestBase()
    {
        var options = new DbContextOptionsBuilder<ChatAppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new ChatAppDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}