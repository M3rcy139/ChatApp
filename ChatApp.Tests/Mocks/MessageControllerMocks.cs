using ChatApp.Business.Interfaces.Services;
using ChatApp.Domain.Models;
using Moq;

namespace ChatApp.Tests.Mocks;

public static class MessageControllerMocks
{
    public static void SetupSendMessage(Mock<IMessageService> messageServiceMock, Guid chatId, Guid userId, 
        Message message)
    {
        messageServiceMock.Setup(x => x.SendMessageAsync(chatId, userId, message.Text))
            .ReturnsAsync(message);
    }

    public static void SetupNotifyMessageSent(Mock<IChatNotificationService> notificationServiceMock, Guid chatId,
        Guid userId, string text, DateTime sentAt)
    {
        notificationServiceMock.Setup(x => x.NotifyMessageSentAsync(chatId, userId, text, sentAt))
            .Returns(Task.CompletedTask);
    }

    public static void SetupEditMessage(Mock<IMessageService> messageServiceMock, Guid chatId, Guid userId, 
        string newText, Message message)
    {
        messageServiceMock.Setup(x => x.EditMessageAsync(chatId, message.Id, userId, newText))
            .ReturnsAsync(message);
    }

    public static void SetupNotifyMessageEdited(Mock<IChatNotificationService> notificationServiceMock, Guid chatId,
        Message message)
    {
        notificationServiceMock.Setup(x => x.NotifyMessageEditedAsync(chatId, message.Id, 
                message.Text, message.EditedAt))
            .Returns(Task.CompletedTask);
    }

    public static void SetupDeleteMessage(Mock<IMessageService> messageServiceMock, Guid chatId, Guid messageId,
        Guid userId)
    {
        messageServiceMock.Setup(x => x.DeleteMessageAsync(chatId, messageId, userId))
            .Returns(Task.CompletedTask);
    }

    public static void SetupNotifyMessageDeleted(Mock<IChatNotificationService> notificationServiceMock, Guid chatId,
        Guid messageId)
    {
        notificationServiceMock.Setup(x => x.NotifyMessageDeletedAsync(chatId, messageId))
            .Returns(Task.CompletedTask);
    }

    public static void SetupGetMessagesByChatId(Mock<IMessageService> messageServiceMock, Guid chatId, int page, 
        int pageSize, List<Message> messages)
    {
        messageServiceMock.Setup(x => x.GetMessagesByChatIdAsync(chatId, page, pageSize))
            .ReturnsAsync(messages);
    }

    public static void SetupSearchMessages(Mock<IMessageService> messageServiceMock, Guid chatId, Guid userId, 
        string query, List<Message> filteredMessages)
    {
        messageServiceMock.Setup(x => x.SearchMessagesAsync(chatId, userId, query))
            .ReturnsAsync(filteredMessages);
    }
}
