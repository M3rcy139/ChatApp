using ChatApp.Business.Interfaces.Cache;
using ChatApp.Business.Services;
using ChatApp.DataAccess.Interfaces;
using Moq;

namespace ChatApp.Tests.Configurations;

public class MessageServiceConfiguration
{
    public Mock<IMessageRepository> MessageRepositoryMock { get; }
    public Mock<IMessageCacheService> MessageCacheMock { get; }

    public MessageService Service { get; }

    public MessageServiceConfiguration()
    {
        MessageRepositoryMock = new Mock<IMessageRepository>();
        MessageCacheMock = new Mock<IMessageCacheService>();

        Service = new MessageService(
            MessageRepositoryMock.Object,
            MessageCacheMock.Object
        );
    }

    public void ResetMocks()
    {
        MessageRepositoryMock.Invocations.Clear();
        MessageCacheMock.Invocations.Clear();
    }
}