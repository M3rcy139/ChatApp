using ChatApp.Business.Interfaces;
using ChatApp.Business.Interfaces.Cache;
using ChatApp.Business.Interfaces.Services;
using ChatApp.DataAccess.Interfaces;
using ChatApp.Domain.Constants;
using ChatApp.Domain.Extensions;
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

    public async Task<Chat> CreateChatAsync(Guid creatorUserId, string chatName, List<Guid> participantUserIds)
    {
        var users = await GetAndValidateUsersAsync(participantUserIds.Append(creatorUserId).Distinct().ToList());

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
        var chat = await _chatRepository.GetChatByIdAsync(chatId);
        chat.ValidateEntity(ErrorMessages.ChatNotFound);
        
        if (chat!.Users.All(u => u.Id != userId))
            throw new UnauthorizedAccessException(ErrorMessages.NotParticipantOfChat);
    }

    private async Task<List<User>> GetAndValidateUsersAsync(List<Guid> userIds)
    {
        var users = new List<User>(userIds.Count());
    
        foreach (var userId in userIds)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            user.ValidateEntity(ErrorMessages.UserNotFound);
            users.Add(user!);
        }
    
        return users;
    }
}