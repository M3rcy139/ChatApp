using ChatApp.Business.Interfaces;
using ChatApp.Business.Interfaces.Services;
using ChatApp.DataAccess.Interfaces;
using ChatApp.Domain.Models;

namespace ChatApp.Business.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IChatRepository _chatRepository;
    public MessageService(IMessageRepository messageRepository, IChatRepository chatRepository)
    {
        _messageRepository = messageRepository;
        _chatRepository = chatRepository;
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

        return await _messageRepository.AddMessageAsync(message);
    }

    public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId, int page, int pageSize)
    {
        return await _messageRepository.GetMessagesByChatIdAsync(chatId, page, pageSize);
    }
}