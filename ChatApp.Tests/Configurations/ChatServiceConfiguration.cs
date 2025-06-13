using ChatApp.Business.Interfaces.Cache;
using ChatApp.Business.Services;
using ChatApp.DataAccess.Interfaces;
using Moq;

namespace ChatApp.Tests.Configurations;

public class ChatServiceConfiguration
{
    public Mock<IChatRepository> ChatRepoMock { get; }
    public Mock<IUserRepository> UserRepoMock { get; }
    public Mock<IChatCacheService> ChatCacheMock { get; }

    public ChatService Service { get; }

    public ChatServiceConfiguration()
    {
        ChatRepoMock = new Mock<IChatRepository>();
        UserRepoMock = new Mock<IUserRepository>();
        ChatCacheMock = new Mock<IChatCacheService>();
        
        Service = new ChatService(
            ChatRepoMock.Object,
            UserRepoMock.Object,
            ChatCacheMock.Object
        );
    }

    public void ResetMocks()
    {
        ChatRepoMock.Invocations.Clear();
        UserRepoMock.Invocations.Clear();
        ChatCacheMock.Invocations.Clear();
    }
}