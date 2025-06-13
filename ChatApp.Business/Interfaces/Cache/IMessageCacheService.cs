using ChatApp.Domain.Models;

namespace ChatApp.Business.Interfaces.Cache;

public interface IMessageCacheService
{
    Task CacheMessageAsync(Message message);
    Task CacheMessagesAsync(Guid chatId, IEnumerable<Message> messages);
    Task<IEnumerable<Message>> GetCachedMessagesAsync(Guid chatId, int page, int pageSize);
    Task DeleteMessageFromCacheAsync(Guid chatId, Guid messageId);

}