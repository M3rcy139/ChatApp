using ChatApp.Domain.Models;

namespace ChatApp.Business.Interfaces.Services;

public interface IMessageService
{
    Task<Message> SendMessageAsync(Guid chatId, Guid senderId, string text);
    Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId, int page, int pageSize);
    Task<Message> EditMessageAsync(Guid chatId, Guid messageId, Guid userId, string newText);
    Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid userId);
    Task<IEnumerable<Message>> SearchMessagesAsync(Guid chatId, Guid userId, string query);
}