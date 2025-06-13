using ChatApp.Business.Interfaces.Cache;
using ChatApp.DataAccess.Interfaces;
using ChatApp.Domain.Models;
using Moq;

namespace ChatApp.Tests.Mocks;

public class ChatServiceMocks
{
    public static void SetupGetUserById(Mock<IUserRepository> userRepoMock, Guid userId, User? user)
    {
        userRepoMock.Setup(r => r.GetUserByIdAsync(userId))
            .ReturnsAsync(user);
    }

    public static void SetupCreateChat(Mock<IChatRepository> chatRepoMock, Chat createdChat)
    {
        chatRepoMock.Setup(r => r.CreateChatAsync(It.IsAny<Chat>()))
            .ReturnsAsync(createdChat);
    }

    public static void SetupGetCachedUserChats(Mock<IChatCacheService> cacheMock, Guid userId, IEnumerable<Chat>? cachedChats)
    {
        cacheMock.Setup(c => c.GetCachedUserChatsAsync(userId))
            .ReturnsAsync(cachedChats);
    }

    public static void SetupGetChatsByUserId(Mock<IChatRepository> chatRepoMock, Guid userId, IEnumerable<Chat> chats)
    {
        chatRepoMock.Setup(r => r.GetChatsByUserIdAsync(userId))
            .ReturnsAsync(chats);
    }

    public static void SetupGetChatById(Mock<IChatRepository> chatRepoMock, Guid chatId, Chat? chat)
    {
        chatRepoMock.Setup(r => r.GetChatByIdAsync(chatId))
            .ReturnsAsync(chat);
    }
}