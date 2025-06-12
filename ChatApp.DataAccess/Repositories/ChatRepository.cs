using ChatApp.DataAccess.Interfaces;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.DataAccess.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly ChatAppDbContext _context;
    public ChatRepository(ChatAppDbContext context) => _context = context;

    public async Task<Chat> CreateChatAsync(Chat chat)
    {
        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();
        return chat;
    }

    public async Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId)
    {
        return await _context.Chats
            .Where(c => c.Users.Any(u => u.Id == userId))
            .ToListAsync();
    }

    public async Task<Chat?> GetChatByIdAsync(Guid chatId)
    {
        return await _context.Chats
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.Id == chatId);
    }
}