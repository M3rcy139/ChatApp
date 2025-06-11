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
    
    public async Task<Message?> GetMessageByIdAsync(Guid messageId)
    {
        return await _context.Messages.FindAsync(messageId);
    }

    public async Task UpdateMessageAsync(Message message)
    {
        _context.Messages.Update(message);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMessageAsync(Guid messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message != null)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Message>> SearchMessagesAsync(Guid chatId, string query)
    {
        var tsQuery = query + ":*"; 

        return await _context.Messages
            .Where(m => m.ChatId == chatId)
            .Where(m =>
                EF.Functions.ToTsVector("russian", m.Text)
                    .Matches(EF.Functions.PlainToTsQuery("russian", tsQuery)))
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();
    }
}