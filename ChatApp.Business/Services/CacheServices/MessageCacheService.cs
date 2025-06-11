using StackExchange.Redis;
using System.Text.Json;
using ChatApp.Business.Interfaces.Cache;
using ChatApp.Domain.Models;

namespace ChatApp.Business.Services.CacheServices;

public class MessageCacheService : IMessageCacheService
{
    private readonly IDatabase _redisDb;
    private static readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

    public MessageCacheService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task CacheMessageAsync(Message message)
    {
        var setKey = GetSortedSetKey(message.ChatId);
        var hashKey = GetHashKey(message.ChatId);
        var messageId = message.Id.ToString();
        var score = message.SentAt.Ticks;

        var json = JsonSerializer.Serialize(message);

        var tran = _redisDb.CreateTransaction();
        _ = tran.SortedSetAddAsync(setKey, messageId, score);
        _ = tran.HashSetAsync(hashKey, messageId, json);
        _ = tran.KeyExpireAsync(setKey, _cacheTtl);
        _ = tran.KeyExpireAsync(hashKey, _cacheTtl);
        await tran.ExecuteAsync();
    }

    public async Task CacheMessagesAsync(Guid chatId, IEnumerable<Message> messages)
    {
        var setKey = GetSortedSetKey(chatId);
        var hashKey = GetHashKey(chatId);

        var sortedSetEntries = messages
            .Select(m => new SortedSetEntry(m.Id.ToString(), m.SentAt.Ticks))
            .ToArray();

        var hashEntries = messages
            .Select(m => new HashEntry(m.Id.ToString(), JsonSerializer.Serialize(m)))
            .ToArray();

        var tran = _redisDb.CreateTransaction();
        _ = tran.SortedSetAddAsync(setKey, sortedSetEntries);
        _ = tran.HashSetAsync(hashKey, hashEntries);
        _ = tran.KeyExpireAsync(setKey, _cacheTtl);
        _ = tran.KeyExpireAsync(hashKey, _cacheTtl);
        await tran.ExecuteAsync();
    }

    public async Task<IEnumerable<Message>> GetCachedMessagesAsync(Guid chatId, int page, int pageSize)
    {
        var setKey = GetSortedSetKey(chatId);
        var hashKey = GetHashKey(chatId);
        var start = (page - 1) * pageSize;
        var stop = start + pageSize - 1;

        var messageIds = await _redisDb.SortedSetRangeByRankAsync(
            setKey,
            start,
            stop,
            Order.Descending); 

        if (messageIds.Length == 0)
            return Enumerable.Empty<Message>();

        var hashValues = await _redisDb.HashGetAsync(hashKey, messageIds);

        var messages = new List<Message>();

        for (int i = 0; i < messageIds.Length; i++)
        {
            if (hashValues[i].HasValue)
            {
                var msg = JsonSerializer.Deserialize<Message>(hashValues[i]);
                if (msg != null)
                    messages.Add(msg);
            }
        }

        return messages.OrderByDescending(m => m.SentAt); 
    }

    public async Task DeleteMessageFromCacheAsync(Guid chatId, Guid messageId)
    {
        var setKey = GetSortedSetKey(chatId);
        var hashKey = GetHashKey(chatId);
        var messageIdStr = messageId.ToString();

        var tran = _redisDb.CreateTransaction();
        _ = tran.SortedSetRemoveAsync(setKey, messageIdStr);
        _ = tran.HashDeleteAsync(hashKey, messageIdStr);
        await tran.ExecuteAsync();
    }

    private static string GetSortedSetKey(Guid chatId) => $"chat:messages:{chatId}:ids";
    private static string GetHashKey(Guid chatId) => $"chat:messages:{chatId}:data";
}
