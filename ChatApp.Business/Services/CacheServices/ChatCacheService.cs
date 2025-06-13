using System.Text.Json;
using System.Text.Json.Serialization;
using ChatApp.Business.Interfaces.Cache;
using ChatApp.Domain.Models;
using StackExchange.Redis;

namespace ChatApp.Business.Services.CacheServices;

public class ChatCacheService : IChatCacheService
{
    private readonly IDatabase _redisDb;
    private static readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(5);

    public ChatCacheService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task CacheUserChatsAsync(Guid userId, IEnumerable<Chat> chats)
    {
        var key = GetUserChatsKey(userId);
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = false
        };

        var json = JsonSerializer.Serialize(chats, options);
        await _redisDb.StringSetAsync(key, json, _cacheTtl);
    }

    public async Task<IEnumerable<Chat>?> GetCachedUserChatsAsync(Guid userId)
    {
        var key = GetUserChatsKey(userId);
        var value = await _redisDb.StringGetAsync(key);

        if (value.IsNullOrEmpty)
            return null;
        
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve
        };

        return JsonSerializer.Deserialize<IEnumerable<Chat>>(value!, options)!;
    }

    public async Task InvalidateUserChatsCacheAsync(Guid userId)
    {
        var key = GetUserChatsKey(userId);
        await _redisDb.KeyDeleteAsync(key);
    }

    private string GetUserChatsKey(Guid userId) => $"user:chats:{userId}";
}