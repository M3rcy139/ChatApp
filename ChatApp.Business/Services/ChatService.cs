using ChatApp.Business.Interfaces;
using ChatApp.Business.Interfaces.Cache;
using ChatApp.Business.Interfaces.Services;
using ChatApp.DataAccess.Interfaces;
using ChatApp.Domain.Models;

namespace ChatApp.Business.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository; 
    private readonly IChatCacheService _cacheService;
    public ChatService(IChatRepository chatRepository, IUserRepository userRepository, IChatCacheService cacheService)
    {
        _chatRepository = chatRepository;
        _userRepository = userRepository;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<Chat>> GetUserChatsAsync(Guid userId)
    {
        var cached = await _cacheService.GetCachedUserChatsAsync(userId);
        if (cached != null)
            return cached;

        var chats = await _chatRepository.GetChatsByUserIdAsync(userId);
        await _cacheService.CacheUserChatsAsync(userId, chats);
        return chats;
    }

    public async Task<Chat> CreateChatAsync(Guid creatorUserId, string chatName, IEnumerable<Guid> participantUserIds)
    {
        var users = new List<User>();
        
        foreach (var userId in participantUserIds.Append(creatorUserId).Distinct())
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) throw new Exception($"User {userId} not found");
            users.Add(user);
        }

        var chat = new Chat
        {
            Name = chatName,
            Users = users
        };

        var created = await _chatRepository.CreateChatAsync(chat);
        
        foreach (var user in users)
        {
            await _cacheService.InvalidateUserChatsCacheAsync(user.Id);
        }

        return created;
    }
    
    public async Task EnsureUserIsParticipantAsync(Guid chatId, Guid userId)
    {
        var chat = await _chatRepository.GetChatByIdAsync(chatId)!;
        if (!chat.Users.Any(u => u.Id == userId))
            throw new UnauthorizedAccessException("User is not a participant of this chat.");
    }

}