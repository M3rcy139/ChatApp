using ChatApp.Domain.Models;

namespace ChatApp.DataAccess.Interfaces;

public interface IMessageRepository
{
    Task<Message> AddMessageAsync(Message message);
    Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId, int page = 1, int pageSize = 50);
}