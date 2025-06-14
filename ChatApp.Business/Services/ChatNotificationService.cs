using ChatApp.Business.Contracts;
using ChatApp.Business.Hubs;
using ChatApp.Business.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Business.Services;

public class ChatNotificationService : IChatNotificationService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatNotificationService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyMessageSentAsync(Guid chatId, Guid senderId, string text, DateTime sentAt)
    {
        return _hubContext.Clients.Group(chatId.ToString()).SendAsync(SignalRMethods.ReceiveMessage, new
        {
            ChatId = chatId,
            UserId = senderId,
            Text = text,
            SentAt = sentAt
        });
    }

    public Task NotifyMessageEditedAsync(Guid chatId, Guid messageId, string newText, DateTime editedAt)
    {
        return _hubContext.Clients.Group(chatId.ToString()).SendAsync(SignalRMethods.MessageEdited, new
        {
            ChatId = chatId,
            MessageId = messageId,
            NewText = newText,
            EditedAt = editedAt
        });
    }

    public Task NotifyMessageDeletedAsync(Guid chatId, Guid messageId)
    {
        return _hubContext.Clients.Group(chatId.ToString()).SendAsync(SignalRMethods.MessageDeleted, new
        {
            ChatId = chatId,
            MessageId = messageId
        });
    }
}