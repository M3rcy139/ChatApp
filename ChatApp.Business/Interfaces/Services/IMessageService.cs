using ChatApp.Domain.Models;

namespace ChatApp.Business.Interfaces.Services;

public interface IMessageService
{
    Task<Message> SendMessageAsync(Guid chatId, Guid senderId, string text);
    Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId, int page, int pageSize);
}