using ChatApp.Domain.Models;

namespace ChatApp.DataAccess.Interfaces;

public interface IChatRepository
{
    Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId);
    Task<Chat> CreateChatAsync(Chat chat);
    Task<Chat?> GetChatByIdAsync(Guid chatId);
}
