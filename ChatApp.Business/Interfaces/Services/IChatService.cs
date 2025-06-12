using ChatApp.Domain.Models;

namespace ChatApp.Business.Interfaces.Services;

public interface IChatService
{
    Task<Chat> CreateChatAsync(Guid creatorUserId, string chatName, List<Guid> participantUserIds);
    Task<IEnumerable<Chat>> GetUserChatsAsync(Guid userId);
    Task EnsureUserIsParticipantAsync(Guid chatId, Guid userId);
}