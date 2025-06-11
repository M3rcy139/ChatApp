using ChatApp.Domain.Models;

namespace ChatApp.Business.Interfaces.Cache;

public interface IChatCacheService
{
    Task<IEnumerable<Chat>?> GetCachedUserChatsAsync(Guid userId);
    Task CacheUserChatsAsync(Guid userId, IEnumerable<Chat> chats);
    Task InvalidateUserChatsCacheAsync(Guid userId);
}