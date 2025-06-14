namespace ChatApp.Business.Interfaces.Services;

public interface IChatNotificationService
{
    Task NotifyMessageSentAsync(Guid chatId, Guid senderId, string text, DateTime sentAt);
    Task NotifyMessageEditedAsync(Guid chatId, Guid messageId, string newText, DateTime editedAt);
    Task NotifyMessageDeletedAsync(Guid chatId, Guid messageId);
}