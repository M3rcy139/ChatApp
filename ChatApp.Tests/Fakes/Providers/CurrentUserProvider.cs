namespace ChatApp.Tests.Fakes.Providers;

public class CurrentUserProvider(Guid currentUserId)
{
    public Guid CurrentUserId { get; set; } = currentUserId;
}