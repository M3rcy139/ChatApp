using ChatApp.DataAccess.Interfaces;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.DataAccess.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ChatAppDbContext _context;
    public MessageRepository(ChatAppDbContext context) => _context = context;

    public async Task<Message> AddMessageAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId, int page = 1, int pageSize = 50)
    {
        return await _context.Messages
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}