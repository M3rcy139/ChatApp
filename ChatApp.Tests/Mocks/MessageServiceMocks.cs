using ChatApp.Business.Interfaces.Cache;
using ChatApp.DataAccess.Interfaces;
using ChatApp.Domain.Constants;
using ChatApp.Domain.Models;
using Moq;

namespace ChatApp.Tests.Mocks;

public class MessageServiceMocks
{
    public static void SetupAddMessage(Mock<IMessageRepository> repoMock)
    {
        repoMock.Setup(r => r.AddMessageAsync(It.IsAny<Message>()))
                .ReturnsAsync((Message m) => m);
    }

    public static void SetupGetCachedMessages(Mock<IMessageCacheService> cacheMock, Guid chatId, int page, int pageSize, List<Message> result)
    {
        cacheMock.Setup(c => c.GetCachedMessagesAsync(chatId, page, pageSize))
                 .ReturnsAsync(result);
    }

    public static void SetupGetMessagesFromDb(Mock<IMessageRepository> repoMock, Guid chatId, int page, int pageSize, List<Message> result)
    {
        repoMock.Setup(r => r.GetMessagesByChatIdAsync(chatId, page, pageSize))
                .ReturnsAsync(result);
    }
    

    public static void SetupGetMessageById(Mock<IMessageRepository> repoMock, Guid messageId, Message? result)
    {
        repoMock.Setup(r => r.GetMessageByIdAsync(messageId))
                .ReturnsAsync(result);
    }
    

    public static void SetupGetMessageByIdThrows(Mock<IMessageRepository> repoMock, Guid messageId)
    {
        repoMock.Setup(r => r.GetMessageByIdAsync(messageId))
                .ThrowsAsync(new ArgumentException(ErrorMessages.MessageNotFound));
    }

    public static void SetupSearchMessages(Mock<IMessageRepository> repoMock, Guid chatId, string tsQuery, List<Message> result)
    {
        repoMock.Setup(r => r.SearchMessagesAsync(chatId, tsQuery))
                .ReturnsAsync(result);
    }
}