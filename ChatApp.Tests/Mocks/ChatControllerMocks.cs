using ChatApp.Business.DTOs.Requests;
using ChatApp.Business.Interfaces.Services;
using ChatApp.Domain.Models;
using Moq;

namespace ChatApp.Tests.Mocks;

public static class ChatControllerMocks
{
    public static void SetupCreateChat(Mock<IChatService> chatServiceMock, Guid userId, Guid chatId, CreateChatRequest request)
    {
        chatServiceMock.Setup(cs => cs.CreateChatAsync(userId, request.Name, request.ParticipantUserIds))
            .ReturnsAsync(new Chat { Id = chatId, Name = request.Name });
    }
    
    public static void SetupGetChat(Mock<IChatService> chatServiceMock, Guid userId, List<Chat> chats)
    {
        chatServiceMock.Setup(cs => cs.GetUserChatsAsync(userId))
            .ReturnsAsync(chats);
    }
}