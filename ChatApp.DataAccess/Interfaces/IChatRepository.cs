using ChatApp.Domain.Models;

namespace ChatApp.DataAccess.Interfaces;

public interface IChatRepository
{
    Task<Chat> CreateChatAsync(Chat chat);
    Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId);
    Task<Chat?> GetChatByIdAsync(Guid chatId);
}
