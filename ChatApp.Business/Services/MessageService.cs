using ChatApp.Business.Interfaces.Cache;
using ChatApp.Business.Interfaces.Services;
using ChatApp.DataAccess.Interfaces;
using ChatApp.Domain.Constants;
using ChatApp.Domain.Models;

namespace ChatApp.Business.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageCacheService _cacheService;
    public MessageService(IMessageRepository messageRepository, IMessageCacheService cacheService)
    {
        _messageRepository = messageRepository;
        _cacheService = cacheService;
    }

    public async Task<Message> SendMessageAsync(Guid chatId, Guid senderId, string text)
    {
        var message = new Message
        {
            ChatId = chatId,
            SenderId = senderId,
            Text = text,
            SentAt = DateTime.UtcNow
        };

        var saved = await _messageRepository.AddMessageAsync(message);
        await _cacheService.CacheMessageAsync(saved);
        return saved;
    }

    public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId, int page, int pageSize)
    {
        var cached = await _cacheService.GetCachedMessagesAsync(chatId, page, pageSize);

        if (cached.Count() < pageSize)
        {
            var messages = await _messageRepository.GetMessagesByChatIdAsync(chatId, page, pageSize);
            if (messages.Any())
                await _cacheService.CacheMessagesAsync(chatId, messages);
            return messages;
        }

        return cached;
    }
    
    public async Task<Message> EditMessageAsync(Guid chatId, Guid messageId, Guid userId, string newText)
    {
        var message = await GetAndValidateMessagesAsync(messageId, chatId, userId);
        
        message.Text = newText;
        message.EditedAt = DateTime.UtcNow;

        await _messageRepository.UpdateMessageAsync(message);
        await _cacheService.CacheMessageAsync(message);

        return message;
    }

    public async Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid userId)
    {
        var message = await GetAndValidateMessagesAsync(messageId, chatId, userId);

        await _messageRepository.DeleteMessageAsync(message);
        await _cacheService.DeleteMessageFromCacheAsync(chatId, messageId);
    }
    
    public async Task<IEnumerable<Message>> SearchMessagesAsync(Guid chatId, Guid userId, string query)
    {
        var tsQuery = query + ":*";
        
        return await _messageRepository.SearchMessagesAsync(chatId, tsQuery);
    }

    private async Task<Message> GetAndValidateMessagesAsync(Guid messageId, Guid chatId, Guid userId)
    {
        var message = await _messageRepository.GetMessageByIdAsync(messageId);

        if (message == null || message.ChatId != chatId)
            throw new ArgumentException(ErrorMessages.MessageNotFound);

        if (message.SenderId != userId)
            throw new UnauthorizedAccessException(ErrorMessages.YouCanOnlyEditOwnMessages);
        
        return message;
    }
}