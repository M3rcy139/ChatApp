using ChatApp.Business.DTOs.Requests;
using ChatApp.Business.Interfaces.Services;
using ChatApp.Domain.Models;
using Moq;

namespace ChatApp.Tests.Mocks;

public static class ChatControllerMocks
{
    public static void SetupCreateChat(Mock<IChatService> chatServiceMock, Guid userId, Chat chat, 
        CreateChatRequest request)
    {
        chatServiceMock.Setup(cs => cs.CreateChatAsync(userId, request.Name, request.ParticipantUserIds))
            .ReturnsAsync(chat);
    }

    public static void  SetupCreateChatThrows(Mock<IChatService> chatServiceMock, Exception exception)
    {
        chatServiceMock.Setup(cs => cs.CreateChatAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<Guid>>()))
            .ThrowsAsync(exception);
    }
    
    public static void SetupGetChat(Mock<IChatService> chatServiceMock, Guid userId, List<Chat> chats)
    {
        chatServiceMock.Setup(cs => cs.GetUserChatsAsync(userId))
            .ReturnsAsync(chats);
    }
}