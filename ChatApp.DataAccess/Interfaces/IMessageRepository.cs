using ChatApp.Domain.Models;

namespace ChatApp.DataAccess.Interfaces;

public interface IMessageRepository
{
    Task<Message> AddMessageAsync(Message message);
    Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId, int page = 1, int pageSize = 50);
    Task<Message?> GetMessageByIdAsync(Guid messageId);
    Task UpdateMessageAsync(Message message);
    Task DeleteMessageAsync(Guid messageId);
    Task<IEnumerable<Message>> SearchMessagesAsync(Guid chatId, string query);

}